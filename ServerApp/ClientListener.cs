using SharedLibrary;
using Newtonsoft.Json;
using System;
using System.IO.Pipes;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ServerApp
{
    internal class ClientListener
    {
        internal static void Start(Action<ApplicationMessage, NamedPipeServerStream> callBack)
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
                            Task.Run(async () => await namedPipeServer.ReadAsync(messageBuffer, 0, messageBuffer.Length));
                            messageChunk = Encoding.UTF8.GetString(messageBuffer);
                            messageBuilder.Append(messageChunk);
                            messageBuffer = new byte[messageBuffer.Length];
                        } while (!namedPipeServer.IsMessageComplete);
                        var applicationMessage = JsonConvert.DeserializeObject<ApplicationMessage>(messageBuilder.ToString());
                        callBack(applicationMessage, namedPipeServer);

                    }
                }
                catch (Exception ex)
                {
                    if (ex.GetType() == typeof(IOException))
                        Console.WriteLine("Client disconnected");
                    else
                        Console.WriteLine("ERROR: {0}", ex.Message);
                    namedPipeServer.Disconnect();
                    //namedPipeServer.Close();
                }
            }
            Start(callBack);
        }

        internal static void ProcesClientMessage(ApplicationMessage applicationMessage, NamedPipeServerStream namedPipeServer)
        {
            if (applicationMessage != null)
            {
                var printMessage = $"Sync Time:{applicationMessage.SyncTime}\n" +
                    $"Sync ID's received:{String.Join<int>(",", applicationMessage.SyncIDs)}";
                Console.WriteLine($"Message from client:\n{printMessage}");
                var serialized = JsonConvert.SerializeObject(printMessage);
                byte[] messageBytes = Encoding.UTF8.GetBytes(serialized);
                if (!namedPipeServer.IsConnected)
                {
                    namedPipeServer.WaitForConnection();
                    Console.WriteLine("Client connected");
                }
                Task.Run(async () =>
                {
                    await namedPipeServer.WriteAsync(messageBytes, 0, messageBytes.Length);
                    await namedPipeServer.FlushAsync();
                }
                ).Wait();
            }
        }
    }
}