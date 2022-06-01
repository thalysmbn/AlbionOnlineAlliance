using Discord;
using Discord.Commands;

namespace AlbionOnline.Extensions
{
    public static class CommandServiceExtension
    {
        public static Embed GetDefaultHelpEmbed(this CommandService commandService, string command)
        {
            EmbedBuilder helpEmbedBuilder;
            var commandModules = commandService.GetModulesWithCommands();
            var moduleMatch = commandModules.FirstOrDefault(m => m.Name == command || m.Aliases.Contains(command));

            if (string.IsNullOrEmpty(command))
                helpEmbedBuilder = commandService.GenerateHelpCommandEmbed();
            else if (moduleMatch != null)
                helpEmbedBuilder = commandService.GenerateSpecificModuleHelpEmbed(moduleMatch);
            else
                helpEmbedBuilder = GenerateSpecificCommandHelpEmbed(commandService, command);

            helpEmbedBuilder.WithFooter(GenerateUsageFooterMessage());
            return helpEmbedBuilder.Build();
        }

        private static string GenerateUsageFooterMessage()
        {
            return "Use help [command module] or help [command name] for more information.";
        }

        private static IEnumerable<ModuleInfo> GetModulesWithCommands(this CommandService commandService)
        {
            return commandService.Modules.Where(module => module.Commands.Count > 0);
        }

        private static EmbedBuilder GenerateSpecificCommandHelpEmbed(this CommandService commandService, string command)
        {
            //TODO: This won't allow commands that ends with a number
            var isNumeric = int.TryParse(command[command.Length - 1].ToString(), out var pageNum);

            if (isNumeric)
                command = command.Substring(0, command.Length - 2);
            else
                pageNum = 1;

            var helpEmbedBuilder = new EmbedBuilder();
            var commandSearchResult = commandService.Search(command);

            var commandsInfoWeNeed = new List<CommandInfo>();

            if (commandSearchResult.IsSuccess)
            {
                commandsInfoWeNeed.AddRange(commandSearchResult.Commands.Select(c => c.Command));
            }
            else
            {
                var commandModulesList = commandService.Modules.ToList();
                foreach (var c in commandModulesList)
                    commandsInfoWeNeed.AddRange(c.Commands.Where(h =>
                        string.Equals(h.Name, command, StringComparison.CurrentCultureIgnoreCase)));
            }

            if (pageNum > commandsInfoWeNeed.Count || pageNum <= 0)
                pageNum = 1;


            if (commandsInfoWeNeed.Count <= 0)
            {
                helpEmbedBuilder.WithTitle("Command not found");
                return helpEmbedBuilder;
            }

            var commandInformation = commandsInfoWeNeed[pageNum - 1].Module.Summary;


            helpEmbedBuilder.WithDescription(commandInformation);

            if (commandsInfoWeNeed.Count >= 2)
                helpEmbedBuilder.WithTitle($"Variant {pageNum}/{commandsInfoWeNeed.Count}.\n" +
                                           "_______\n");

            return helpEmbedBuilder;
        }

        private static EmbedBuilder GenerateSpecificModuleHelpEmbed(this CommandService commandService,
            ModuleInfo module)
        {
            var helpEmbedBuilder = new EmbedBuilder();
            helpEmbedBuilder.AddField(module.Name, module.Summary);
            return helpEmbedBuilder;
        }

        private static EmbedBuilder GenerateHelpCommandEmbed(this CommandService commandService)
        {
            var helpEmbedBuilder = new EmbedBuilder();
            var commandModules = commandService.GetModulesWithCommands();
            helpEmbedBuilder.WithTitle("How can I help you?");

            foreach (var module in commandModules)
                helpEmbedBuilder.AddField(module.Name, module.Summary);
            return helpEmbedBuilder;
        }
    }
}
