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
        public List<string> players { get; set; }
        private string runCommand = String.Empty;
        private Process serverProcess;
        private bool serverRunning = false;
        private string serverPath = String.Empty;
        private StreamWriter ServerInput;

        public ServerManager()
        {
            //Initialize the server manager
            runCommand = LoadServerRunFile();

            //Get the server path
            string p = Directory.GetCurrentDirectory();
            serverPath = $"{p}/ServerManager";
        }

        /// <summary>
        /// Loads the file containing the command to start the server
        /// </summary>
        private string LoadServerRunFile()
        {
            return File.ReadAllText("runCommand.txt");
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
                WorkingDirectory = serverPath
            };
            if((serverProcess = Process.Start(startInfo)) != null) //If successfully started process
            {
                serverProcess.EnableRaisingEvents = true;
                serverProcess.Exited += ServerProcessTerminated;
                ServerInput = serverProcess.StandardInput;

                serverProcess.OutputDataReceived += NewServerDataReceived;
            }
        }

        private void NewServerDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                //A string with content received, send the event

            }
        }

        private void ServerProcessTerminated(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
