using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HoloBowlFn.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace HoloBowlFn.Functions
{
    public static class AddScore
    {
        [FunctionName("AddScore")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestMessage req,
            [Table("Score")]CloudTable table,
            TraceWriter log)
        {
            var jsonString = await req.Content.ReadAsStringAsync();

            var data = JsonConvert.DeserializeObject<Score>(jsonString);

            if (data.PlayerName == null || data.PlayerName.Length != 3 || data.PlayerScore < 0 || data.PlayerScore > 99)
                return req.CreateResponse(HttpStatusCode.BadRequest, "Missing required params");

            var existingPlayerScore = (await table.ExecuteAsync(TableOperation.Retrieve<Score>("HoloLens", data.PlayerName))).Result as Score;
            if (existingPlayerScore != null && existingPlayerScore.PlayerScore >= data.PlayerScore)
                return req.CreateResponse(HttpStatusCode.OK);

            var tableOperation = TableOperation.InsertOrReplace(data);
            await table.ExecuteAsync(tableOperation);

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
