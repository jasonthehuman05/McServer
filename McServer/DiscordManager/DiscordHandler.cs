using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace McServer.DiscordManager
{
    internal class DiscordHandler
    {
        static DiscordSocketClient client;
        static CommandHandler ch;
        static string token;

        public DiscordHandler()
        {
            MainAsyncProcess(); //Run the main process that handles running the bot
            token = System.IO.File.ReadAllText("DiscordManager/discord.token"); //Load the discord bot token from the file
        }

        static async void MainAsyncProcess()
        {
            //Create the discord client and wire up all needed events
            client = new DiscordSocketClient();
            client.Log += DiscordLogger;
            client.Ready += BotReady;
            //Create event to process command
            client.SlashCommandExecuted += commandHandler.CommandExecuted;

            //Attempt first log in
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            //Do nothing until program is closed
            await Task.Delay(-1);
            await client.StopAsync();
            client.Dispose();
        }

        private static async Task BotReady()
        {
            //Load Commands
            ch = new CommandHandler();
            SlashCommandProperties[] commands = await ch.LoadCommands();
            await client.CreateGlobalApplicationCommandAsync(commands[0]);
            await client.CreateGlobalApplicationCommandAsync(commands[1]);
        }

        private static Task DiscordLogger(LogMessage arg)
        {
            Debug.WriteLine(arg.Message);
            return Task.CompletedTask;
        }
    }
}
