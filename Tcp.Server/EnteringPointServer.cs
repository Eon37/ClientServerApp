using System;
using SomeProject.Library.Server;

namespace SomeProject.TcpServer
{
    class EnteringPointServer
    {
        /// <summary>
        /// Entry point to server
        /// </summary>
        /// <param name="args">Command line arguments</param>
        static void Main(string[] args)
        {
           try
            {
                Server server = new Server(10);
                server.TurnOnListener().Wait();
                
                //server.turnOffListener();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
