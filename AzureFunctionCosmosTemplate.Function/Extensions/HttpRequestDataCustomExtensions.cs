using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using System.Text.Json;

namespace AzureFunctionCosmosTemplate.Function.Extensions;

/// <summary>
/// Extension methods for HttpRequestData to provide standardized response patterns.
/// Ensures consistent response structure across all API endpoints.
/// </summary>
public static class HttpRequestDataCustomExtensions
{
    /// <summary>
    /// Deserializes the request body to the specified type using System.Text.Json.
    /// </summary>
    /// <typeparam name="T">The type to deserialize to.</typeparam>
    /// <param name="httpRequestData">The HTTP request data.</param>
    /// <returns>The deserialized object or null if deserialization fails.</returns>
    public static async Task<T?> ToDeserializedRequestBody<T>(this HttpRequestData httpRequestData)
    {
        using var reader = new StreamReader(httpRequestData.Body);
        var requestBody = await reader.ReadToEndAsync();

        // Uses System.Text.Json with case-insensitive property matching
        var request = JsonSerializer.Deserialize<T>(requestBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        });

        return request;
    }

    // Success Responses

    /// <summary>
    /// Creates a successful JSON response with HTTP 200 OK status.
    /// </summary>
    /// <typeparam name="T">The type of the response data.</typeparam>
    /// <param name="req">The HTTP request data.</param>
    /// <param name="value">The data to return in the response.</param>
    /// <returns>An HTTP response with the specified data.</returns>
    public static async Task<HttpResponseData> WriteJsonResponseAsync<T>(this HttpRequestData req, T value)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(value);
        return response;
    }

    /// <summary>
    /// Creates a successful JSON response with HTTP 201 Created status.
    /// </summary>
    /// <typeparam name="T">The type of the created resource.</typeparam>
    /// <param name="req">The HTTP request data.</param>
    /// <param name="value">The created resource to return in the response.</param>
    /// <returns>An HTTP response with the created resource.</returns>
    public static async Task<HttpResponseData> WriteCreatedResponseAsync<T>(this HttpRequestData req, T value)
    {
        var response = req.CreateResponse(HttpStatusCode.Created);
        await response.WriteAsJsonAsync(value);
        return response;
    }

    /// <summary>
    /// Creates a successful response with HTTP 204 No Content status.
    /// </summary>
    /// <param name="req">The HTTP request data.</param>
    /// <returns>An HTTP response with no content.</returns>
    public static HttpResponseData WriteNoContentResponse(this HttpRequestData req)
    {
        return req.CreateResponse(HttpStatusCode.NoContent);
    }

    // Error Responses

    /// <summary>
    /// Creates a bad request response with HTTP 400 status and error message.
    /// </summary>
    /// <param name="req">The HTTP request data.</param>
    /// <param name="message">The error message to return.</param>
    /// <returns>An HTTP response with the error message.</returns>
    public static async Task<HttpResponseData> WriteBadRequestResponseAsync(this HttpRequestData req, string message)
    {
        var response = req.CreateResponse(HttpStatusCode.BadRequest);
        await response.WriteStringAsync(message);
        return response;
    }

    /// <summary>
    /// Creates a not found response with HTTP 404 status and error message.
    /// </summary>
    /// <param name="req">The HTTP request data.</param>
    /// <param name="message">The error message to return.</param>
    /// <returns>An HTTP response with the error message.</returns>
    public static async Task<HttpResponseData> WriteNotFoundResponseAsync(this HttpRequestData req, string message)
    {
        var response = req.CreateResponse(HttpStatusCode.NotFound);
        await response.WriteStringAsync(message);
        return response;
    }

    /// <summary>
    /// Creates a conflict response with HTTP 409 status and error message.
    /// </summary>
    /// <param name="req">The HTTP request data.</param>
    /// <param name="message">The error message to return.</param>
    /// <returns>An HTTP response with the error message.</returns>
    public static async Task<HttpResponseData> WriteConflictResponseAsync(this HttpRequestData req, string message)
    {
        var response = req.CreateResponse(HttpStatusCode.Conflict);
        await response.WriteStringAsync(message);
        return response;
    }

    /// <summary>
    /// Creates an internal server error response with HTTP 500 status and error message.
    /// </summary>
    /// <param name="req">The HTTP request data.</param>
    /// <param name="message">The error message to return.</param>
    /// <returns>An HTTP response with the error message.</returns>
    public static async Task<HttpResponseData> WriteInternalServerErrorResponseAsync(this HttpRequestData req, string message)
    {
        var response = req.CreateResponse(HttpStatusCode.InternalServerError);
        await response.WriteStringAsync(message);
        return response;
    }

    // Generic Error Response (for backward compatibility)

    /// <summary>
    /// Creates a generic error response with the specified status code and message.
    /// </summary>
    /// <param name="req">The HTTP request data.</param>
    /// <param name="message">The error message to return.</param>
    /// <param name="statusCode">The HTTP status code for the response.</param>
    /// <returns>An HTTP response with the specified status and message.</returns>
    public static async Task<HttpResponseData> WriteErrorResponseAsync(this HttpRequestData req, string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
    {
        var response = req.CreateResponse(statusCode);
        await response.WriteStringAsync(message);
        return response;
    }

    /// <summary>
    /// Creates a generic JSON error response with the specified status code and data.
    /// </summary>
    /// <typeparam name="T">The type of the error data.</typeparam>
    /// <param name="req">The HTTP request data.</param>
    /// <param name="value">The error data to return in the response.</param>
    /// <param name="statusCode">The HTTP status code for the response.</param>
    /// <returns>An HTTP response with the specified status and data.</returns>
    public static async Task<HttpResponseData> WriteJsonExceptionResponseAsync<T>(this HttpRequestData req, T value, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
    {
        var response = req.CreateResponse(statusCode);
        await response.WriteAsJsonAsync(value);
        return response;
    }
}
