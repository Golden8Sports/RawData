using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace Queue
{
    class webReq
    {
        private static readonly HttpClient client = new HttpClient();

        public async Task asyncasyncmakePost(string user, string pwd, string action, string rot, string sport, string period, string lineTypeID, string visitorML, string homeML, string total, string totalOver, string totalUnder, string visitorSpread, string visitorSpreadOdds, string homeSpread, string homeSpreadOdds, string draw, string sportBookId, string date, string leagueid)

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

            var response = await client.PostAsync("http://tools.golden8sports.com/bridge/donbest.asp", content);

            if (response != null)
            {
                // Error Here
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseContent);
            }

            Console.WriteLine(" u :" + user + ", p :" + pwd + ", action: " + action + ",rot: " + rot + ",sid: " + sport + ",pid: " + period + ",lid: " + lineTypeID + ",vml: " + visitorML + ",hml: " + homeML + ",ttl: " + total + ",tov: " + totalOver + ",tun: " + totalUnder + ",vsd: " + visitorSpread + ",vso: " + visitorSpreadOdds + ",hsd: " + homeSpread + ",hso: " + homeSpreadOdds + ",draw: " + draw + ",sbkid: " + sportBookId + ",gdt: " + date + ",leagueid: " + leagueid + "");

        }

    }
}
