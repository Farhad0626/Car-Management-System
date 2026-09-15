using Terminal.Gui;
using CarApp.Data;
using CarApp.Repository;
using CarApp.Service;
using CarApp.UI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
    .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: false)
    .AddEnvironmentVariables()
    .Build();

var services = new ServiceCollection();

services
    .AddOptions<DatabaseOptions>()
    .Bind(configuration.GetSection(DatabaseOptions.SectionName));
    //.ValidateDataAnnotations();

services
    .AddSingleton<IConfiguration>(configuration)
    .AddTransient<IDatabaseConnectionFactory, PostgresConnectionFactory>()
    .AddTransient<ICarRepository, CarRepository>()
    .AddTransient<CarService>()
    .AddTransient<MainWindow>();

var provider = services.BuildServiceProvider(validateScopes: true);

// Fail fast with a clear error if the connection string is missing/invalid,
// instead of waiting for the first database call. Accessing .Value is what
// actually runs the DataAnnotations validation (it's evaluated lazily otherwise).
// _ = provider.GetRequiredService<IOptions<DatabaseOptions>>().Value;

Application.Init();
Application.Run(provider.GetRequiredService<MainWindow>());
Application.Shutdown();
