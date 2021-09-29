using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using InterfazMensaje;
using Belikov.GenuineChannels;
using Belikov.GenuineChannels.BroadcastEngine;
using Belikov.GenuineChannels.DotNetRemotingLayer;
using System.Runtime.Remoting;

namespace Cliente
{
    class Cliente : MarshalByRefObject
    {
        public static Cliente Instance = new Cliente();
        public static MessageReceiver messageReceiver = new MessageReceiver();

        static void Main(string[] args)
        {
            Console.WriteLine("Sleep for 3 seconds.");
            Thread.Sleep(TimeSpan.FromSeconds(3));


            Console.WriteLine("Configuration Remoting environment....");
            System.Configuration.ConfigurationSettings.GetConfig("DNS");
            GlobalEventContainer.GenuineChannelsGlobalEvent += new GenuineChannelsGlobalEventHandler(GenuineChannelsEventHandler);
            RemotingConfiguration.Configure("Cliente.exe.config");

            messageReceiver.MessageDeliveredHandler = new MessageDeliveredEventHandler(Instance.ReceiveMessage);
            Console.WriteLine(".NET Remoting has been configured from Client.exe.config file.");
            for (;;)
            {
                try
                {
                    InterfaceMessage iChatRoom = (InterfaceMessage)Activator.GetObject(typeof(InterfaceMessage), "gtcp://127.0.0.1:8737/ChatRoom.rem");
                    iChatRoom.MessageDelivered += new MessageDeliveredEventHandler(messageReceiver.MessageDelivered);

                    for (;;)
                    {
                        Console.WriteLine("Enter a message to send or an empty string to exit.");

                        string str = Console.ReadLine();
                        if (str.Length <= 0)
                            return;

                        iChatRoom.SendMessage(str);
                        Console.WriteLine("Message \"{0}\" has been sent.", str);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: {0}. Stack trace: {1}.", ex.Message, ex.StackTrace);
                }

                Console.WriteLine("Next attempt to connect to the server will be in 3 seconds.");
                Thread.Sleep(3000);




            }

        }

        public static void GenuineChannelsEventHandler(object sender, GlobalEventArgs e)
        {
            Console.WriteLine("Global event: {0}, Url: {1}, Exception: {2}", e.EventType, e.Url, e.SourceException);
            if (e.EventType == GlobalEventTypes.GTcpConnectionClosed &&
                e.SourceException is OperationException &&
                ((OperationException)e.SourceException).OperationErrorMessage.ErrorIdentifier.IndexOf("ServerHasBeenRestarted") > -1)
            {
                // server has been restarted so we have to register our listener again
                InterfaceMessage iChatRoom = (InterfaceMessage)Activator.GetObject(typeof(InterfaceMessage), "gtcp://127.0.0.1:8737/ChatRoom.rem");
                iChatRoom.MessageDelivered += new MessageDeliveredEventHandler(messageReceiver.MessageDelivered);
            }
        }

        public void ReceiveMessage(string message)
        {
            Console.WriteLine("Message \"{0}\" has been received from the server.", message);
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

    }
}
