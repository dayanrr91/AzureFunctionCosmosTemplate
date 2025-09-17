using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using System.Text.Json;

namespace AzureFunctionCosmosTemplate.Function.Extensions;

public static class HttpRequestDataCustomExtensions
{
    public static async Task<T?> ToDeserializedRequestBody<T>(this HttpRequestData httpRequestData)
    {
        using var reader = new StreamReader(httpRequestData.Body);
        var requestBody = await reader.ReadToEndAsync();

        // Usa System.Text.Json
        var request = JsonSerializer.Deserialize<T>(requestBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        });

        return request;
    }

    public static async Task<HttpResponseData> WriteJsonResponseAsync<T>(this HttpRequestData req, T value)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(value);
        return response;
    }

    public static async Task<HttpResponseData> WriteJsonExceptionResponseAsync<T>(this HttpRequestData req, T value, HttpStatusCode code = HttpStatusCode.NotFound)
    {
        var response = req.CreateResponse(code);
        await response.WriteAsJsonAsync(value);
        return response;
    }
}
