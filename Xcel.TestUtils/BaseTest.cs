﻿using Application;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Infra;
using Infra.Repositories;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using Xcel.Config.Options;
using Xcel.Services.Auth;
using Xcel.Services.Auth.Interfaces.Repositories;
using Xcel.Services.Auth.Interfaces.Services;
using Xcel.Services.Email.Interfaces;
using Xcel.TestUtils.Mocks;
using Xunit;

namespace Xcel.TestUtils;

public abstract class BaseTest : IAsyncLifetime
{
    protected static ILogger<T> CreateLogger<T>() => NullLogger<T>.Instance;

    private ServiceProvider _serviceProvider = null!;
    private AppDbContext _context = null!;

    protected static FakeTimeProvider FakeTimeProvider { get; } = new(DateTimeOffset.UtcNow);

    protected ISender Sender => GetService<ISender>();
    protected ISubjectsRepository SubjectsRepository => GetService<ISubjectsRepository>();
    protected ITutorsRepository TutorsRepository => GetService<ITutorsRepository>();
    protected IPersonsRepository PersonsRepository => GetService<IPersonsRepository>();
    protected IRolesRepository RolesRepository => GetService<IRolesRepository>();
    protected IOtpRepository OtpRepository => GetService<IOtpRepository>();
    protected IPersonRoleRepository PersonRoleRepository => GetService<IPersonRoleRepository>();
    protected InMemoryFileService InMemoryFileService => (InMemoryFileService)GetService<IFileService>();
    protected InMemoryEmailSender InMemoryEmailSender => (InMemoryEmailSender)GetService<IEmailSender>();
    protected IEmailService EmailService => GetService<IEmailService>();
    protected IOtpService OtpService => GetService<IOtpService>();
    protected IAccountService AccountService => GetService<IAccountService>();
    protected IJwtService JwtService => GetService<IJwtService>();
    protected InfraOptions InfraOptions => GetService<InfraOptions>();
    protected AuthOptions AuthOptions => GetService<InfraOptions>().Auth;
    private AppDbContext Context => GetService<AppDbContext>();

    public virtual async Task InitializeAsync()
    {
        _serviceProvider = await CreateServiceProvider();
        _context = GetService<AppDbContext>();
        await EnsureDatabaseCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await EnsureDatabaseDeletedAsync();
        await _context.DisposeAsync();
        await _serviceProvider.DisposeAsync();
    }

    private static async Task<ServiceProvider> CreateServiceProvider()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("appsettings.test.json")
            .Build();

        var infraOptions = configuration.GetRequiredSection("Infra").Get<InfraOptions>()
                           ?? throw new Exception("It's mandatory to have the Infra configuration");

        infraOptions.Database.ConnectionString =
            infraOptions.Database.ConnectionString.Replace("<guid>", $"{Guid.NewGuid()}");

        var services = new ServiceCollection()
            .AddApplicationServices();

        await services.AddInfraServicesAsync(infraOptions, new(EnvironmentType.Production));

        services.AddSingleton(services);

        return MockServices(services)
            .BuildServiceProvider();
    }

    private static IServiceCollection MockServices(IServiceCollection services)
    {
        return services
            .AddSingleton<TimeProvider>(FakeTimeProvider)
            .AddScoped<IFileService, InMemoryFileService>()
            .AddScoped<IEmailSender, InMemoryEmailSender>();
    }

    private T GetService<T>() where T : class => _serviceProvider.GetRequiredService<T>();

    private async Task EnsureDatabaseCreatedAsync()
    {
        await Context.Database.EnsureCreatedAsync();
    }

    private async Task EnsureDatabaseDeletedAsync()
    {
        try
        {
            await Context.Database.EnsureDeletedAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting database: {ex}");
        }
    }
}