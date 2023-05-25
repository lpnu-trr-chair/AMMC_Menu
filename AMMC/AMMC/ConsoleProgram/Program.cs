using ConsoleProgram;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var services = ConfigureServices();

var serviceProvider = services.BuildServiceProvider();

// calls the Run method in App, which is replacing Main
#pragma warning disable CS8602 // Dereference of a possibly null reference.
serviceProvider.GetService<App>().Run();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

static IServiceCollection ConfigureServices()
{
    IServiceCollection services = new ServiceCollection();

    var configuration = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
           .Build();

    services.AddSingleton<IConfiguration>(configuration);
    // Add app
    services.AddTransient<App>();

    return services;
}