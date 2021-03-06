﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using Apache.NMS;
using Apache.NMS.Util;
using System.Threading;

namespace ConsoleApplication1
{
    class Program
    {

        protected static AutoResetEvent semaphore = new AutoResetEvent(false);
        protected static ITextMessage message = null;
        protected static TimeSpan receiveTimeout = TimeSpan.FromSeconds(1000);

        public static void Main(string[] args)
        {
            Program p = new Program();

            p.receiver();
        }
        

        protected static void OnMessage(IMessage receivedMsg)
        {
            message = receivedMsg as ITextMessage;
            semaphore.Set();
        }

        private void Message_Listener(IMessage message)
        {
            ITextMessage textMessage = message as ITextMessage;
            string text = textMessage.Text;
            Console.WriteLine(text);
            //this.extractXMLText(text);
            //this.numberMessages = this.processTxtMessage(this.numberMessages, (IMessage)textMessage);
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
            // Example connection strings:
            //    activemq:tcp://activemqhost:61616
            //    stomp:tcp://activemqhost:61613
            //    ems:tcp://tibcohost:7222
            //    msmq://localhost

            Uri connecturi = new Uri("activemq:tcp://activemqhost:61616");

            Console.WriteLine("About to connect to " + connecturi);

            // NOTE: ensure the nmsprovider-activemq.config file exists in the executable folder.

            IConnectionFactory factory = new NMSConnectionFactory(brokerUri);

            IConnection connection = factory.CreateConnection(userName, password);
            using (ISession session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge))
            {
                // Examples for getting a destination:
                //
                // Hard coded destinations:
                //    IDestination destination = session.GetQueue("FOO.BAR");
                //    Debug.Assert(destination is IQueue);
                //    IDestination destination = session.GetTopic("FOO.BAR");
                //    Debug.Assert(destination is ITopic);
                //
                // Embedded destination type in the name:
                //    IDestination destination = SessionUtil.GetDestination(session, "queue://FOO.BAR");
                //    Debug.Assert(destination is IQueue);
                //    IDestination destination = SessionUtil.GetDestination(session, "topic://FOO.BAR");
                //    Debug.Assert(destination is ITopic);
                //
                // Defaults to queue if type is not specified:
                //    IDestination destination = SessionUtil.GetDestination(session, "FOO.BAR");
                //    Debug.Assert(destination is IQueue);
                //
                // .NET 3.5 Supports Extension methods for a simplified syntax:
                //    IDestination destination = session.GetDestination("queue://FOO.BAR");
                //    Debug.Assert(destination is IQueue);
                //    IDestination destination = session.GetDestination("topic://FOO.BAR");
                //    Debug.Assert(destination is ITopic);

                IDestination destination = session.GetTopic(topic);

                Console.WriteLine("Using destination: " + destination);

                // Create a consumer and producer
                using (IMessageConsumer consumer = session.CreateConsumer(destination))
                //using (IMessageProducer producer = session.CreateProducer(destination))
                {
                    // Start the connection so that messages will be processed.
                    connection.Start();
                    //producer.Persistent = true;
                    //producer.RequestTimeout = receiveTimeout;
                    consumer.Listener += new MessageListener(OnMessage);

                    // Send a message
                    //ITextMessage request = session.CreateTextMessage("Hello World!");
                    //request.NMSCorrelationID = "abc";
                    //request.Properties["NMSXGroupID"] = "cheese";
                    //request.Properties["myHeader"] = "Cheddar";

                    //producer.Send(request);

                    // Wait for the message
                    semaphore.WaitOne((int)receiveTimeout.TotalMilliseconds, true);
                    if (message == null)
                    {
                        Console.WriteLine("No message received!");
                    }
                    else
                    {
                        Console.WriteLine("Received message with ID:   " + message.NMSMessageId);
                        Console.WriteLine("Received message with text: " + message.Text);
                    }
                }
            }
        }
    }

 



}
