using AlbionOnline.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("settings.json", false).Build();

var serviceProvider = new ServiceCollection();
serviceProvider.Configure<Configurations>(configuration.GetSection("Devlopment"));
serviceProvider.AddLogging();
serviceProvider.AddOptions();