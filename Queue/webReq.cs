using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;

namespace Queue
{
    public class webReq
    {
        private static readonly HttpClient client = new HttpClient();

        public async Task asyncasyncmakePost(string user, string pwd, string action, string rot, string sport, string period, string lineTypeID, string visitorML, string homeML, string total, string totalOver, string totalUnder, string visitorSpread, string visitorSpreadOdds, string homeSpread, string homeSpreadOdds, string draw, string sportBookId, string date, string leagueid)
        {
            try
            {
                var values = new Dictionary<string, string>
            {
                  { "u", user },
                  { "p", pwd },
                  { "action", action },
                  { "rot", rot },
                  { "sid", sport },
                  { "pid", period },
                  { "lid", lineTypeID },
                  { "vml", visitorML },
                  { "hml", homeML },
                  { "ttl", total },
                  { "tov", totalOver },
                  { "tun", totalUnder },
                  { "vsd", visitorSpread },
                  { "vso", visitorSpreadOdds },
                  { "hsd", homeSpread },
                  { "hso", homeSpreadOdds },
                  { "draw", draw },
                  { "sbkid", sportBookId },
                  { "gdt", date},
                  { "leagueid", leagueid}
            };

                var content = new FormUrlEncodedContent(values);


                if (leagueid.Trim() == "18" )
                {
                    int asd = 0;
                }

                //var response = await client.PostAsync("http://tools.golden8sports.com/bridge/donbest.asp", content);
                var response = await client.PostAsync("http://10.10.10.45:8077/bridge/donbest.asp", content);

                if (leagueid == "18" || sport == "1")
                {
                    int asd = 0;
                }

                if (response != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseContent);
                    Console.WriteLine(" u :" + user + ", p :" + pwd + ", action: " + action + ",rot: " + rot + ",sid: " + sport + ",pid: " + period + ",lid: " + lineTypeID + ",vml: " + visitorML + ",hml: " + homeML + ",ttl: " + total + ",tov: " + totalOver + ",tun: " + totalUnder + ",vsd: " + visitorSpread + ",vso: " + visitorSpreadOdds + ",hsd: " + homeSpread + ",hso: " + homeSpreadOdds + ",draw: " + draw + ",sbkid: " + sportBookId + ",gdt: " + date + ",leagueid: " + leagueid + "");
                }
                else
                {
                    Console.WriteLine("Error to post on G8 Sports");
                }

               
            }
            catch (Exception ex)
            {
                Console.WriteLine("***************** Error in the web request **************", ex.Message);
            }

        }

    }
}
