using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbionOnline.Services.Discord.Entities
{
    public class CommandServiceInfo
    {
        public CommandServiceInfo(CommandInfo commandInformation, ModuleInfo moduleInformation,
            string commandParameters, string commandAliases)
        {
            CommandInformation = commandInformation;
            ModuleInformation = moduleInformation;
            CommandParameters = commandParameters;
            CommandAliases = commandAliases;
        }

        public CommandServiceInfo()
        {
        }

        public CommandInfo CommandInformation { get; set; }
        public ModuleInfo ModuleInformation { get; set; }

        /// <summary>
        ///     Every command parameter, separated by comma
        ///     Optional values will be enclosed with <>,
        ///     while mandatory values, with []
        /// </summary>
        public string CommandParameters { get; set; }

        /// <summary>
        ///     A string made with every command alias,
        ///     separated by comma
        /// </summary>
        public string CommandAliases { get; set; }
    }
}
