using StackExchange.Redis;
using WorkerService1;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddSingleton<IConnectionMultiplexer>(c =>
        {
            var configuration = ConfigurationOptions.Parse("localhost",
                true);
            return ConnectionMultiplexer.Connect(configuration);
        });    })
    .Build();

await host.RunAsync();