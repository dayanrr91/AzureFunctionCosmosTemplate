using AzureFunctionCosmosTemplate.Application.Interfaces;
using AzureFunctionCosmosTemplate.Domain.DTOs;
using AzureFunctionCosmosTemplate.Domain.Entities;
using AzureFunctionCosmosTemplate.Repository.Repositories.UsersRepository;
using Microsoft.Extensions.Logging;

namespace AzureFunctionCosmosTemplate.Application.Services;

/// <summary>
/// Service implementation for managing user business logic operations.
/// Handles mapping between DTOs and entities to maintain proper separation of concerns.
/// </summary>
public class UsersService : IUsersService
{
    private readonly IUsersCosmosRepository _usersRepository;
    private readonly ILogger<UsersService> _logger;

    public UsersService(IUsersCosmosRepository usersRepository, ILogger<UsersService> logger)
    {
        _usersRepository = usersRepository;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all users from the system as DTOs.
    /// </summary>
    /// <returns>A collection of user DTOs.</returns>
    public async Task<IEnumerable<UsersDto>> GetAllUsersAsync()
    {
        try
        {
            _logger.LogInformation("Getting all users from service");
            var users = await _usersRepository.GetAllAsync();
            return users.ToDto();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all users in service");
            throw;
        }
    }

    /// <summary>
    /// Retrieves a specific user by their unique identifier as a DTO.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>The user DTO if found; otherwise, null.</returns>
    public async Task<UsersDto?> GetUserByIdAsync(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning("GetUserByIdAsync called with empty id");
                return null;
            }

            _logger.LogInformation("Getting user by id: {UserId}", id);
            var user = await _usersRepository.GetByIdAsync(id);
            return user?.ToDto();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by id: {UserId}", id);
            throw;
        }
    }

    /// <summary>
    /// Creates a new user in the system from a DTO.
    /// </summary>
    /// <param name="userDto">The user DTO with the data to create.</param>
    /// <returns>The newly created user as a DTO.</returns>
    public async Task<UsersDto> CreateUserAsync(UsersDto userDto)
    {
        try
        {
            if (userDto == null)
            {
                throw new ArgumentNullException(nameof(userDto));
            }

            // Validaciones de negocio
            if (string.IsNullOrEmpty(userDto.Email))
            {
                throw new ArgumentException("Email is required");
            }

            if (string.IsNullOrEmpty(userDto.FirstName))
            {
                throw new ArgumentException("FirstName is required");
            }

            // Verificar si el email ya existe
            var existingUser = await _usersRepository.GetByEmailAsync(userDto.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException($"User with email {userDto.Email} already exists");
            }

            _logger.LogInformation("Creating new user with email: {Email}", userDto.Email);
            var userEntity = userDto.ToEntity();
            var createdUser = await _usersRepository.CreateAsync(userEntity);
            return createdUser.ToDto();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            throw;
        }
    }

    /// <summary>
    /// Updates an existing user's information from a DTO.
    /// </summary>
    /// <param name="id">The unique identifier of the user to update.</param>
    /// <param name="userDto">The user DTO with updated information.</param>
    /// <returns>The updated user as a DTO.</returns>
    public async Task<UsersDto> UpdateUserAsync(string id, UsersDto userDto)
    {
        try
        {
            if (userDto == null)
            {
                throw new ArgumentNullException(nameof(userDto));
            }

            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("Id is required for update");
            }

            // Verificar que el usuario existe
            var existingUser = await _usersRepository.GetByIdAsync(id);
            if (existingUser == null)
            {
                throw new InvalidOperationException($"User with id {id} not found");
            }

            // Si se cambió el email, verificar que no exista otro usuario con ese email
            if (!string.Equals(existingUser.Email, userDto.Email, StringComparison.OrdinalIgnoreCase))
            {
                var userWithSameEmail = await _usersRepository.GetByEmailAsync(userDto.Email);
                if (userWithSameEmail != null && userWithSameEmail.Id != id)
                {
                    throw new InvalidOperationException($"Another user with email {userDto.Email} already exists");
                }
            }

            _logger.LogInformation("Updating user with id: {UserId}", id);
            var updatedEntity = userDto.UpdateEntity(existingUser);
            var updatedUser = await _usersRepository.UpdateAsync(updatedEntity);
            return updatedUser.ToDto();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with id: {UserId}", id);
            throw;
        }
    }

    /// <summary>
    /// Deletes a user from the system by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user to delete.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    public async Task DeleteUserAsync(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("Id is required for delete");
            }

            // Verificar que el usuario existe
            var existingUser = await _usersRepository.GetByIdAsync(id);
            if (existingUser == null)
            {
                throw new InvalidOperationException($"User with id {id} not found");
            }

            _logger.LogInformation("Deleting user with id: {UserId}", id);
            await _usersRepository.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with id: {UserId}", id);
            throw;
        }
    }

    /// <summary>
    /// Retrieves a user by their email address as a DTO.
    /// </summary>
    /// <param name="email">The email address of the user.</param>
    /// <returns>The user DTO if found; otherwise, null.</returns>
    public async Task<UsersDto?> GetUserByEmailAsync(string email)
    {
        try
        {
            if (string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("GetUserByEmailAsync called with empty email");
                return null;
            }

            _logger.LogInformation("Getting user by email: {Email}", email);
            var user = await _usersRepository.GetByEmailAsync(email);
            return user?.ToDto();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by email: {Email}", email);
            throw;
        }
    }

    /// <summary>
    /// Retrieves all active users from the system as DTOs.
    /// </summary>
    /// <returns>A collection of active user DTOs.</returns>
    public async Task<IEnumerable<UsersDto>> GetActiveUsersAsync()
    {
        try
        {
            _logger.LogInformation("Getting active users from service");
            var users = await _usersRepository.GetActiveUsersAsync();
            return users.ToDto();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active users in service");
            throw;
        }
    }
}
