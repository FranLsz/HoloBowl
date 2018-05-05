using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HoloBowlFn.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace HoloBowlFn.Functions
{
    public static class GetScores
    {
        [FunctionName("GetScores")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequestMessage req,
            [Table("Score")] IQueryable<Score> scores,
            TraceWriter log)
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(scores), Encoding.UTF8, "application/json")
            };
        }
    }
}
