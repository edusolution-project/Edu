using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using System.Data.SqlClient;

namespace CoreEDB.Database
{
    public static class StaticGetConfig
    {
        public static string StrConnecting { get; private set; }
        public static IApplicationBuilder UseDynamicSql(this IApplicationBuilder app, IConfiguration configuration)
        {
            if (string.IsNullOrEmpty(StrConnecting))
            {
                var data = configuration.GetSection("Connection");
                 
                // Specify the provider name, server and database.
                string serverName = data["Server"];
                string databaseName = data["Database"];
                string userID = data["User"];
                string pass = data["Pass"];
                //Initialize the connection string builder for the
                //underlying provider.

               SqlConnectionStringBuilder sqlBuilder =
                   new SqlConnectionStringBuilder
                   {
                       UserID = userID,
                       Password = pass,
                        // Set the properties for the data source.
                        DataSource = serverName,
                       InitialCatalog = databaseName,
                       IntegratedSecurity = false
                   };
                StrConnecting = sqlBuilder.ToString();
                //// Build the SqlConnection connection string.
                //string providerString = sqlBuilder.ToString();

                //// Initialize the EntityConnectionStringBuilder.
                //EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder
                //{

                //    //Set the provider name.
                //    Provider = providerName,

                //    // Set the provider-specific connection string.
                //    ProviderConnectionString = providerString,

                //    // Set the Metadata location.
                //    Metadata = @"res://*/AdventureWorksModel.csdl|
                //            res://*/AdventureWorksModel.ssdl|
                //            res://*/AdventureWorksModel.msl"
                //};
                //StrConnecting = entityBuilder.ToString();
            }
            return app;
        }
    }
}
