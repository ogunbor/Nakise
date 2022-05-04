using Microsoft.Extensions.Configuration;
using Shared.ResourceParameters;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Utils.ExternalServices
{
    public static class HttpClientExtensions
    {
        public static async Task<T?> ReadContentAs<T>(this HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
                throw new ApplicationException($"Something went wrong calling the API: {response.ReasonPhrase}");

            var dataAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<T>(dataAsString);
        }

        public static async Task<HttpResponseMessage> PostAsJson<T>(this HttpClient httpClient, string url, T? data, IConfiguration config)
        {
            var dataAsString = JsonSerializer.Serialize(data);
            var content = new StringContent(dataAsString, Encoding.UTF8);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            httpClient.DefaultRequestHeaders.Add("X-API-KEY", config["X-API-KEY"]);

            return await httpClient.PostAsync(url, content);
        }

        public static Task<HttpResponseMessage> PutAsJson<T>(this HttpClient httpClient, string url, T data, IConfiguration config)
        {
            var dataAsString = JsonSerializer.Serialize(data);
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            httpClient.DefaultRequestHeaders.Add("X-API-KEY", config["X-API-KEY"]);

            return httpClient.PutAsync(url, content);
        }

        public static Task<HttpResponseMessage> GetAsJson(this HttpClient httpClient, string url, IConfiguration config, AssessmentParameters? parameters = null)
        {
            url = UrlBuilder(url, parameters);

            httpClient.DefaultRequestHeaders.Add("X-API-KEY", config["X-API-KEY"]);

            return httpClient.GetAsync(url);
        }

        private static string UrlBuilder(string url, AssessmentParameters? parameters)
        {
            if (parameters is not null)
            {
                if (!string.IsNullOrWhiteSpace(parameters.Search))
                {
                    url = $"{url}?search={parameters.Search}";
                }

                if (parameters.PageSize != default && parameters.PageSize is not null)
                {
                    if (url.Contains('?'))
                    {
                        url = $"{url}&page_size={parameters.PageSize}";
                    }
                    else
                    {
                        url = $"{url}?page_size={parameters.PageSize}";
                    }

                }

                if (parameters.Page != default && parameters.Page is not null)
                {
                    if (url.Contains('?'))
                    {
                        url = $"{url}&page={parameters.Page}";
                    }
                    else
                    {
                        url = $"{url}?page={parameters.Page}";
                    }
                }
            }

            return url;
        }
    }
}
