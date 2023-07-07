﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace McServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ServerControlPanel : Window
    {
        ServerManager.ServerManager sm;
        DiscordManager.DiscordHandler dc;

        public ServerControlPanel()
        {
            InitializeComponent();
            sm = new ServerManager.ServerManager();
            sm.OnOutputReceived += ServerOutput;
            sm.ServerClosed += ServerClosed;
            sm.ServerOpened += ServerOpened;
            sm.OnPlayerJoined += PlayerChange;
            sm.OnPlayerLeft += PlayerChange;
            
            dc = new DiscordManager.DiscordHandler(sm.StartServer, sm.StopServer);
            CheckCommandLineArgs();
        }

        /// <summary>
        /// Check to see if any cli args were provided. This will allow you to autorun the server from a script, for example
        /// </summary>
        private void CheckCommandLineArgs()
        {
            string[] args = Environment.GetCommandLineArgs();

            if (args.Contains("autorun"))
            {
                //Autorun the server
                ConsoleOutputBox.Text = "Autostarting the server!";
                sm.StartServer();
            }
        }

        private void PlayerChange(ServerManager.OutputEventArgs e)
        {
            //Player list has changed
            UpdatePlayerList(sm.players.ToArray());
        }

        private void ServerOpened(object? sender, EventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                HeaderBar.Fill = Brushes.Green;
            });
        }

        private void ServerClosed(object? sender, EventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                HeaderBar.Fill = Brushes.Red;
                PlayerListBox.Items.Clear();
            });
        }

        private void ServerOutput(ServerManager.OutputEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                ConsoleOutputBox.Text = ConsoleOutputBox.Text + "\n" + e.output;
                ConsoleOutputBox.ScrollToEnd();

                if(e.output.Contains("For help, type \"help\""))
                {
                    //Server has started. Send a dc message
                    dc.SendMessage("Server is online!");
                }
            });
        }

        public void UpdatePlayerList(string[] players)
        {
            this.Dispatcher.Invoke(() =>
            {
                Debug.WriteLine($"PLAYERS: {players}");
                PlayerListBox.Items.Clear();
                foreach (string player in players)
                {
                    PlayerListBox.Items.Add(player);
                }
            });
        }

        private void StartServerButton_Click(object sender, RoutedEventArgs e)
        {
            ConsoleOutputBox.Text = "";
            sm.StartServer();
        }

        private void StopServerButton_Click(object sender, RoutedEventArgs e)
        {
            sm.StopServer();
        }

        private void SendCommandButton_Click(object sender, RoutedEventArgs e)
        {
            sm.SendCommand(ConsoleInputBox.Text);
        }

        private void FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            sm.StopServer();
        }
    }
}
