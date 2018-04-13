using SharedLibrary;
using Newtonsoft.Json;
using System;
using System.IO.Pipes;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.IO;
using System.Threading;

namespace ServerApp
{
    internal class ClientListener
    {
        internal static void Start()
        {
            using (var namedPipeServer =
         new NamedPipeServerStream("ClientServerPipe", PipeDirection.InOut, 1, PipeTransmissionMode.Message))
            {
                try
                {
                    while (1 == 1)
                    {
                        if (!namedPipeServer.IsConnected)
                        {
                            namedPipeServer.WaitForConnection();
                            Console.WriteLine("Client connected");
                        }
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
                        var applicationMessage = JsonConvert.DeserializeObject<ApplicationMessage>(messageBuilder.ToString());
                        if (applicationMessage != null)
                            Console.WriteLine($"Sync Time:{applicationMessage.SyncTime}\n" +
                                $"Sync ID's received:{String.Join<int>(",", applicationMessage.SyncIDs)}");
                    }
                }
                catch (Exception ex)
                {
                    if (ex.GetType() == typeof(IOException))
                        Console.WriteLine("Client disconnected");
                    else
                        Console.WriteLine("ERROR: {0}", ex.Message);
                    namedPipeServer.Disconnect();
                }
            }
            Start();
        }
    }
}