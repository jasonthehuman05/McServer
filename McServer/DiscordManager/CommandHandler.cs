using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McServer.DiscordManager
{
    internal class CommandHandler
    {
        public async Task<SlashCommandProperties[]> LoadCommands()
        {
            SlashCommandBuilder startCommandBuilder = new SlashCommandBuilder();
            startCommandBuilder.WithName("mcstart");
            startCommandBuilder.WithDescription("Start the server");

            //Stop Command
            SlashCommandBuilder stopCommandBuilder = new SlashCommandBuilder();
            stopCommandBuilder.WithName("mcstart");
            stopCommandBuilder.WithDescription("Start the server");

            //Build commands and whack in an array to return them
            SlashCommandProperties[] scp = {
                startCommandBuilder.Build(),
                stopCommandBuilder.Build() 
            };

            return scp;
        }
    }
}
