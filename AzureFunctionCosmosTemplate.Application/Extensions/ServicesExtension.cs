using AzureFunctionCosmosTemplate.Application.Interfaces;
using AzureFunctionCosmosTemplate.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AzureFunctionCosmosTemplate.Application.Extensions;

public static class ServicesExtension
{
    public static IServiceCollection AddServicesDependencies(this IServiceCollection services)
    {
        // Registrar servicios de aplicación
        services.AddScoped<IUsersService, UsersService>();

        return services;
    }
}
