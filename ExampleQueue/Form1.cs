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


namespace ExampleQueue
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IObjectMessage objMessage;
            OperatorRequestObject OperatorRequestObject = new OperatorRequestObject();
            OperatorRequestObject.Shortcode = textBox1.Text.ToString();

            IConnectionFactory factory = new NMSConnectionFactory("tcp://localhost:61616");
            IConnection connection = factory.CreateConnection();
            connection = factory.CreateConnection();
            connection.Start();

            ISession session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge);
            IDestination QueueDestination = SessionUtil.GetDestination(session, "ExampleQueue");
            IMessageProducer MessageProducer = session.CreateProducer(QueueDestination);
            objMessage = session.CreateObjectMessage(OperatorRequestObject);

            MessageProducer.Send(objMessage);
            session.Close();
            connection.Stop();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
