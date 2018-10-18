using Apache.NMS;
using DataLayer;
using DB.Entities;
using DB.Logic;
using Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace Queue
{
    public class MainClassRAWData
    {
        private DateTime inititalTime = DateTime.Now;
        private string hour = DateTime.Now.ToString();
        private webReq theWebRequest = new webReq();
        private string query;
        private string query2;
        private string attrValue;
        private line lineClass;
        private int numberMessages;
        private System.Threading.Thread thread;
        private System.Threading.Thread Cicle;
        private blLine lineBL = new blLine();
        private int MinutesOFF = -3;
        private string Errors = "";

        public MainClassRAWData()
        {
            Start();            
        }


        private void Print(string txt)
        {
            int i = txt.Max();

            try
            {
                if(txt.Length < i)
                {
                    // Set a variable to the My Documents path.
                    string mydocpath =
                   Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                    // Write the string array to a new file named "WriteLines.txt".
                    using (StreamWriter outputFile = new StreamWriter(mydocpath + @"\ErrorLines.txt"))
                    {
                        //foreach (string line in lines)
                        outputFile.WriteLine(Environment.NewLine + txt);
                    }
                }
                else
                {
                    txt = "";
                }
            }
            catch (Exception)
            {
                txt = "";
            }
        }


        private void Consult()
        {
            csLine collection = null;
            
            while (true)
            {
                System.Threading.Thread.Sleep(TimeSpan.FromMinutes(2));
                collection = lineBL.GetLastDate();

                if (collection != null)
                {
                    if (collection.MinutesStopped <= MinutesOFF && collection.MinutesHeartBeat <= MinutesOFF)
                    {
                        Application.ExitThread();
                        Application.Exit();
                        Application.Restart();
                        Environment.Exit(0);                    
                    }
                }               
            }            
        }


        private void Start()
        {
            this.thread = new System.Threading.Thread(receiver);
            this.thread.Start();

            this.Cicle = new System.Threading.Thread(Consult);
            this.Cicle.Start();
        }

        public void Stop(){ this.thread.Abort(); this.Cicle.Abort();}


        private void receiver()
        {
            try
            {
                string topic = "com.donbest.message.public.xmleddie";
                string userName = "xmleddie";
                string password = "xmlfootball";
                string brokerUri = "tcp://amq.donbest.com:61616";

                IConnectionFactory factory = new NMSConnectionFactory(brokerUri);
                IConnection connection = factory.CreateConnection(userName, password);
                connection.Start();
                ISession session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge);
                IDestination destination = session.GetTopic(topic);
                IMessageConsumer receiver = session.CreateConsumer(destination);
                receiver.Listener += new MessageListener(Message_Listener);
            }
            catch (Exception ex)
            {
                Errors += ex.Message + DateTime.Now + Environment.NewLine;
                Print(Errors);
                Console.WriteLine("Error: " + ex.Message);
            }
        }


        public void extractXML(string xmlText)
        {
            try
            {
                new XmlDocument().LoadXml(xmlText);
                string str = xmlText.Replace("utf-8", "UTF-16");
                IDictionary<string, string> parametersDictionary = (IDictionary<string, string>)new Dictionary<string, string>();
                parametersDictionary.Add("@xmlFile", str);
                this.ExeSPWithResults("rawdata_xml_insert", parametersDictionary);
            }catch (Exception ex)
            {
                Errors += ex.Message + DateTime.Now + Environment.NewLine;
                Print(Errors);
            }
        }

        public DataTable ExeSPWithResults(string storedProcedureName, IDictionary<string, string> parametersDictionary)
        {
            return new Dbconnection().ExeSPWithResults(storedProcedureName, parametersDictionary);
        }

        public void extractXMLText(string xmlText)
        {
            try
            {
                IDictionary<string, string> parametersDictionary = (IDictionary<string, string>)new Dictionary<string, string>();
                parametersDictionary.Add("@textFile", xmlText);
                this.ExeSPWithResults("rawdata_text_insert", parametersDictionary);
            }catch (Exception ex)
            {
                Errors += ex.Message + DateTime.Now + Environment.NewLine;
                Print(Errors);
            }
        }

        public string getLineDate(int eventId)
        {
            DataTable dataTable2 = null;

            try
            {
                IDictionary<string, string> parametersDictionary = (IDictionary<string, string>)new Dictionary<string, string>();
                parametersDictionary.Add("id_event", eventId.ToString());
                Dbconnection dbconnection = new Dbconnection();
                DataTable dataTable1 = new DataTable();
                dataTable2 = dbconnection.ExeSPWithResults("get_date_from_event_id", parametersDictionary);
                
            } catch (Exception ex)
            {
                Errors += ex.Message + DateTime.Now + Environment.NewLine;
                Print(Errors);
                Console.WriteLine("Error to getLineDate: " + ex.Message);
            }

            return dataTable2.Rows.Count <= 0 ? "NOID" : dataTable2.Rows[0][0].ToString();
        }

        public void postOng8Sports(line lineCls, Dictionary<string, string> dictionary)
        {
            try
            {
                if (lineCls.event_id == -9999) return;

                string str1 = this.getLineDate(lineCls.event_id).ToString();

                if (str1 != "NOID")
                {
                    // string str2 = str1.Substring(0,10); //dt.Date.ToShortDateString();
                    DateTime dt = Convert.ToDateTime(str1);
                    string dateBridge = dt.Month.ToString() + "/" + dt.Day.ToString() + "/" + dt.Year.ToString();

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
                    string leagueid = lineCls.league_id.ToString();

                    this.doWebReq(user, pwd, action, rot, sport, period, lineTypeID, visitorML, homeML, total, totalOver, totalUnder, visitorSpread, visitorSpreadOdds, homeSpread, homeSpreadOdds, draw, sportBookId, dateBridge, leagueid);
                }
                else
                {
                    Console.WriteLine("There are not event id  " + (object)lineCls.event_id + "in database, therefore can not be sent to API");
                }
                    
            }
            catch (Exception ex)
            {
                Console.WriteLine("****************  THERE IS AN ERROR ******************: " + ex.Message + " Date: " + this.getLineDate(lineCls.event_id).ToString());
                Errors += ex.Message + DateTime.Now + Environment.NewLine;
                Print(Errors);
            }
        }
            
        public async void doWebReq(string user, string pwd, string action, string rot, string sport, string period, string lineTypeID, string visitorML, string homeML, string total, string totalOver, string totalUnder, string visitorSpread, string visitorSpreadOdds, string homeSpread, string homeSpreadOdds, string draw, string sportBookId, string date, string leagueid)
        {
            await this.theWebRequest.asyncasyncmakePost(user, pwd, action, rot, sport, period, lineTypeID, visitorML, homeML, total, totalOver, totalUnder, visitorSpread, visitorSpreadOdds, homeSpread, homeSpreadOdds, draw, sportBookId, date, leagueid);
        }

        public object doQuery(string query)
        {
            return new Dbconnection().ExeScalar(query);
        }

        public int processTxtMessage(int linesCount, IMessage msg)
        {
            try
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

                    try
                    {
                        this.extractXMLText(text);
                    }
                    catch (Exception ex)
                    {
                        Errors += ex.Message + DateTime.Now + Environment.NewLine;
                        Print(Errors);
                        Console.WriteLine(ex.Message);
                    }


                    if(text.ToUpper().Contains("806592"))
                    {
                        int n = 0;
                    }

                    XDocument xdocument = XDocument.Parse(text);
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(text);

                    foreach (XElement descendant in xdocument.Descendants())
                    {


                        if (text.ToUpper().Contains("806592"))
                        {
                            int n = 0;
                        }


                        if (num == 0)
                        {
                            this.query = this.query + (object)descendant.Name + " (";
                            this.query2 = this.query2 + "(";
                            num += 1;
                        }

                        foreach (XAttribute attribute in descendant.Attributes())
                        {

                            if (text.ToUpper().Contains("806593") && text.ToUpper().Contains("1") && text.ToUpper().Contains("18") && !text.ToUpper().Contains("SCORE"))
                            {
                                int n = 0;
                            }


                            string key = string.Concat((object)attribute.Name);
                            if (attribute.Name == (XName)"away_spread" || attribute.Name == (XName)"away_price" || attribute.Name == (XName)"home_spread" || attribute.Name == (XName)"home_price")
                            {
                                this.attrValue = attribute.Value;
                                key = descendant.Name.ToString() + "_" + (object)attribute.Name ?? "";
                            }
                            this.query = this.query + key + ",";
                            this.attrValue = !(attribute.Name == (XName)"timestamp") && !(attribute.Name == (XName)"time") && !(attribute.Name == (XName)"message_timestamp") && !(attribute.Name == (XName)"open_time") ? attribute.Value : this.convertToEastern(attribute.Value).ToString();
                            

                            if (key == "time" || key == "message_timestamp" || key == "timestamp")
                            {
                                DateTime dt = Convert.ToDateTime(this.attrValue);
                                dictionary.Add(key, dt.Year + "-" + dt.Month + "-" + dt.Day);
                                this.query2 = this.query2 + "'" + (dt.Year + "-" + dt.Month + "-" + dt.Day + " " + dt.Hour + ":" + dt.Minute + ":" + dt.Second + "." + dt.Millisecond) + "',";
                            }
                            else
                            {
                                dictionary.Add(key, this.attrValue);
                                this.query2 = this.query2 + "'" + this.attrValue + "',";
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
                        Errors += ex.Message + DateTime.Now + Environment.NewLine;
                        Print(Errors);
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
                        Errors += ex.Message + DateTime.Now + Environment.NewLine;
                        Print(Errors);
                        Console.WriteLine("Error: " + ex.Message + "(" + this.query + this.query2 + ")");
                    }
                    return linesCount + 1;
                }

                Console.WriteLine("Unexpected message type: " + msg.GetType().Name + "(" + this.query + this.query2 + ")");
            }catch (Exception ex)
            {
                Errors += ex.Message + DateTime.Now + Environment.NewLine;
                Print(Errors);
            }

            return linesCount + 1;
        }

        public string getDonBestStatus()
        {
            IDictionary<string, string> parametersDictionary = null;
            Dbconnection dbconnection = null;
            DataTable dataTable = null;

            try
            {
                parametersDictionary = (IDictionary<string, string>)new Dictionary<string, string>();
                dbconnection = new Dbconnection();
                dataTable = new DataTable();
            }catch (Exception ex)
            {
                Errors += ex.Message + DateTime.Now + Environment.NewLine;
                Print(Errors);
            }

            return dbconnection.ExeSPWithResultsdb2("GetDonBestStatus", parametersDictionary).Rows[0][0].ToString();
        }

        private string GetValue(string key, Dictionary<string, string> dictionary)
        {
            string str = "";

            try
            {               
                if (!dictionary.TryGetValue(key, out str)) str = "-9999";                
            }catch (Exception ex)
            {
                Errors += ex.Message + DateTime.Now + Environment.NewLine;
                Print(Errors);
            }

            return str;
        }

        public line createClassLine(Dictionary<string, string> dictionary)
        {
            this.lineClass = new line();
            try
            {
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
            }
            catch (Exception ex)
            {
                Errors += ex.Message + DateTime.Now + Environment.NewLine;
                Print(Errors);
                Console.WriteLine("Error to createClassLine: " + ex.Message);
            }

            return this.lineClass;
        }

        private DateTime convertToEastern(string originalDate)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(originalDate.Length != 24 ? DateTime.ParseExact(originalDate, "yy-MM-dd'T'HH:mm:ssK", (IFormatProvider)CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal) : DateTime.ParseExact(originalDate, "yyyy-MM-dd'T'HH:mm:ssK", (IFormatProvider)CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal), TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
        }

        private void Message_Listener(IMessage message)
        {
            try
            {
                ITextMessage textMessage = message as ITextMessage;
                string text = textMessage.Text;
                Console.WriteLine(text);
                this.extractXMLText(text);
                this.numberMessages = this.processTxtMessage(this.numberMessages, (IMessage)textMessage);
            } catch (Exception ex)
            {
                Errors += ex.Message + DateTime.Now + Environment.NewLine;
                Print(Errors);
            }
        }
    }




 //**************************************************  MAIN CLASS ****************************************************************

    static class Program
    {
        private static MainClassRAWData MainClass = null;

        [STAThread]
        static void Main(string[] args)
        {
            MainClass = new MainClassRAWData();
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;                    
            while(true){}
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            MainClass.Stop();
        }

//***************************************************************************************************************************************
    }
}
