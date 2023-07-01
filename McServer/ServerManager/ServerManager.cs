using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace McServer.ServerManager
{
    internal class ServerManager
    {
        // Minecraft Automatic Data Delivery Initialization Equipment
        public List<string> players { get; set; } = new List<string>();
        public event OutputEventHandler OnOutputReceived;
        public event OutputEventHandler OnPlayerJoined;
        public event OutputEventHandler OnPlayerLeft;
        public event EventHandler ServerClosed;
        public event EventHandler ServerOpened;
        private string runCommand = String.Empty;
        private Process serverProcess;
        private bool serverRunning = false;
        private string serverPath = String.Empty;
        private StreamWriter ServerInput;

        public ServerManager()
        {
            OnOutputReceived = new OutputEventHandler(OutputReceivedMethod);
            ServerClosed = new EventHandler(ServerClosedMethod);
            ServerOpened = new EventHandler(ServerOpenedMethod);

            //Get the server path
            string p = Directory.GetCurrentDirectory();
            serverPath = $"{p}/ServerManager";
            
            //Initialize the server manager
            runCommand = LoadServerRunFile();
        }

        /// <summary>
        /// Fires when new output has been created, thus triggering the relevant event
        /// </summary>
        /// <param name="e">Arguments for the event</param>
        private void OutputReceivedMethod(OutputEventArgs e) { NewOutputProcessing(e.output); }
        /// <summary>
        /// Fires when the server is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServerClosedMethod(object sender, EventArgs e) { }
        /// <summary>
        /// Fires when the server is opened
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServerOpenedMethod(object sender, EventArgs e) { }

        /// <summary>
        /// Loads the file containing the command to start the server
        /// </summary>
        private string LoadServerRunFile()
        {
            return File.ReadAllText($"{serverPath}/runCommand.txt");
        }

        /// <summary>
        /// Start the server. Continue if it is already running
        /// </summary>
        public void StartServer()
        {
            if (!serverRunning)
            {
                //Server is not running, start it.
                RunServerProcess();
            }
        }

        /// <summary>
        /// Stops the server. Nothing happens if the server is not already running.
        /// </summary>
        public void StopServer()
        {
            if (serverRunning)
            {
                //server is running. Kill the thing
                SendCommand("stop"); //Send stop command
                players.Clear();
                serverRunning = false;
            }
        }

        public void NewOutputProcessing(string output)
        {
            Debug.WriteLine(output);
            //Check to see if the string is a joined the game message
            if (output.Contains("joined the game"))
            {
                //it is a joined the game message. Get the username
                //remove the end
                output.Replace(" joined the game", string.Empty);
                int usernameFirstCharIndex = -1;
                for (int i = output.Length-1; i > 0; i--)//Go backwards until we find a space. then we have the index of the start of the name!
                {
                    if (output[i] == ' ')
                    {
                        //found index
                        usernameFirstCharIndex = i;
                        break;
                    }
                }
                string username = output.Substring(usernameFirstCharIndex); //extract the username
                Debug.WriteLine(username);
                PlayerJoined(username);
            }


            //Check to see if the string is a left the game message
            if (output.Contains("left the game"))
            {
                //it is a left the game message. Get the username
                //remove the end
                output.Replace(" left the game", string.Empty);
                int usernameFirstCharIndex = -1;
                for (int i = output.Length-1; i > 0; i--)//Go backwards until we find a space. then we have the index of the start of the name!
                {
                    if (output[i] == ' ')
                    {
                        //found index
                        usernameFirstCharIndex = i;
                        break;
                    }
                }
                string username = output.Substring(usernameFirstCharIndex); //extract the username
                Debug.WriteLine(username);
                PlayerLeft(username);
            }
        }

        /// <summary>
        /// Send a command to the Minecraft server
        /// </summary>
        /// <param name="command">The command to send</param>
        public void SendCommand(string command)
        {
            Debug.WriteLine(command);
            Debug.WriteLine(serverRunning);
            if (!serverRunning) { return; } //Do not continue if the server is not running

            ServerInput.WriteLine(command); //Write line to input stream
        }

        /// <summary>
        /// Launches the server process
        /// </summary>
        public void RunServerProcess()
        {
            //Build process with java command and process information
            ProcessStartInfo startInfo = new ProcessStartInfo("java", runCommand)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = serverPath + "/server"
            };
            if((serverProcess = Process.Start(startInfo)) != null) //If successfully started process
            {
                ServerOpened.Invoke(null, null);
                //Set up events
                serverProcess.EnableRaisingEvents = true;
                serverProcess.Exited += ServerProcessTerminated;
                ServerInput = serverProcess.StandardInput;

                serverProcess.OutputDataReceived += NewServerDataReceived;
                serverProcess.ErrorDataReceived += NewServerDataReceived;
                serverProcess.BeginOutputReadLine();
                serverProcess.BeginErrorReadLine();
                serverRunning = true;
            }
        }

        #region events
        private void NewServerDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                //A string with content received, send the event
                OnOutputReceived.Invoke(new OutputEventArgs(e.Data));
            }
        }

        private void ServerProcessTerminated(object? sender, EventArgs e)
        {
            //Server Has Closed. Handle it
            serverRunning = false;
            ServerClosed.Invoke(null, null);
        }

        private void PlayerJoined(string username)
        {
            players.Add(username);
            OnPlayerJoined.Invoke(new OutputEventArgs(username));
        }
        
        private void PlayerLeft(string username)
        {
            players.Remove(username);
            OnPlayerLeft.Invoke(new OutputEventArgs(username));
        }
        #endregion
    }
}
