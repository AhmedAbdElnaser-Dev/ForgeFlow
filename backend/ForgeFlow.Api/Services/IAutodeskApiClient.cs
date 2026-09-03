using ForgeFlow.Api.Models;

namespace ForgeFlow.Api.Services;

// Sends requests to Autodesk with a token for the scopes that call needs.
// The response is returned as-is: each service decides what its own failures mean.
public interface IAutodeskApiClient
{
    Task<HttpResponseMessage> GetAsync(
        string path,
        AutodeskScope scopes,
        CancellationToken cancellationToken = default);

    Task<HttpResponseMessage> PostJsonAsync<TBody>(
        string path,
        TBody body,
        AutodeskScope scopes,
        CancellationToken cancellationToken = default);

    Task<HttpResponseMessage> DeleteAsync(
        string path,
        AutodeskScope scopes,
        CancellationToken cancellationToken = default);

    Task<HttpResponseMessage> SendAsync(
        HttpMethod method,
        string path,
        AutodeskScope scopes,
        HttpContent? content = null,
        CancellationToken cancellationToken = default);
}
