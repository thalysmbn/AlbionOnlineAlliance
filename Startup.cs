using AlbionOnline.JSON;
using AlbionOnline.Services.Discord;
using AlbionOnline.Services.Mongo;
using AlbionOnline.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

await Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, serviceProvider) =>
    {
        var configuration = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("settings.json", false).Build();

        serviceProvider.Configure<Configurations>(configuration.GetSection("Devlopment"));
        serviceProvider.AddLogging();
        serviceProvider.AddOptions();
        serviceProvider.AddSingleton<IJSONAlbion, JSONAlbion>();

        serviceProvider.AddSingleton(typeof(IMongoRepository<>), typeof(MongoRepository<>));

        serviceProvider.AddHostedService<DiscordBOT>();

        var service = serviceProvider.BuildServiceProvider();
        var discord = service.GetService<DiscordBOT>();
    }).RunConsoleAsync();

await Task.Delay(-1);