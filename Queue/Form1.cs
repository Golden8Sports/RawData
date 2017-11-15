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
using System.Globalization;
using Entities;


namespace Queue
{
    public partial class Form1 : Form
    {
        line lineClass;
        webReq theWebRequest = new webReq();
        public Form1()
        {
            InitializeComponent();
            receiver();
        }

        public async void doWebReq(string user, string pwd, string action, string rot, string sport, string period, string lineTypeID, string visitorML, string homeML, string total, string totalOver, string totalUnder, string visitorSpread, string visitorSpreadOdds, string homeSpread, string homeSpreadOdds, string draw, string sportBookId, string date)
        {


            await theWebRequest.asyncasyncmakePost(user, pwd, action, rot, sport, period, lineTypeID, visitorML, homeML, total, totalOver, totalUnder, visitorSpread, visitorSpreadOdds, homeSpread, homeSpreadOdds, draw, sportBookId, date);
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

        public object doQuery(string query)
        {
            Dbconnection dbCon = new Dbconnection();
            return dbCon.ExeScalar(query);
        }

        DateTime convertToEastern(string originalDate)
        {


            int dateLength;
            DateTime dt;


            dateLength = originalDate.Length;



            //'Actual received date is  "2017-11-10T13:02:37+0000"" />
            //before was <heart_beat timestamp="17-11-09T15:30:39+0000" />
            if (dateLength == 24)
            {
                dt = DateTime.ParseExact(originalDate, "yyyy-MM-dd'T'HH:mm:ssK",
                                CultureInfo.InvariantCulture,
                                DateTimeStyles.AdjustToUniversal);
            }
            else
            {
                dt = DateTime.ParseExact(originalDate, "yy-MM-dd'T'HH:mm:ssK",
                               CultureInfo.InvariantCulture,
                               DateTimeStyles.AdjustToUniversal);
            }


            TimeZoneInfo easternTimeZone = TimeZoneInfo.FindSystemTimeZoneById(
                                 "Eastern Standard Time");

            DateTime easternDateTime = TimeZoneInfo.ConvertTimeFromUtc(dt,
                                                                       easternTimeZone);

            return easternDateTime;
        }


        public object doQuery(string table, string column, string text)
        {
            string query;
            query = "insert into " + table + "(" + column + ") values ('" + text + "')"; ;

            Dbconnection dbCon = new Dbconnection();
            return dbCon.ExeScalar(query);
        }

        private void Message_Listener(IMessage message)
        {
            Dictionary<string, string> dictionaryQuery = new Dictionary<string, string>();
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

        public void postOng8Sports(line lineCls, Dictionary<string, string> dictionary)
        {



            if (lineCls.event_id != -9999)
            {



                string lineDate = this.getLineDate(lineCls.event_id).ToString();



                if (lineDate != "NOID")
                {

                    lineDate = lineDate.Substring(0, 10);
                    doWebReq("g8", "g8bridge", "setValue", lineCls.rot.ToString(), lineCls.sport_id.ToString(), lineCls.period_id.ToString(), "154", lineCls.ml_away_price.ToString(), lineCls.ml_home_price.ToString(), lineCls.total.ToString(), lineCls.over_price.ToString(), lineCls.under_price.ToString(), lineCls.ps_away_spread.ToString(), lineCls.ps_away_price.ToString(), lineCls.ps_home_spread.ToString(), lineCls.ps_home_price.ToString(), lineCls.draw_price.ToString(), lineCls.sportsbook.ToString(), lineDate);
                }
                else

                {
                    Console.WriteLine("There are not event id  " + lineCls.event_id + "in database, therefore can not be sent to API");
                }
            }
        }

        private string GetValue(string key, Dictionary<String, String> dictionary)
        {
            string returnValue;
            if (!dictionary.TryGetValue(key, out returnValue))
            {
                returnValue = "-9999";
            }
            return returnValue;
        }

        public string getLineDate(int eventId)

        {

            string returnValue;

            IDictionary<string, string> parametersDictionary = new Dictionary<string, string>();


            parametersDictionary.Add("id_event", eventId.ToString());

            Dbconnection theDbConnection = new Dbconnection();

            DataTable theDatatable = new DataTable();

            theDatatable = theDbConnection.ExeSPWithResults("get_date_from_event_id", parametersDictionary);

            if (theDatatable.Rows.Count > 0)
            {

                returnValue = theDatatable.Rows[0][0].ToString();

            }

            else
            {
                returnValue = "NOID";

            }

            return returnValue;

        }

        public string getDonBestStatus()
        {

            IDictionary<string, string> parametersDictionary = new Dictionary<string, string>();

            Dbconnection theDbConnection = new Dbconnection();

            DataTable theDatatable = new DataTable();

            theDatatable = theDbConnection.ExeSPWithResultsdb2("GetDonBestStatus", parametersDictionary);

            string returnValue = theDatatable.Rows[0][0].ToString();



            return returnValue;
        }

        public line createClassLine(Dictionary<String, String> dictionary)
        {
            //lineClass.away_price = int.Parse(dictionary["ps_away_money"]);
            //lineClass.ps
            //            user = "g8”
            //pwd = “g8bridge”
            //action = Request.Form("action")




            lineClass.rot = int.Parse(GetValue("rot", dictionary));



            //lineTypeID = 154


            lineClass.sport_id = int.Parse(GetValue("sport_id", dictionary));
            lineClass.away_money = int.Parse(GetValue("away_money", dictionary));
            lineClass.period = GetValue("period", dictionary);
            lineClass.period_id = int.Parse(GetValue("period_id", dictionary));
            lineClass.ml_away_price = int.Parse(GetValue("ml_away_price", dictionary));
            lineClass.ml_home_price = int.Parse(GetValue("ml_home_price", dictionary));
            lineClass.total = float.Parse(GetValue("total", dictionary));
            lineClass.over_price = int.Parse(GetValue("over_price", dictionary));
            lineClass.under_price = int.Parse(GetValue("under_price", dictionary));
            lineClass.ps_away_spread = float.Parse(GetValue("ps_away_spread", dictionary));
            lineClass.ps_away_price = int.Parse(GetValue("ps_away_price", dictionary));
            lineClass.ps_home_spread = float.Parse(GetValue("ps_home_spread", dictionary));
            lineClass.ps_home_price = float.Parse(GetValue("ps_home_price", dictionary));
            lineClass.draw_price = int.Parse(GetValue("draw_price", dictionary));
            lineClass.ps_away_price = int.Parse(GetValue("ps_away_price", dictionary));
            lineClass.sportsbook = short.Parse(GetValue("sportsbook", dictionary));
            lineClass.event_id = int.Parse(GetValue("event_id", dictionary));

            return lineClass;



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
