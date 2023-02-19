using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GeekShopping.Web.Extensions;

public static class HttpClientExtensions
{
    private static MediaTypeHeaderValue contentType = new MediaTypeHeaderValue("application/json");
    
    public static async Task<T> ReadContentAs<T>(this HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode) throw
                 new ApplicationException(
                     $"Something went wrong calling the API: " +
                     $"{response.ReasonPhrase}");
        var dataAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonConvert.DeserializeObject<T>(dataAsString)!;
    }

    public static Task<HttpResponseMessage> PostAsJson<T>(this HttpClient httpClient, string url, T data)
    {
        var dataAsString = JsonConvert.SerializeObject(data);
        var content = new StringContent(dataAsString);
        content.Headers.ContentType = contentType;
        return httpClient.PostAsync(url, content);
    }

    public static Task<HttpResponseMessage> PutAsJson<T>(this HttpClient httpClient, string url, T data)
    {
        var dataAsString = JsonConvert.SerializeObject(data);
        var content = new StringContent(dataAsString);
        content.Headers.ContentType = contentType;
        return httpClient.PutAsync(url, content);
    }
}
