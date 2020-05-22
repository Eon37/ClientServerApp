using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SomeProject.Library.Server
{
    public class Server
    {
        /// <summary>
        /// TCP Listener
        /// </summary>
        TcpListener serverListener;

        /// <summary>
        /// Maximum number of posible connections
        /// </summary>
        int maxConnections;

        /// <summary>
        /// Current number of connections
        /// </summary>
        int curConnections=0;

        /// <summary>
        /// Number of files recieved during server's session
        /// </summary>
        int filesCount=0;

        /// <summary>
        /// Path where recieved files are saved
        /// </summary>
        const string filesPath = "D:\\PoliProgi\\c#\\Cl_ServApp\\Клиент-серверное приложение\\ПРИМЕР\\Tcp.Server\\RecievedFiles\\";

        /// <summary>
        /// Parametrized constructor
        /// </summary>
        /// <remarks>Uses port 8080</remarks>
        /// <param name="maxConnections">Sets maximum number of possible connections</param>
        public Server(int maxConnections)
        {
            this.maxConnections = maxConnections;
            serverListener = new TcpListener(IPAddress.Loopback, 8080);            
        }

        /// <summary>
        /// Turns off the TCP Listener
        /// </summary>
        /// <returns>Determines whether the Listener was turned off successfully</returns>
        public bool TurnOffListener()
        {
            try
            {
                if (serverListener != null)
                    serverListener.Stop();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot turn off listener: " + e.Message);
                return false;
            }
        }

        /// <summary>
        /// Turns on the TCP Listener
        /// </summary>
        /// <returns>Determines whether the Listener was turned on successfully</returns>
        public async Task TurnOnListener()
        {
            try
            {
                if (serverListener != null)
                    serverListener.Start();
                while (true)
                {
                    Console.WriteLine("Waiting for connections...");

                    OperationResult result = await ChooseOption();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot turn on listener: " + e.Message);
            }
        }

        /// <summary>
        /// Chooses what operation to process
        /// </summary>
        /// <remarks>Operations: RecieveMessageFromClient; RecieveFileFromClient</remarks>        
        /// <returns>The result of operation</returns>
        private async Task<OperationResult> ChooseOption()
        {
            try
            {
                if (curConnections > maxConnections) return new OperationResult(Result.Fail, "Server busy. Try connect Later");
                TcpClient client = serverListener.AcceptTcpClient();
                Interlocked.Increment(ref curConnections);
                NetworkStream stream = client.GetStream();

                BinaryFormatter formatter = new BinaryFormatter();
                bool op = (bool)formatter.Deserialize(stream);

                if (op)
                {
                    OperationResult result = await ReceiveMessageFromClient(stream);
                    if (result.Result == Result.Fail)
                        Console.WriteLine("Unexpected error: " + result.Message);
                    else
                        Console.WriteLine("New message from client: " + result.Message);
                }
                else
                {
                    OperationResult result = await ReceiveFileFromClient(stream);
                    if (result.Result == Result.Fail)
                        Console.WriteLine("Unexpected error: " + result.Message);
                    else
                        Console.WriteLine("New file recieved from client: " + result.Message);
                }

                stream.Close();
                client.Close();
                Interlocked.Decrement(ref curConnections);

                return new OperationResult(Result.OK, "");
            }
            catch (Exception e)
            {
                return new OperationResult(Result.Fail, e.Message);
            }
        }

        /// <summary>
        /// Recieves message from client
        /// </summary>
        /// <param name="stream">Stream from which message is recieved</param>
        /// <returns>The result of operation</returns>
        public async Task<OperationResult> ReceiveMessageFromClient(NetworkStream stream)
        {
            try
            {                
                StringBuilder recievedMessage = new StringBuilder();
                byte[] data = new byte[256];

                do
                {
                    int bytes = stream.Read(data, 0, data.Length);
                    recievedMessage.Append(Encoding.UTF8.GetString(data, 0, bytes));
                }
                while (stream.DataAvailable);
                stream.Close();

                return new OperationResult(Result.OK, recievedMessage.ToString());
            }
            catch (Exception e)
            {
                return new OperationResult(Result.Fail, e.Message);
            }
        }

        /// <summary>
        /// Sends message to client
        /// </summary>
        /// <param name="message">Contents of message</param>
        /// <returns>The result of operation</returns>
        public OperationResult SendMessageToClient(string message)
        {
            try
            {
                if (curConnections > maxConnections) return new OperationResult(Result.Fail, "Server busy. Try connect Later");
                TcpClient client = serverListener.AcceptTcpClient();
                Interlocked.Increment(ref curConnections);
                NetworkStream stream = client.GetStream();

                byte[] data = Encoding.UTF8.GetBytes(message);
                stream.Write(data, 0, data.Length);

                stream.Close();
                client.Close();
                Interlocked.Decrement(ref curConnections);
            }
            catch (Exception e)
            {
                return new OperationResult(Result.Fail, e.Message);
            }
            return new OperationResult(Result.OK, "");
        }

        /// <summary>
        /// Recieves file from client
        /// </summary>
        /// <param name="stream">Stream from which file is recieved</param>
        /// <returns>The result of operation</returns>
        public async Task<OperationResult> ReceiveFileFromClient(NetworkStream stream)
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                string extension = (string) formatter.Deserialize(stream);
                byte[] data = (byte[]) formatter.Deserialize(stream);

                string newFile = newFileName(extension);
                File.WriteAllBytes(newFile, data);
                Interlocked.Increment(ref filesCount);

                stream.Close();

                return new OperationResult(Result.OK, Path.GetFileName(newFile));
            }
            catch (Exception e)
            {
                return new OperationResult(Result.Fail, e.Message);
            }
        }

        /// <summary>
        /// Creates new file name to save on server
        /// </summary>
        /// <param name="extension">file extension</param>
        /// <returns>File name</returns>
        private string newFileName(string extension)
        {
            string path = filesPath + DateTime.Today.Year.ToString() + "-" 
                            + DateTime.Today.Month.ToString() + "-" 
                            + DateTime.Today.Day.ToString();

            if(!Directory.Exists(path)) Directory.CreateDirectory(path);
            return path + "\\File" + filesCount + "." + extension;
        }
    }
}