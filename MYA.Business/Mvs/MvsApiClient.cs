using System.Diagnostics;
using System.Net.Http.Headers;

namespace MYA.Business.Mvs;

public sealed class MvsApiClient
{
    private readonly HttpClient _httpClient;

    public MvsApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<(string Result, long ElapsedMilliseconds)> GetAsync(
        string url, string apiKey, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            url);

        request.Headers.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", apiKey);

        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            using var response = await _httpClient.SendAsync(
                    request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                .ConfigureAwait(false);

            stopwatch.Stop();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"Errore durante la richiesta. " +
                    $"StatusCode={(int)response.StatusCode} " +
                    $"({response.StatusCode}) - " +
                    $"{response.ReasonPhrase}");
            }

            string result = await response.Content
                .ReadAsStringAsync(cancellationToken)
                .ConfigureAwait(false);

            return (
                result,
                stopwatch.ElapsedMilliseconds);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine(ex.ToString());
            Console.WriteLine(ex.InnerException?.Message);
            Console.WriteLine(ex.InnerException?.InnerException?.Message);
            stopwatch.Stop();
            throw;
        }
    }
}