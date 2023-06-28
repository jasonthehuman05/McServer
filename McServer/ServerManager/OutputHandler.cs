using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McServer.ServerManager
{
    public delegate void OutputEventHandler(OutputEventArgs e);
    

    /// <summary>
    /// Event Arguments for any outputs
    /// </summary>
    public class OutputEventArgs : EventArgs
    {
        public string output = string.Empty;
        public OutputEventArgs(string output) : base()
        {
            this.output = output;
        }
    }
}
