using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apache.NMS;
using Apache.NMS.Util;
using BusinessObjects;


namespace Queue
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            receiver();
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
           

            IConnectionFactory factory = new NMSConnectionFactory("tcp://localhost:61616");
            IConnection connection = factory.CreateConnection(userName,password);
            connection.Start();
            ISession session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge);
            IDestination destination = SessionUtil.GetDestination(session, "ExampleQueue");
            IMessageConsumer receiver = session.CreateConsumer(destination);
            receiver.Listener += new MessageListener(Message_Listener);

        }

        private void Message_Listener(IMessage message)
        {
            IObjectMessage objMessage = message as IObjectMessage;
            OperatorRequestObject OperatorRequestObject = ((BusinessObjects.OperatorRequestObject)(objMessage.Body));
            MessageBox.Show(OperatorRequestObject.Shortcode);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
