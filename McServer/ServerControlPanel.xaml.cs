using System;
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

namespace McServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ServerControlPanel : Window
    {
        ServerManager.ServerManager sm;

        public ServerControlPanel()
        {
            InitializeComponent();
            sm = new ServerManager.ServerManager();
            sm.OnOutputReceived += ServerOutput;
        }

        private void ServerOutput(ServerManager.OutputEventArgs e)
        {
            ConsoleOutputBox.Text = ConsoleOutputBox.Text + "\n" + e.output;
        }

        public void UpdatePlayerList(string[] players)
        {
            PlayerListBox.Items.Clear();
            foreach(string player in players)
            {
                PlayerListBox.Items.Add(players);
            }
        }
    }
}
