using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfazMensaje;
using System.Runtime.Remoting;
using Belikov.GenuineChannels.BroadcastEngine;
using Belikov.GenuineChannels.DotNetRemotingLayer;



namespace Server
{
    class Server : MarshalByRefObject, InterfaceMessage
    {
        static void Main(string[] args)
        {
            try
            {
                System.Configuration.ConfigurationSettings.GetConfig("DNS");

                GlobalEventContainer.GenuineChannelsGlobalEvent += new GenuineChannelsGlobalEventHandler(GenuineChannelsEventHandler);
                RemotingConfiguration.Configure("Server.exe.config");

                RemotingServices.Marshal(new Server(), "ChatRoom.rem");

                Console.WriteLine("Server has been started. Press ENTER to exit.");
                Console.ReadLine();


            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}. Stack trace: {1}.", ex.Message, ex.StackTrace);
            }
        }

        public event MessageDeliveredEventHandler MessageDelivered;

        public void SendMessage(string message)
        {
            Console.WriteLine("\"{0}\" message will be sent to all clients.", message);
            if (this.MessageDelivered!= null)
            {
                this.MessageDelivered(message);
            }

        }

        public static void GenuineChannelsEventHandler(object sender, GlobalEventArgs e)
        {
            Console.WriteLine("Global event: {0}, Url: {1}, Exception: {2}", e.EventType, e.Url, e.SourceException);
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
