using AzureFunctionCosmosTemplate.Application.Interfaces;
using AzureFunctionCosmosTemplate.Domain.Entities;
using AzureFunctionCosmosTemplate.Repository.Repositories.UsersRepository;
using Microsoft.Extensions.Logging;

namespace AzureFunctionCosmosTemplate.Application.Services;

public class UsersService : IUsersService
{
    private readonly IUsersCosmosRepository _usersRepository;
    private readonly ILogger<UsersService> _logger;

    public UsersService(IUsersCosmosRepository usersRepository, ILogger<UsersService> logger)
    {
        _usersRepository = usersRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<Users>> GetAllUsersAsync()
    {
        try
        {
            _logger.LogInformation("Getting all users from service");
            return await _usersRepository.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all users in service");
            throw;
        }
    }

    public async Task<Users?> GetUserByIdAsync(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning("GetUserByIdAsync called with empty id");
                return null;
            }

            _logger.LogInformation("Getting user by id: {UserId}", id);
            return await _usersRepository.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by id: {UserId}", id);
            throw;
        }
    }

    public async Task<Users> CreateUserAsync(Users user)
    {
        try
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            // Validaciones de negocio
            if (string.IsNullOrEmpty(user.Email))
            {
                throw new ArgumentException("Email is required");
            }

            if (string.IsNullOrEmpty(user.FirstName))
            {
                throw new ArgumentException("FirstName is required");
            }

            // Verificar si el email ya existe
            var existingUser = await _usersRepository.GetByEmailAsync(user.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException($"User with email {user.Email} already exists");
            }

            _logger.LogInformation("Creating new user with email: {Email}", user.Email);
            return await _usersRepository.CreateAsync(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            throw;
        }
    }

    public async Task<Users> UpdateUserAsync(Users user)
    {
        try
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrEmpty(user.Id))
            {
                throw new ArgumentException("Id is required for update");
            }

            // Verificar que el usuario existe
            var existingUser = await _usersRepository.GetByIdAsync(user.Id);
            if (existingUser == null)
            {
                throw new InvalidOperationException($"User with id {user.Id} not found");
            }

            // Si se cambió el email, verificar que no exista otro usuario con ese email
            if (!string.Equals(existingUser.Email, user.Email, StringComparison.OrdinalIgnoreCase))
            {
                var userWithSameEmail = await _usersRepository.GetByEmailAsync(user.Email);
                if (userWithSameEmail != null && userWithSameEmail.Id != user.Id)
                {
                    throw new InvalidOperationException($"Another user with email {user.Email} already exists");
                }
            }

            _logger.LogInformation("Updating user with id: {UserId}", user.Id);
            return await _usersRepository.UpdateAsync(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with id: {UserId}", user?.Id);
            throw;
        }
    }

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

    public async Task<Users?> GetUserByEmailAsync(string email)
    {
        try
        {
            if (string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("GetUserByEmailAsync called with empty email");
                return null;
            }

            _logger.LogInformation("Getting user by email: {Email}", email);
            return await _usersRepository.GetByEmailAsync(email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by email: {Email}", email);
            throw;
        }
    }

    public async Task<IEnumerable<Users>> GetActiveUsersAsync()
    {
        try
        {
            _logger.LogInformation("Getting active users from service");
            return await _usersRepository.GetActiveUsersAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active users in service");
            throw;
        }
    }
}
