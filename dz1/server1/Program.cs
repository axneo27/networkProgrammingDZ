using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            string ipAddress = "192.168.31.121";
            int port = 5555;

            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            EndPoint endPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);

            server.Bind(endPoint);
            server.Listen(10);

            Console.WriteLine($"Server started {ipAddress}:{port}");
            Console.WriteLine("Listeting...");

            while (true)
            {

                try
                {
                    Socket client = server.Accept();
                    Console.WriteLine($"Client connected {client.RemoteEndPoint?.ToString()}");

                    try
                    {
                        while (client.Connected)
                        {
                            // receive 
                            byte[] buffer = new byte[1024];
                            int l = client.Receive(buffer);
                            string clientMessage = Encoding.UTF8.GetString(buffer, 0, l);
                            Console.WriteLine($"О {DateTime.Now.ToString("HH:mm")} від {client.RemoteEndPoint} отримано рядок: {clientMessage}");

                            // send 
                            if (string.Equals(clientMessage.ToLower(), "date"))
                            {
                                string date = DateTime.Now.ToString("dd.MM.yyyy");
                                byte[] dateBytes = Encoding.UTF8.GetBytes(date);
                                client.Send(dateBytes);
                                Console.WriteLine("Sent date to client");

                                client.Shutdown(SocketShutdown.Both);
                                client.Disconnect(false);
                                client.Close();
                                Console.WriteLine("Client disconnected");
                                return;
                            }
                            else if (string.Equals(clientMessage.ToLower(), "time"))
                            {
                                string time = DateTime.Now.ToString("HH:mm");
                                byte[] timeBytes = Encoding.UTF8.GetBytes(time);
                                client.Send(timeBytes);
                                Console.WriteLine("Sent date to client");

                                client.Shutdown(SocketShutdown.Both);
                                client.Disconnect(false);
                                client.Close();
                                Console.WriteLine("Client disconnected");
                                return;
                            }
                            else
                            {
                                Console.Write("Enter response: ");
                                string? response = Console.ReadLine() ?? "Empty response";

                                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                                client.Send(responseBytes);
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}