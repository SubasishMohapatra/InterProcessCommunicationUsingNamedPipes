using SharedLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Principal;

namespace ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            using (NamedPipeClientStream namedPipeClientStream = new NamedPipeClientStream(".", "ClientServerPipe",
                                PipeDirection.InOut, PipeOptions.None,
                                TokenImpersonationLevel.Impersonation))
            {
                SendMessage(namedPipeClientStream, "Hello 1");
                Console.WriteLine($"Received from server {GetMessage(namedPipeClientStream)}");
                SendMessage(namedPipeClientStream, "Hello 2");
                Console.WriteLine($"Received from server {GetMessage(namedPipeClientStream)}");
                SendMessage(namedPipeClientStream, "Hello 3");
                Console.WriteLine($"Received from server {GetMessage(namedPipeClientStream)}");
            }
            Console.ReadLine();
        }

        static void SendMessage(NamedPipeClientStream namedPipeClient, string message)
        {
            if (!namedPipeClient.IsConnected)
            {                
                namedPipeClient.Connect();
                namedPipeClient.ReadMode = PipeTransmissionMode.Message;
            }
            string serialised = JsonConvert.SerializeObject(message);
            byte[] messageBytes = Encoding.UTF8.GetBytes(serialised);
            namedPipeClient.Write(messageBytes, 0, messageBytes.Length);
            namedPipeClient.Flush();
        }

        static string GetMessage(NamedPipeClientStream namedPipeClient)
        {
            if (!namedPipeClient.IsConnected)
            {
                namedPipeClient.Connect();
            }
            StringBuilder messageBuilder = new StringBuilder();
            string messageChunk = string.Empty;
            byte[] messageBuffer = new byte[1024];
            do
            {
                namedPipeClient.Read(messageBuffer, 0, messageBuffer.Length);
                messageChunk = Encoding.UTF8.GetString(messageBuffer);
                messageBuilder.Append(messageChunk);
                messageBuffer = new byte[messageBuffer.Length];
            } while (!namedPipeClient.IsMessageComplete);
            return JsonConvert.DeserializeObject<String>(messageBuilder.ToString());
        }

        #region Commented code

        //        using (NamedPipeClientStream namedPipeClientStream = new NamedPipeClientStream(".", "ClientServerPipe",
        //                        PipeDirection.InOut, PipeOptions.None,
        //                        TokenImpersonationLevel.Impersonation))
        //            {
        //                try
        //                {

        //                    ThreadPool.QueueUserWorkItem((x) => ServerListener.Start(x), namedPipeClientStream);
        //                    Console.WriteLine("Enter any key(escape to exit)");
        //                    var number = 0;
        //                    while (Console.ReadKey().Key != ConsoleKey.Escape)
        //                    {
        //                        var applicationMessage = new ApplicationMessage()
        //                        {
        //                            SyncTime = DateTime.Now,
        //                            SyncIDs = new List<int>() { ++number, ++number, ++number }
        //                        };
        //    string serialised = JsonConvert.SerializeObject(applicationMessage);
        //                        if (!namedPipeClientStream.IsConnected)
        //                        {
        //                            namedPipeClientStream.ReadMode = PipeTransmissionMode.Message;
        //                            namedPipeClientStream.Connect();
        //                        }
        //byte[] messageBytes = Encoding.UTF8.GetBytes(serialised);
        //Task.Run(async () =>
        //                        {
        //                            await namedPipeClientStream.WriteAsync(messageBytes, 0, messageBytes.Length);
        //await namedPipeClientStream.FlushAsync();
        //                        }).Wait();
        //Console.WriteLine("Enter any key(escape to exit)");
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    Console.WriteLine($"Eror{ex.Message}");
        //                }
        //            }

        #endregion
    }
}
