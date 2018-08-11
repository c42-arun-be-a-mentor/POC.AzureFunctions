using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Data.Entity;
using System;

namespace AzureFunctions
{
    public static class StreetAddressFunction
    {
        [FunctionName("Street")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            // parse query parameter
            string name = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
                .Value;

            if (name == null)
            {
                // Get request body
                dynamic data = await req.Content.ReadAsAsync<object>();
                name = data?.name;
            }

            if (name == null)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body");
            }

            var streetName = new DbAccess().GetStreetAddress(name, log);

            if (string.IsNullOrWhiteSpace(streetName))
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Name not found in database");
            }

            return req.CreateResponse(HttpStatusCode.OK, string.Format("Street name is {0}", streetName));
        }
    }
}
