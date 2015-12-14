using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CSWing.Stockfighter.Api
{
    public class Gm
    {
        HttpClient httpClient;

        public Gm(string authToken)
        {
            httpClient = new HttpClient();

            httpClient.BaseAddress = new Uri("https://api.stockfighter.io/gm/");
            httpClient.DefaultRequestHeaders.Add("X-Stockfighter-Authorization", authToken);
        }

        class Response
        {
            public bool Ok { get; set; }
        }

        class LevelResponse : Response
        {
            public string Account { get; set; }
            public int InstanceId { get; set; }
            public class LevelInstructions
            {
                public string Instructions { get; set; }
                public string OrderTypes { get; set; }

            }
            public LevelInstructions Instructions { get; set; }
            public int SecondsPerTradingDay { get; set; }
            public IList<string> Tickers { get; set; }
            public IList<string> Venues { get; set; }
        }

        public async Task<int> StartLevel(string level)
        {
            var res = await httpClient.PostAsync(string.Format("levels/{0}", level), null);

            if (!res.IsSuccessStatusCode)
            {
                return 0;
            }

            var json = await res.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<LevelResponse>(json);

            if (!data.Ok)
            {
                return 0;
            }

            return data.InstanceId;
        }

        class LevelOperationResponse : Response
        {
            public string Error { get; set; }
        }

        public async Task<bool> DoLevelOperation(int instanceId, string operation)
        {
            var res = await httpClient.PostAsync(string.Format("instances/{0}/{1}", instanceId, operation), null);

            if (!res.IsSuccessStatusCode)
            {
                return false;
            }

            var json = await res.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<LevelOperationResponse>(json);

            if (!data.Ok)
            {
                return false;
            }

            return true;
        }
    }
}
