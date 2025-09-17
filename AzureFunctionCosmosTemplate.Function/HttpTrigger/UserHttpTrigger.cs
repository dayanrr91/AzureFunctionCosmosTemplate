using AzureFunctionCosmosTemplate.Application.Interfaces;
using AzureFunctionCosmosTemplate.Domain.Entities;
using AzureFunctionCosmosTemplate.Function.Extensions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using System.Net;

namespace AzureFunctionCosmosTemplate.Function.HttpTrigger;

internal class UserHttpTrigger
{
    private readonly IUsersService _usersService;
    private readonly ILogger<UserHttpTrigger> _logger;

    public UserHttpTrigger(IUsersService usersService, ILogger<UserHttpTrigger> logger)
    {
        _usersService = usersService;
        _logger = logger;
    }

    [Function("get-users")]
    [OpenApiOperation(operationId: "get-users", tags: ["Users"], Description = "Method to get all users")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IEnumerable<Users>), Description = "Success")]
    public async Task<HttpResponseData> GetUsersAsync(
       [HttpTrigger(AuthorizationLevel.Anonymous,
        "get",
        Route = "users")]
        HttpRequestData req
       )
    {
        try
        {
            _logger.LogInformation("Getting all users");
            var users = await _usersService.GetAllUsersAsync();
            var response = await req.WriteJsonResponseAsync(users);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users");
            return await req.WriteJsonExceptionResponseAsync("Error getting users");
        }
    }

    [Function("get-user-by-id")]
    [OpenApiOperation(operationId: "get-user-by-id", tags: ["Users"], Description = "Method to get user by id")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Users), Description = "Success")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "text/plain", bodyType: typeof(string), Description = "User not found")]
    public async Task<HttpResponseData> GetUserByIdAsync(
       [HttpTrigger(AuthorizationLevel.Anonymous,
        "get",
        Route = "users/{id}")]
        HttpRequestData req,
        string id
       )
    {
        try
        {
            _logger.LogInformation("Getting user with id: {UserId}", id);
            var user = await _usersService.GetUserByIdAsync(id);

            if (user == null)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
                await notFoundResponse.WriteStringAsync("User not found");
                return notFoundResponse;
            }

            var response = await req.WriteJsonResponseAsync(user);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user with id: {UserId}", id);
            return await req.WriteJsonExceptionResponseAsync("Error getting user");
        }
    }

    [Function("create-user")]
    [OpenApiOperation(operationId: "create-user", tags: ["Users"], Description = "Method to create a new user")]
    [OpenApiRequestBody("application/json", typeof(Users), Description = "User data")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: "application/json", bodyType: typeof(Users), Description = "User created successfully")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Invalid request")]
    public async Task<HttpResponseData> CreateUserAsync(
       [HttpTrigger(AuthorizationLevel.Anonymous,
        "post",
        Route = "users")]
        HttpRequestData req
       )
    {
        try
        {
            _logger.LogInformation("Creating new user");
            var user = await req.ReadFromJsonAsync<Users>();

            if (user == null)
            {
                var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequestResponse.WriteStringAsync("Invalid user data");
                return badRequestResponse;
            }

            var createdUser = await _usersService.CreateUserAsync(user);
            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(createdUser);
            return response;
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid user data");
            var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequestResponse.WriteStringAsync(ex.Message);
            return badRequestResponse;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Business rule violation");
            var conflictResponse = req.CreateResponse(HttpStatusCode.Conflict);
            await conflictResponse.WriteStringAsync(ex.Message);
            return conflictResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return await req.WriteJsonExceptionResponseAsync("Error creating user");
        }
    }

    [Function("update-user")]
    [OpenApiOperation(operationId: "update-user", tags: ["Users"], Description = "Method to update an existing user")]
    [OpenApiRequestBody("application/json", typeof(Users), Description = "User data")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Users), Description = "User updated successfully")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "text/plain", bodyType: typeof(string), Description = "User not found")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Invalid request")]
    public async Task<HttpResponseData> UpdateUserAsync(
       [HttpTrigger(AuthorizationLevel.Anonymous,
        "put",
        Route = "users/{id}")]
        HttpRequestData req,
        string id
       )
    {
        try
        {
            _logger.LogInformation("Updating user with id: {UserId}", id);
            var user = await req.ReadFromJsonAsync<Users>();

            if (user == null)
            {
                var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequestResponse.WriteStringAsync("Invalid user data");
                return badRequestResponse;
            }

            user.Id = id; // Asegurar que el ID coincida con el de la ruta
            var updatedUser = await _usersService.UpdateUserAsync(user);
            var response = await req.WriteJsonResponseAsync(updatedUser);
            return response;
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid user data");
            var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequestResponse.WriteStringAsync(ex.Message);
            return badRequestResponse;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Business rule violation or user not found");
            var statusCode = ex.Message.Contains("not found") ? HttpStatusCode.NotFound : HttpStatusCode.Conflict;
            var errorResponse = req.CreateResponse(statusCode);
            await errorResponse.WriteStringAsync(ex.Message);
            return errorResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with id: {UserId}", id);
            return await req.WriteJsonExceptionResponseAsync("Error updating user");
        }
    }

    [Function("delete-user")]
    [OpenApiOperation(operationId: "delete-user", tags: ["Users"], Description = "Method to delete a user")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NoContent, contentType: "text/plain", bodyType: typeof(string), Description = "User deleted successfully")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "text/plain", bodyType: typeof(string), Description = "User not found")]
    public async Task<HttpResponseData> DeleteUserAsync(
       [HttpTrigger(AuthorizationLevel.Anonymous,
        "delete",
        Route = "users/{id}")]
        HttpRequestData req,
        string id
       )
    {
        try
        {
            _logger.LogInformation("Deleting user with id: {UserId}", id);
            await _usersService.DeleteUserAsync(id);

            var response = req.CreateResponse(HttpStatusCode.NoContent);
            return response;
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            _logger.LogWarning(ex, "User not found for deletion");
            var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
            await notFoundResponse.WriteStringAsync(ex.Message);
            return notFoundResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with id: {UserId}", id);
            return await req.WriteJsonExceptionResponseAsync("Error deleting user");
        }
    }

    [Function("get-user-by-email")]
    [OpenApiOperation(operationId: "get-user-by-email", tags: ["Users"], Description = "Method to get user by email")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Users), Description = "Success")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "text/plain", bodyType: typeof(string), Description = "User not found")]
    public async Task<HttpResponseData> GetUserByEmailAsync(
       [HttpTrigger(AuthorizationLevel.Anonymous,
        "get",
        Route = "users/email/{email}")]
        HttpRequestData req,
        string email
       )
    {
        try
        {
            _logger.LogInformation("Getting user with email: {Email}", email);
            var user = await _usersService.GetUserByEmailAsync(email);

            if (user == null)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
                await notFoundResponse.WriteStringAsync("User not found");
                return notFoundResponse;
            }

            var response = await req.WriteJsonResponseAsync(user);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user with email: {Email}", email);
            return await req.WriteJsonExceptionResponseAsync("Error getting user");
        }
    }

    [Function("get-active-users")]
    [OpenApiOperation(operationId: "get-active-users", tags: ["Users"], Description = "Method to get all active users")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IEnumerable<Users>), Description = "Success")]
    public async Task<HttpResponseData> GetActiveUsersAsync(
       [HttpTrigger(AuthorizationLevel.Anonymous,
        "get",
        Route = "users/active")]
        HttpRequestData req
       )
    {
        try
        {
            _logger.LogInformation("Getting all active users");
            var users = await _usersService.GetActiveUsersAsync();
            var response = await req.WriteJsonResponseAsync(users);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active users");
            return await req.WriteJsonExceptionResponseAsync("Error getting active users");
        }
    }
}
