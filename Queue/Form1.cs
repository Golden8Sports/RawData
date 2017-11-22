using Apache.NMS;
using DataLayer;
using Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace Queue
{
    public partial class Form1 : Form
    {
        private DateTime inititalTime = DateTime.Now;
        private string hour = DateTime.Now.ToString();
        private webReq theWebRequest = new webReq();
        private IContainer components = (IContainer)null;
        private string query;
        private string query2;
        private string attrValue;
        private line lineClass;
        private int numberMessages;
        private Label label1;
        private Label label2;

        public Form1()
        {
            InitializeComponent();
            this.receiver();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public object doQuery(string query)
        {
            return new Dbconnection().ExeScalar(query);
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

        public void extractXML(string xmlText)
        {
            new XmlDocument().LoadXml(xmlText);
            string str = xmlText.Replace("utf-8", "UTF-16");
            IDictionary<string, string> parametersDictionary = (IDictionary<string, string>)new Dictionary<string, string>();
            parametersDictionary.Add("@xmlFile", str);
            this.ExeSPWithResults("rawdata_xml_insert", parametersDictionary);
        }

        public DataTable ExeSPWithResults(string storedProcedureName, IDictionary<string, string> parametersDictionary)
        {
            return new Dbconnection().ExeSPWithResults(storedProcedureName, parametersDictionary);
        }

        public void extractXMLText(string xmlText)
        {
            IDictionary<string, string> parametersDictionary = (IDictionary<string, string>)new Dictionary<string, string>();
            parametersDictionary.Add("@textFile", xmlText);
            this.ExeSPWithResults("rawdata_text_insert", parametersDictionary);
        }

        public string getLineDate(int eventId)
        {
            IDictionary<string, string> parametersDictionary = (IDictionary<string, string>)new Dictionary<string, string>();
            parametersDictionary.Add("id_event", eventId.ToString());
            Dbconnection dbconnection = new Dbconnection();
            DataTable dataTable1 = new DataTable();
            DataTable dataTable2 = dbconnection.ExeSPWithResults("get_date_from_event_id", parametersDictionary);
            return dataTable2.Rows.Count <= 0 ? "NOID" : dataTable2.Rows[0][0].ToString();
        }
        public void postOng8Sports(line lineCls, Dictionary<string, string> dictionary)
        {
            if (lineCls.event_id == -9999)
                return;
            string str1 = this.getLineDate(lineCls.event_id).ToString();
            if (str1 != "NOID")
            {
                string str2 = str1.Substring(0, 10);
                string user = "g8";
                string pwd = "g8bridge";
                string action = "setValue";
                string rot = lineCls.rot.ToString();
                string sport = lineCls.sport_id.ToString();
                string period = lineCls.period_id.ToString();
                string lineTypeID = "154";
                int? nullable1 = lineCls.ml_away_price;
                string visitorML = nullable1.ToString();
                nullable1 = lineCls.ml_home_price;
                string homeML = nullable1.ToString();
                float? nullable2 = lineCls.total;
                string total = nullable2.ToString();
                nullable1 = lineCls.over_price;
                string totalOver = nullable1.ToString();
                nullable1 = lineCls.under_price;
                string totalUnder = nullable1.ToString();
                nullable2 = lineCls.ps_away_spread;
                string visitorSpread = nullable2.ToString();
                nullable1 = lineCls.ps_away_price;
                string visitorSpreadOdds = nullable1.ToString();
                nullable2 = lineCls.ps_home_spread;
                string homeSpread = nullable2.ToString();
                string homeSpreadOdds = lineCls.ps_home_price.ToString();
                nullable1 = lineCls.draw_price;
                string draw = nullable1.ToString();
                string sportBookId = lineCls.sportsbook.ToString();
                string date = str2;
                string leagueid = lineCls.league_id.ToString();

                this.doWebReq(user, pwd, action, rot, sport, period, lineTypeID, visitorML, homeML, total, totalOver, totalUnder, visitorSpread, visitorSpreadOdds, homeSpread, homeSpreadOdds, draw, sportBookId, date, leagueid);
            }
            else
                Console.WriteLine("There are not event id  " + (object)lineCls.event_id + "in database, therefore can not be sent to API");
        }

        public async void doWebReq(string user, string pwd, string action, string rot, string sport, string period, string lineTypeID, string visitorML, string homeML, string total, string totalOver, string totalUnder, string visitorSpread, string visitorSpreadOdds, string homeSpread, string homeSpreadOdds, string draw, string sportBookId, string date, string leagueid)
        {
            await this.theWebRequest.asyncasyncmakePost(user, pwd, action, rot, sport, period, lineTypeID, visitorML, homeML, total, totalOver, totalUnder, visitorSpread, visitorSpreadOdds, homeSpread, homeSpreadOdds, draw, sportBookId, date, leagueid);
        }


        public int processTxtMessage(int linesCount, IMessage msg)
        {
            Console.WriteLine("");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            if (msg is ITextMessage)
            {
                int num = 0;
                this.query = "insert into ";
                this.query2 = "values ";
                string text = (msg as ITextMessage).Text;
                Console.WriteLine("PROCESSING message " + (object)this.numberMessages + "from + " + this.hour + text);
                this.extractXMLText(text);
                try
                {
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                XDocument xdocument = XDocument.Parse(text);
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(text);
                foreach (XElement descendant in xdocument.Descendants())
                {
                    ++num;
                    if (num == 1)
                    {
                        this.query = this.query + (object)descendant.Name + " (";
                        this.query2 = this.query2 + "(";
                    }
                    foreach (XAttribute attribute in descendant.Attributes())
                    {
                        string key = string.Concat((object)attribute.Name);
                        if (attribute.Name == (XName)"away_spread" || attribute.Name == (XName)"away_price" || attribute.Name == (XName)"home_spread" || attribute.Name == (XName)"home_price")
                        {
                            this.attrValue = attribute.Value;
                            key = descendant.Name.ToString() + "_" + (object)attribute.Name ?? "";
                        }
                        this.query = this.query + key + ",";
                        this.attrValue = !(attribute.Name == (XName)"timestamp") && !(attribute.Name == (XName)"time") && !(attribute.Name == (XName)"message_timestamp") && !(attribute.Name == (XName)"open_time") ? attribute.Value : this.convertToEastern(attribute.Value).ToString();
                        this.query2 = this.query2 + "'" + this.attrValue + "',";
                        dictionary.Add(key, this.attrValue);
                    }
                }
                foreach (XmlNode childNode1 in xmlDocument.ChildNodes)
                {
                    foreach (XmlNode childNode2 in childNode1.ChildNodes)
                    {
                        foreach (XmlAttribute attribute in (XmlNamedNodeMap)childNode2.Attributes)
                        {
                            foreach (XmlNode childNode3 in childNode2.ChildNodes)
                                ;
                        }
                    }
                }
                this.query = this.query + "timeReceived) ";
                this.query2 = this.query2 + "getDate())";
                this.query = this.query.Replace(",)", ")");
                this.query2 = this.query2.Replace(",)", ")");
                try
                {
                    this.doQuery(this.query + this.query2);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("DB exception: " + ex.Message + "(" + this.query + this.query2 + ")");
                }
                try
                {
                    if (this.getDonBestStatus() == "1")
                    {
                        if (int.Parse(this.GetValue("rot", dictionary)) != -9999)
                        {
                            this.lineClass = this.createClassLine(dictionary);
                            this.postOng8Sports(this.lineClass, dictionary);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message + "(" + this.query + this.query2 + ")");
                }
                return linesCount + 1;
            }
            Console.WriteLine("Unexpected message type: " + msg.GetType().Name + "(" + this.query + this.query2 + ")");
            return linesCount + 1;
        }

        public string getDonBestStatus()
        {
            IDictionary<string, string> parametersDictionary = (IDictionary<string, string>)new Dictionary<string, string>();
            Dbconnection dbconnection = new Dbconnection();
            DataTable dataTable = new DataTable();
            return dbconnection.ExeSPWithResultsdb2("GetDonBestStatus", parametersDictionary).Rows[0][0].ToString();
        }

        private string GetValue(string key, Dictionary<string, string> dictionary)
        {
            string str;
            if (!dictionary.TryGetValue(key, out str))
                str = "-9999";
            return str;
        }
        public line createClassLine(Dictionary<string, string> dictionary)
        {
            this.lineClass = new line();
            this.lineClass.rot = new int?(int.Parse(this.GetValue("rot", dictionary)));
            this.lineClass.sport_id = int.Parse(this.GetValue("sport_id", dictionary));
            this.lineClass.away_money = new int?(int.Parse(this.GetValue("away_money", dictionary)));
            this.lineClass.period = this.GetValue("period", dictionary);
            this.lineClass.period_id = int.Parse(this.GetValue("period_id", dictionary));
            this.lineClass.ml_away_price = new int?(int.Parse(this.GetValue("ml_away_price", dictionary)));
            this.lineClass.ml_home_price = new int?(int.Parse(this.GetValue("ml_home_price", dictionary)));
            this.lineClass.total = new float?(float.Parse(this.GetValue("total", dictionary)));
            this.lineClass.over_price = new int?(int.Parse(this.GetValue("over_price", dictionary)));
            this.lineClass.under_price = new int?(int.Parse(this.GetValue("under_price", dictionary)));
            this.lineClass.ps_away_spread = new float?(float.Parse(this.GetValue("ps_away_spread", dictionary)));
            this.lineClass.ps_away_price = new int?(int.Parse(this.GetValue("ps_away_price", dictionary)));
            this.lineClass.ps_home_spread = new float?(float.Parse(this.GetValue("ps_home_spread", dictionary)));
            this.lineClass.ps_home_price = new double?((double)float.Parse(this.GetValue("ps_home_price", dictionary)));
            this.lineClass.draw_price = new int?(int.Parse(this.GetValue("draw_price", dictionary)));
            this.lineClass.ps_away_price = new int?(int.Parse(this.GetValue("ps_away_price", dictionary)));
            this.lineClass.sportsbook = new short?(short.Parse(this.GetValue("sportsbook", dictionary)));
            this.lineClass.event_id = int.Parse(this.GetValue("event_id", dictionary));
            this.lineClass.league_id = int.Parse(this.GetValue("league_id", dictionary));
            return this.lineClass;
        }

        private DateTime convertToEastern(string originalDate)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(originalDate.Length != 24 ? DateTime.ParseExact(originalDate, "yy-MM-dd'T'HH:mm:ssK", (IFormatProvider)CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal) : DateTime.ParseExact(originalDate, "yyyy-MM-dd'T'HH:mm:ssK", (IFormatProvider)CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal), TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
        }

        private void Message_Listener(IMessage message)
        {
            ITextMessage textMessage = message as ITextMessage;
            string text = textMessage.Text;
            Console.WriteLine(text);
            this.extractXMLText(text);
            this.numberMessages = this.processTxtMessage(this.numberMessages, (IMessage)textMessage);
        }


    }
}
