using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfazMensaje
{
    public delegate void MessageDeliveredEventHandler(string message);
    public interface InterfaceMessage
    {
        void SendMessage(string message);

        event MessageDeliveredEventHandler MessageDelivered;
    }

    public class MessageReceiver : MarshalByRefObject
    {
        public MessageDeliveredEventHandler MessageDeliveredHandler;
        public void MessageDelivered(string message)
        {
            if (this.MessageDeliveredHandler!=null)
            {
                this.MessageDeliveredHandler(message);
            }
        }
    }
}
