using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;
using ForgeFlow.Api.Options;
using Microsoft.Extensions.Options;

namespace ForgeFlow.Api.Services;

public class AutodeskTokenProvider(
    IHttpClientFactory httpClientFactory,
    IOptions<AutodeskOptions> options,
    ILogger<AutodeskTokenProvider> logger) : IAutodeskTokenProvider
{
    /// <summary>Name of the configured <see cref="HttpClient"/> this provider resolves.</summary>
    public const string HttpClientName = "autodesk";

    private const string TokenPath = "authentication/v2/token";

    // Renew slightly early so a token never expires mid-request.
    private static readonly TimeSpan ExpirySkew = TimeSpan.FromSeconds(60);

    private readonly SemaphoreSlim _refreshLock = new(1, 1);
    private readonly AutodeskOptions _options = options.Value;

    private AutodeskAccessToken? _cached;

    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default) =>
        (await GetTokenAsync(cancellationToken)).AccessToken;

    public async Task<AutodeskAccessToken> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        if (IsCurrent())
        {
            return _cached!;
        }

        await _refreshLock.WaitAsync(cancellationToken);
        try
        {
            // Another caller may have refreshed while we waited.
            if (IsCurrent())
            {
                return _cached!;
            }

            var response = await RequestTokenAsync(cancellationToken);
            _cached = new AutodeskAccessToken(
                response.AccessToken,
                response.TokenType,
                DateTimeOffset.UtcNow.AddSeconds(response.ExpiresIn));

            logger.LogInformation(
                "Retrieved Autodesk access token, valid for {ExpiresIn}s.",
                response.ExpiresIn);

            return _cached;
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    private bool IsCurrent() =>
        _cached is not null && DateTimeOffset.UtcNow < _cached.ExpiresAtUtc - ExpirySkew;

    private async Task<TokenResponse> RequestTokenAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.ClientId) ||
            string.IsNullOrWhiteSpace(_options.ClientSecret))
        {
            throw new InvalidOperationException(
                "Autodesk:ClientId and Autodesk:ClientSecret are not configured.");
        }

        var credentials = Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{_options.ClientId}:{_options.ClientSecret}"));

        using var request = new HttpRequestMessage(HttpMethod.Post, TokenPath);
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials",
            ["scope"] = _options.Scopes,
        });

        var httpClient = httpClientFactory.CreateClient(HttpClientName);
        using var response = await httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException(
                $"Autodesk token request failed with {(int)response.StatusCode}: {body}");
        }

        return await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken)
            ?? throw new HttpRequestException("Autodesk token response was empty.");
    }

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
