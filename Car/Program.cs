using Terminal.Gui;
using CarApp.Data;
using CarApp.Repository;
using CarApp.Service;
using CarApp.UI;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services
    .AddTransient<IDatabase, Postgresdb>()
    .AddTransient<ICarRepository, CarRepository>()
    .AddTransient<CarService>()
    .AddTransient<MainWindow>();

var provider = services.BuildServiceProvider();

Application.Init();
Application.Run(provider.GetRequiredService<MainWindow>());
Application.Shutdown();