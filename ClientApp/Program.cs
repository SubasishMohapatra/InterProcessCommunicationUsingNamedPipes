using SharedLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Text;

namespace ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            using (NamedPipeClientStream namedPipeClient = new NamedPipeClientStream("CGSCLRD48139536", "ClientServerPipe"))
            {
                try
                {
                    Console.WriteLine("Enter any key(escape to exit)");
                    var number = 0;
                    while (Console.ReadKey().Key != ConsoleKey.Escape)
                    {
                        var applicationMessage = new ApplicationMessage()
                        {
                            SyncTime = DateTime.Now,
                            SyncIDs = new List<int>() { ++number, ++number, ++number }
                        };
                        string serialised = JsonConvert.SerializeObject(applicationMessage);
                        if (!namedPipeClient.IsConnected)
                            namedPipeClient.Connect();
                        byte[] messageBytes = Encoding.UTF8.GetBytes(serialised);
                        namedPipeClient.Write(messageBytes, 0, messageBytes.Length);
                        Console.WriteLine("Enter any key(escape to exit)");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Eror{ex.Message}");
                }
            }
        }
    }
}
