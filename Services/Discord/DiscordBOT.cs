using AlbionOnline.JSON;
using AlbionOnline.Models;
using AlbionOnline.Services.Mongo;
using AlbionOnline.Settings;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbionOnline.Services.Discord
{
    public class DiscordBOT : IHostedService
    {
        private ILogger<DiscordBOT> Logger { get; }
        private IOptions<Configurations> Configurations { get; }
        private ServiceProvider Service { get; }
        private DiscordSocketClient Client { get; }

        public DiscordBOT(ILogger<DiscordBOT> logger,
            IOptions<Configurations> discordSettings,
            IJSONAlbion jsonAlbion,
            IMongoRepository<AccountModel> accountsRepository,
            IMongoRepository<DiscordGuildModel> discordGuildRepository
        )
        {
            Logger = logger;
            Configurations = discordSettings;
            Service = new ServiceCollection()
                .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
                {
                    GatewayIntents = GatewayIntents.All,
                    AlwaysDownloadUsers = true
                }))
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton(jsonAlbion)
                .AddSingleton(accountsRepository)
                .AddSingleton(discordGuildRepository)
                .BuildServiceProvider();
            Client = Service.GetRequiredService<DiscordSocketClient>();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
