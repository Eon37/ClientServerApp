using System;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SomeProject.Library.Client
{
    public class Client
    {
        /// <summary>
        /// TCP Client
        /// </summary>
        public TcpClient tcpClient;

        /// <summary>
        /// Recieves message from server
        /// </summary>
        /// <returns>The result of operation</returns>
        public OperationResult ReceiveMessageFromServer()
        {
            try
            {
                tcpClient = new TcpClient("127.0.0.1", 8080);
                StringBuilder recievedMessage = new StringBuilder();
                byte[] data = new byte[256];
                NetworkStream stream = tcpClient.GetStream();

                do
                {
                    int bytes = stream.Read(data, 0, data.Length);
                    recievedMessage.Append(Encoding.UTF8.GetString(data, 0, bytes));
                }
                while (stream.DataAvailable);
                stream.Close();
                tcpClient.Close();

                return new OperationResult(Result.OK, recievedMessage.ToString());
            }
            catch (Exception e)
            {
                return new OperationResult(Result.Fail, e.ToString());
            }
        }

        /// <summary>
        /// Sends message to server
        /// </summary>
        /// <param name="message">Contents of message</param>
        /// <returns>The result of operation</returns>
        public OperationResult SendMessageToServer(string message)
        {
            try
            {
                tcpClient = new TcpClient("127.0.0.1", 8080);
                NetworkStream stream = tcpClient.GetStream();
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, true);
                
                byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
                stream.Write(data, 0, data.Length);
                stream.Close();
                tcpClient.Close();
                return new OperationResult(Result.OK, "Message sent successfuly.") ;
            }
            catch (Exception e)
            {
                return new OperationResult(Result.Fail, e.Message);
            }
        }

        /// <summary>
        /// Sends file to server
        /// </summary>
        /// <param name="filePath">Path to a file to send</param>
        /// <returns>The result of operation</returns>
        public OperationResult SendFileToServer(string filePath)
        {
            try
            {
                tcpClient = new TcpClient("127.0.0.1", 8080);
                NetworkStream stream = tcpClient.GetStream();
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, false);
                byte[] data = File.ReadAllBytes(filePath);

                formatter.Serialize(stream, filePath.Substring(filePath.IndexOf('.') + 1));
                formatter.Serialize(stream, data);
                
                stream.Close();
                tcpClient.Close();
                return new OperationResult(Result.OK, "File " + Path.GetFileName(filePath) + " sent successfully.");
            }
            catch (System.Runtime.Serialization.SerializationException e)
            {
                return new OperationResult(Result.Fail, e.Message);
            }
            catch (Exception e)
            {
                return new OperationResult(Result.Fail, e.Message);
            }
        }
    }
}
