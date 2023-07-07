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
        static Action startFunction;
        static Action stopFunction;

        public DiscordHandler(Action _startFunction, Action _stopFunction)
        {
            //Get start and stop functions
            startFunction = _startFunction;
            stopFunction = _stopFunction;

            token = System.IO.File.ReadAllText("DiscordManager/discord.token"); //Load the discord bot token from the file

            MainAsyncProcess(); //Run the main process that handles running the bot
        }

        static async void MainAsyncProcess()
        {
            //Create the discord client and wire up all needed events
            client = new DiscordSocketClient();
            client.Log += DiscordLogger;
            client.Ready += BotReady;
            //Create event to process command
            client.SlashCommandExecuted += CommandExecuted; ;

            //Attempt first log in
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            //Do nothing until program is closed
            await Task.Delay(-1);
            await client.StopAsync();
            client.Dispose();
        }

        private static Task CommandExecuted(SocketSlashCommand arg)
        {
            if(arg.CommandName == "mcstart") //Server start command
            {
                startFunction();
                arg.RespondAsync("Starting, Please Wait.");
            }
            else//Server stop command
            {
                stopFunction();
                arg.RespondAsync("Stopping, Please Wait.");
            }

            return Task.CompletedTask;
        }

        public void SendMessage(string message)
        {
            IMessageChannel channel = (IMessageChannel)client.GetChannel(910475029542764574);
            channel.SendMessageAsync(message);
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
