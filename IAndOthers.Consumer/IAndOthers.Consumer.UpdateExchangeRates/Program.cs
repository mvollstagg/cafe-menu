using IAndOthers.Core.Configs;
using IAndOthers.Core.Data.Services;
using IAndOthers.Core.IoC;
using IAndOthers.Core.MassTransit;
using IAndOthers.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        // Configure DbContext for MySQL
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(hostContext.Configuration.GetConnectionString("DefaultConnection")));

        // Add application services
        IOIoCRegistrar.RegisterDependencies(services);
        // Add repository services
        services.AddScoped(typeof(IIORepository<,>), typeof(IORepositoryBase<,>));

        // Resolve RabbitMQ settings from appsettings.json
        services.Configure<RabbitMqConfig>(hostContext.Configuration.GetSection("RabbitMqConfig"));
        RabbitMqConfig rabbitMqConfig = hostContext.Configuration.GetSection("RabbitMqConfig").Get<RabbitMqConfig>();

        // Register dependencies
        IOIoCRegistrar.RegisterDependencies(services);

        // Resolve the service provider
        IODependencyResolver.SetServiceProvider(services.BuildServiceProvider());

        

        // Configure MassTransit
        var massTransitHelper = IODependencyResolver.Resolve<MassTransitBusHelper>();
        massTransitHelper.ConfigureMassTransit(services);
    })
    .Build();

// Add a try-catch block to handle errors and log the output
var massTransitHelper = IODependencyResolver.Resolve<MassTransitBusHelper>();
try
{
    // Start the bus
    await massTransitHelper.StartBusAsync();

    // Log the startup message in green
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"[{DateTime.UtcNow}] - Application started and running...");

    // Run the host
    await host.RunAsync();

    // Log success message when the application shuts down
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"[{DateTime.UtcNow}] - Application shutdown completed successfully.");
}
catch (Exception ex)
{
    // Log error message in red
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"[{DateTime.UtcNow}] - An error occurred: {ex.Message}");
    Console.WriteLine(ex.StackTrace);
}
finally
{
    // Stop the bus to avoid memory leaks
    await massTransitHelper.StopBusAsync();

    // Reset the console color
    Console.ResetColor();
}
