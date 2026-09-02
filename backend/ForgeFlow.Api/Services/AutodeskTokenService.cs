using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;
using ForgeFlow.Api.Models;
using ForgeFlow.Api.Options;
using Microsoft.Extensions.Options;

namespace ForgeFlow.Api.Services;

/// <summary>
/// Requests two-legged Autodesk tokens and keeps the current one in memory.
/// Registered as a singleton so every caller shares the same cached token.
/// </summary>
public class AutodeskTokenService(
    IHttpClientFactory httpClientFactory,
    IOptions<AutodeskOptions> options,
    ILogger<AutodeskTokenService> logger) : IAutodeskTokenService
{
    public const string HttpClientName = "autodesk";

    private const string TokenPath = "authentication/v2/token";

    /// <summary>Renew this early so a token cannot expire mid-request.</summary>
    private static readonly TimeSpan RenewBefore = TimeSpan.FromSeconds(60);

    private readonly AutodeskOptions _options = options.Value;
    private readonly AutodeskScope _scopes = AutodeskScopes.Parse(options.Value.Scopes);
    private readonly SemaphoreSlim _refreshLock = new(1, 1);

    private AutodeskAccessToken? _token;

    private bool HasUsableToken =>
        _token is not null && DateTimeOffset.UtcNow < _token.ExpiresAtUtc - RenewBefore;

    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default) =>
        (await GetTokenAsync(cancellationToken)).AccessToken;

    public async Task<AutodeskAccessToken> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        if (HasUsableToken)
        {
            return _token!;
        }

        // One lock so a burst of callers triggers a single request instead of one each.
        await _refreshLock.WaitAsync(cancellationToken);
        try
        {
            if (HasUsableToken)
            {
                return _token!;
            }

            _token = await FetchTokenAsync(cancellationToken);
            return _token;
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    private async Task<AutodeskAccessToken> FetchTokenAsync(CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient(HttpClientName);

        using var request = CreateTokenRequest();
        using var response = await client.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException(
                $"Autodesk token request failed with {(int)response.StatusCode}: {body}");
        }

        var payload = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken)
            ?? throw new HttpRequestException("Autodesk token response was empty.");

        logger.LogInformation(
            "Retrieved Autodesk access token for [{Scopes}], valid for {ExpiresIn}s.",
            _scopes.ToWireFormat(),
            payload.ExpiresIn);

        return new AutodeskAccessToken(
            payload.AccessToken,
            payload.TokenType,
            DateTimeOffset.UtcNow.AddSeconds(payload.ExpiresIn));
    }

    private HttpRequestMessage CreateTokenRequest()
    {
        if (_scopes == AutodeskScope.None)
        {
            throw new InvalidOperationException(
                $"Autodesk:Scopes contains no recognised scope (value: '{_options.Scopes}').");
        }

        var request = new HttpRequestMessage(HttpMethod.Post, TokenPath);

        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", EncodeCredentials());
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials",
            ["scope"] = _scopes.ToWireFormat(),
        });

        return request;
    }

    /// <summary>Base64 of "clientId:clientSecret", as the Basic scheme expects.</summary>
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

    /// <summary>Raw response shape. Private: nothing outside this service should see it.</summary>
    private sealed record TokenResponse
    {
        [JsonPropertyName("access_token")]
        public required string AccessToken { get; init; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; init; } = "Bearer";

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; init; }
    }
}
