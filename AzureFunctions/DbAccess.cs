using Microsoft.Azure.WebJobs.Host;
using System;
using System.Data.Entity;
using System.Linq;

namespace AzureFunctions
{
    class DbAccess
    {
        string connectionString = "Server=<<server>>;Initial Catalog=<<db>>;"
                    + "Persist Security Info=False;User ID=<<username>>;Password=<<password>>;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public string GetLastName(string firstName, TraceWriter log)
        {
            return RunQuery("SELECT LastName FROM dbo.Customers WHERE FirstName = '{0}'", firstName, log);
        }

        public string GetStreetAddress(string firstName, TraceWriter log)
        {
            return RunQuery("SELECT Address_Street FROM dbo.Customers WHERE FirstName = '{0}'", firstName, log);
        }

        private string RunQuery(string query, string queryParam, TraceWriter log)
        {
            try
            {
                using (var context = new DbContext(connectionString))
                {
                    context.Database.Connection.Open();

                    var results = context.Database.SqlQuery<string>(
                            string.Format(query, queryParam))
                            .ToArray();

                    context.Database.Connection.Close();

                    if (results == null || results.Length == 0)
                    {
                        return string.Empty;
                    }

                    return results[0];
                }
            }
            catch (Exception ex)
            {
                log.Info(string.Format("Exception occurred {0}.", ex.Message));

                return string.Empty;
            }
        }
    }
}

