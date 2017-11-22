using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using Apache.NMS;
using Apache.NMS.Util;

namespace QueueB
{
    public class Class1
    {

        private DateTime inititalTime = DateTime.Now;
        private string hour = DateTime.Now.ToString();
        //private webReq theWebRequest = new webReq();
        //private IContainer components = (IContainer)null;
        private string query;
        private string query2;
        private string attrValue;
        //private line lineClass;
        private int numberMessages;
        //private Label label1;
        //private Label label2;

        static void Main(string[] args)
        {
            Class1 p = new Class1();

            p.receiver();
        }
        private void receiver()
        {
            string topic = "com.donbest.message.public.xmleddie";
            string userName;
            string password;
            string brokerUri;

            topic = "com.donbest.message.public.xmleddie";
            userName = "xmleddie";
            password = "xmlfootball";
            brokerUri = "tcp://amq.donbest.com:61616";


            IConnectionFactory factory = new NMSConnectionFactory(brokerUri);
            IConnection connection = factory.CreateConnection(userName, password);
            connection.Start();
            ISession session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge);
            IDestination destination = session.GetTopic(topic);
            IMessageConsumer receiver = session.CreateConsumer(destination);
            receiver.Listener += new MessageListener(Message_Listener);

        }

        private void Message_Listener(IMessage message)
        {


            ITextMessage txtMsg = message as ITextMessage;
            string body = txtMsg.Text;


            //IObjectMessage objMessage = message as IObjectMessage;
            //OperatorRequestObject OperatorRequestObject = ((BusinessObjects.OperatorRequestObject)(objMessage.Body));

            Console.WriteLine(body);
            //this.numberMessages = this.processTxtMessage(this.numberMessages, (IMessage)textMessage);
        }
    }


}
