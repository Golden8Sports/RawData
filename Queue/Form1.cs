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
using System.Xml;
using System.Xml.Linq;



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
            int cont = 0;
            string query="", query2="";
            string attrValue;

            ITextMessage txtMsg = message as ITextMessage;
            //MessageBox.Show(txtMsg.Text);
            string body = txtMsg.Text;


            extractXMLText(body);
            try
            {


                extractXML(body);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            var xdoc = XDocument.Parse(body);
            XmlDocument xDOC2 = new XmlDocument();
            xDOC2.LoadXml(body);


            foreach (var el in xdoc.Descendants())

            {





                cont += 1;
                //Console.WriteLine("Nodo " +  el.Name + ":" + el.Value + " " + el.Name);

                if (cont == 1)
                {
                    query += el.Name + " (";
                    query2 += "(";
                }

                foreach (var attr in el.Attributes())
                {

                    string attrName = attr.Name + "";

                    if (attr.Name == "away_spread" || attr.Name == "away_price" || attr.Name == "home_spread" || attr.Name == "home_price")
                    {
                        attrValue = attr.Value;
                        //Console.WriteLine("Nombre " + attr.Name + ", value:" + attr.Name);


                        attrName = el.Name + "_" + attr.Name + "";
                    }

                    //Console.WriteLine(attr.Name);
                    query += attrName + ",";


                    if (attr.Name == "timestamp" || attr.Name == "time" || attr.Name == "message_timestamp" || attr.Name == "open_time")
                    {

                        attrValue = convertToEastern(attr.Value).ToString();
                    }




                    else



                    {
                        attrValue = attr.Value;
                        //Console.WriteLine("Nombre " + attr.Name + ", value:" + attr.Name);
                    }

                    query2 += "'" + attrValue + "',";

                    dictionaryQuery.Add(attrName, attrValue);
                }

                foreach (XmlNode xmlNode3 in xDOC2.ChildNodes)
                {

                    //Console.WriteLine("cHILDnODE NAME:" + xmlNode3.Name);

                    foreach (XmlNode xmlNode4 in xmlNode3.ChildNodes)
                    {
                        //Console.WriteLine("gRANDCHildnode name: " + xmlNode4.Name);

                        foreach (XmlAttribute attr in xmlNode4.Attributes)
                        {


                            //Console.WriteLine("Atributo: " + attr.Name + "Valor:" + attr.Value);
                            //query += attr.Name + ",";


                            foreach (XmlNode xmlNode5 in xmlNode4.ChildNodes)
                            {
                                //Console.WriteLine("Son of " + attr.Name + "name" + xmlNode5.Name);

                                //if (attr.Name == "timestamp" || attr.Name == "time" || attr.Name == "message_timestamp" || attr.Name == "open_time")
                                //{

                                //    attrValue = convertToEastern(attr.Value).ToString();
                                //}
                                //else
                                //{
                                //    attrValue = attr.Value;
                                //}

                                //query2 += "'" + attrValue + "',";


                            }

                            //if (attr.Name == "timestamp" || attr.Name == "time" || attr.Name == "message_timestamp" || attr.Name == "open_time")
                            //{

                            //    attrValue = convertToEastern(attr.Value).ToString();
                            //}
                            //else
                            //{
                            //    attrValue = attr.Value;
                            //}

                            //query2 += "'" + attrValue + "',";


                        }




                    }
                }

                query += "timeReceived) ";
                query2 += "getDate())";

                query = query.Replace(",)", ")");
                query2 = query2.Replace(",)", ")");

                try
                {
                    doQuery(query + query2);





                    //foreach (KeyValuePair<string, string> entry in dictionaryQuery)
                    //{
                    //    Console.WriteLine(entry.Key + "," + entry.Value);
                    //}

                    //doWebReq("g8", "g8bridge", "action", "rot", dictionaryQuery["sport_id"], dictionaryQuery["period"], "", dictionaryQuery["sport_id"], dictionaryQuery["ml_away_price"], dictionaryQuery["ml_home_price"], dictionaryQuery["total"], dictionaryQuery["under_price"], dictionaryQuery["over_price"], dictionaryQuery["ml_away_price"], dictionaryQuery["ps_away_spread"], dictionaryQuery["ml_away_price"], dictionaryQuery["ml_away_price"]);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("DB exception: " + ex.Message + "(" + query + query2 + ")");
                }


                try
                {
                    if (this.getDonBestStatus() == "1")
                    {


                        if (int.Parse(GetValue("rot", dictionaryQuery)) != -9999)
                        {
                            lineClass = this.createClassLine(dictionaryQuery);
                            postOng8Sports(lineClass, dictionaryQuery);
                        }

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message + "(" + query + query2 + ")");


                }




            }



            IObjectMessage objMessage = message as IObjectMessage;
            OperatorRequestObject OperatorRequestObject = ((BusinessObjects.OperatorRequestObject)(objMessage.Body));

           
        }

        public void extractXML(string xmlText)
        {

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlText);


            string xmlToSQL = xmlText.Replace("utf-8", "UTF-16");
            IDictionary<string, string> parametersDictionary = new Dictionary<string, string>();
            parametersDictionary.Add("@xmlFile", xmlToSQL);

            ExeSPWithResults("rawdata_xml_insert", parametersDictionary);




        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
