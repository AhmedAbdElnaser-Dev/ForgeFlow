using System.Net.Http.Headers;
using System.Text;
using ForgeFlow.Api.Models;
using ForgeFlow.Api.Options;
using Microsoft.Extensions.Options;

namespace ForgeFlow.Api.Services;

public class AutodeskTokenService(
    IHttpClientFactory httpClientFactory,
    IOptions<AutodeskOptions> options,
    ILogger<AutodeskTokenService> logger) : IAutodeskTokenService
{
    public const string HttpClientName = "autodesk";

    private const string TokenPath = "authentication/v2/token";

    // Renew early so a token cannot expire mid-request.
    private static readonly TimeSpan RenewBefore = TimeSpan.FromSeconds(60);

    private readonly AutodeskOptions _options = options.Value;
    private readonly Dictionary<AutodeskScope, AutodeskAccessToken> _tokens = [];
    private readonly SemaphoreSlim _refreshLock = new(1, 1);

    public async Task<string> GetAccessTokenAsync(
        AutodeskScope scopes,
        CancellationToken cancellationToken = default) =>
        (await GetTokenAsync(scopes, cancellationToken)).AccessToken;

    public async Task<AutodeskAccessToken> GetTokenAsync(
        AutodeskScope scopes,
        CancellationToken cancellationToken = default)
    {
        if (scopes == AutodeskScope.None)
        {
            throw new ArgumentException("At least one scope is required.", nameof(scopes));
        }

        if (TryGetCachedToken(scopes, out var token))
        {
            return token;
        }

        // One lock, so a burst of callers triggers a single request instead of one each.
        await _refreshLock.WaitAsync(cancellationToken);
        try
        {
            // Another caller may have fetched this scope while we waited.
            if (TryGetCachedToken(scopes, out token))
            {
                return token;
            }

            token = await FetchTokenAsync(scopes, cancellationToken);
            _tokens[scopes] = token;

            return token;
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    private bool TryGetCachedToken(AutodeskScope scopes, out AutodeskAccessToken token)
    {
        lock (_tokens)
        {
            if (_tokens.TryGetValue(scopes, out var cached) &&
                DateTimeOffset.UtcNow < cached.ExpiresAtUtc - RenewBefore)
            {
                token = cached;
                return true;
            }
        }

        token = null!;
        return false;
    }

    private async Task<AutodeskAccessToken> FetchTokenAsync(
        AutodeskScope scopes,
        CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient(HttpClientName);

        using var request = CreateTokenRequest(scopes);
        using var response = await client.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException(
                $"Autodesk token request failed with {(int)response.StatusCode}: {body}");
        }

        var payload = await response.Content.ReadFromJsonAsync<AutodeskTokenResponse>(cancellationToken)
            ?? throw new HttpRequestException("Autodesk token response was empty.");

        logger.LogInformation(
            "Retrieved Autodesk access token for [{Scopes}], valid for {ExpiresIn}s.",
            scopes.ToWireFormat(),
            payload.ExpiresIn);

        return new AutodeskAccessToken(
            payload.AccessToken,
            payload.TokenType,
            DateTimeOffset.UtcNow.AddSeconds(payload.ExpiresIn));
    }

    private HttpRequestMessage CreateTokenRequest(AutodeskScope scopes)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, TokenPath);

        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", EncodeCredentials());
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials",
            ["scope"] = scopes.ToWireFormat(),
        });

        return request;
    }

    private string EncodeCredentials()
    {
        if (string.IsNullOrWhiteSpace(_options.ClientId) ||
            string.IsNullOrWhiteSpace(_options.ClientSecret))
        {
            throw new InvalidOperationException(
                "Autodesk:ClientId and Autodesk:ClientSecret are not configured.");
        }

        var pair = $"{_options.ClientId}:{_options.ClientSecret}";
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(pair));
    }
}
