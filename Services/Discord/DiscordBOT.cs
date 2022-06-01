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
        private readonly ILogger<DiscordBOT> _logger;
        private readonly IOptions<Configurations> _configurations;
        private readonly ServiceProvider _service;
        private readonly DiscordSocketClient _client;

        public DiscordBOT(ILogger<DiscordBOT> logger,
            IOptions<Configurations> configurations,
            IJSONAlbion jsonAlbion,
            IMongoRepository<AccountModel> accountsRepository,
            IMongoRepository<DiscordGuildModel> discordGuildRepository
        )
        {
            _logger = logger;
            _configurations = configurations;
            _service = new ServiceCollection()
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
            _client = _service.GetRequiredService<DiscordSocketClient>();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Starting BOT service {_configurations.Value.ClientToken}.");
            await _client.LoginAsync(TokenType.Bot, _configurations.Value.ClientToken);
            await _client.StartAsync();
            await _service.GetRequiredService<CommandHandlingService>().InitializeAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _client.DisposeAsync();
            return Task.CompletedTask;
        }
    }
}
