using System.Net.Http.Headers;
using ForgeFlow.Api.Models;

namespace ForgeFlow.Api.Services;

public class AutodeskApiClient(
    IHttpClientFactory httpClientFactory,
    IAutodeskTokenService tokenService) : IAutodeskApiClient
{
    public Task<HttpResponseMessage> GetAsync(
        string path,
        AutodeskScope scopes,
        CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Get, path, scopes, content: null, cancellationToken);

    public Task<HttpResponseMessage> PostJsonAsync<TBody>(
        string path,
        TBody body,
        AutodeskScope scopes,
        CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Post, path, scopes, JsonContent.Create(body), cancellationToken);

    public Task<HttpResponseMessage> DeleteAsync(
        string path,
        AutodeskScope scopes,
        CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Delete, path, scopes, content: null, cancellationToken);

    public async Task<HttpResponseMessage> SendAsync(
        HttpMethod method,
        string path,
        AutodeskScope scopes,
        HttpContent? content = null,
        CancellationToken cancellationToken = default)
    {
        var bearer = await tokenService.GetAccessTokenAsync(scopes, cancellationToken);

        using var request = new HttpRequestMessage(method, path) { Content = content };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearer);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var client = httpClientFactory.CreateClient(AutodeskTokenService.HttpClientName);

        return await client.SendAsync(request, cancellationToken);
    }
}
