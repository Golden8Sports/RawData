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
using DataLayer;


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
           

            IConnectionFactory factory = new NMSConnectionFactory(brokerUri);
            IConnection connection = factory.CreateConnection(userName, password);
            connection.Start();
            ISession session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge);
            IDestination destination = session.GetTopic(topic);
            IMessageConsumer receiver = session.CreateConsumer(destination);
            receiver.Listener += new MessageListener(Message_Listener);

        }

        public DataTable ExeSPWithResults(string storedProcedureName, IDictionary<string, string> parametersDictionary)
        {
            Dbconnection theConnection = new Dbconnection();

            return theConnection.ExeSPWithResults(storedProcedureName, parametersDictionary);
        }

        public void extractXMLText(string xmlText)
        {


            IDictionary<string, string> parametersDictionary = new Dictionary<string, string>();
            parametersDictionary.Add("@textFile", xmlText);

            ExeSPWithResults("rawdata_text_insert", parametersDictionary);




        }

        private void Message_Listener(IMessage message)
        {


            ITextMessage txtMsg = message as ITextMessage;
            //MessageBox.Show(txtMsg.Text);
            string body = txtMsg.Text;


            extractXMLText(body);

            IObjectMessage objMessage = message as IObjectMessage;
            OperatorRequestObject OperatorRequestObject = ((BusinessObjects.OperatorRequestObject)(objMessage.Body));

           
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
