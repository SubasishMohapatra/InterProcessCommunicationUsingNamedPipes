using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            StartClient();
        }

        private static void StartClient()
        {
            //Process[] processes = Process.GetProcessesByName("ClientApp");
            //if (processes != null && processes.Count() <= 0)
            //{
            //    var autoSync = Process.Start("ClientApp.exe");
                ClientListener.Start();
            //}
        }
    }
}
