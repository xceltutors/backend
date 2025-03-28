﻿using Microsoft.Extensions.DependencyInjection;
using Domain.Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xcel.Services.Auth.Implementations.Services;
using Xcel.Services.Auth.Interfaces.Repositories;
using Xcel.Services.Auth.Interfaces.Services;

namespace Xcel.Services.Auth;

public static class DependencyInjection
{
    public static IServiceCollection AddXcelAuthServices<TOtpRepository, TPersonsRepository, TRolesRepository, TPersonRoleRepository>(
        this IServiceCollection services,
        AuthOptions authOptions)
        where TOtpRepository : class, IOtpRepository
        where TPersonsRepository : class, IPersonsRepository
        where TRolesRepository : class, IRolesRepository
        where TPersonRoleRepository : class, IPersonRoleRepository
    {
        services.TryAddScoped<IOtpRepository, TOtpRepository>();
        services.TryAddScoped<IPersonsRepository, TPersonsRepository>();
        services.TryAddScoped<IRolesRepository, TRolesRepository>();
        services.TryAddScoped<IPersonRoleRepository, TPersonRoleRepository>();
        services.TryAddSingleton(TimeProvider.System);

        services
            .AddScoped<IOtpService, OtpService>()
            .AddScoped<IAccountService, AccountService>()
            .AddScoped<IRoleService, RoleService>()
            .AddScoped<IPersonRoleService, PersonRoleService>()
            .AddScoped<IJwtService, JwtService>();

        services
            .AddSingleton(authOptions);

        return services;
    }
}