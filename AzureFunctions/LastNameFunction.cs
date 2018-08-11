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
    public static class LastNameFunction
    {
        [FunctionName("LastName")]
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

            var lastName = new DbAccess().GetLastName(name, log);

            if (string.IsNullOrWhiteSpace(lastName))
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Name not found in database");
            }

            return req.CreateResponse(HttpStatusCode.OK, string.Format("Last name is {0}", lastName));
        }
    }
}
