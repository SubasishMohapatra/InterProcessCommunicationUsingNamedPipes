using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp
{
    class Program
    {
        static void Main(string[] args)
        {

            using (var namedPipeServer =
      new NamedPipeServerStream("ClientServerPipe", PipeDirection.InOut, 1, PipeTransmissionMode.Message))
            {
                if (!namedPipeServer.IsConnected)
                {
                    namedPipeServer.WaitForConnection();
                    Console.WriteLine("Client connected");
                }
                var message = GetMessage(namedPipeServer);
                Console.WriteLine($"From client: {message}");
                SendMessage(namedPipeServer, $"Received {message}");
                message = GetMessage(namedPipeServer);
                Console.WriteLine($"From client: {message}");
                SendMessage(namedPipeServer, $"Received {message}");
                message = GetMessage(namedPipeServer);
                Console.WriteLine($"From client: {message}");
                SendMessage(namedPipeServer, $"Received {message}");
            }
            Console.ReadLine();
        }

        static void SendMessage(NamedPipeServerStream namedPipeServer, string message)
        {
            var serialized = JsonConvert.SerializeObject(message);
            byte[] messageBytes = Encoding.UTF8.GetBytes(serialized);
            if (!namedPipeServer.IsConnected)
            {
                namedPipeServer.WaitForConnection();
                Console.WriteLine("Client connected");
            }
            namedPipeServer.Write(messageBytes, 0, messageBytes.Length);
            namedPipeServer.Flush();
        }

        static string GetMessage(NamedPipeServerStream namedPipeServer)
        {
            StringBuilder messageBuilder = new StringBuilder();
            string messageChunk = string.Empty;
            byte[] messageBuffer = new byte[1024];
            do
            {
                namedPipeServer.Read(messageBuffer, 0, messageBuffer.Length);
                messageChunk = Encoding.UTF8.GetString(messageBuffer);
                messageBuilder.Append(messageChunk);
                messageBuffer = new byte[messageBuffer.Length];
            } while (!namedPipeServer.IsMessageComplete);
            return JsonConvert.DeserializeObject<string>(messageBuilder.ToString());
        }
    }
}
