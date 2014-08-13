using osm2mssql.Importer.Enums;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osm2mssql.Importer.ViewModel
{
    class SqlDbAvailableChecker
    {
        internal async Task<ConnectionResult> CheckDatabaseAvailability(System.Data.SqlClient.SqlConnectionStringBuilder sqlConnectionStringBuilder)
        {
            try
            {
                var con = new SqlConnectionStringBuilder(sqlConnectionStringBuilder.ToString());
                con.InitialCatalog = string.Empty; //Remove Initial Catalog and try to connect to database
                using (var connection = new SqlConnection(con.ToString()))
                {
                    await connection.OpenAsync();
                }

                try
                {
                    //Now use original Connection String and try to connect to existing Database
                    using (var connection = new SqlConnection(sqlConnectionStringBuilder.ToString()))
                    {
                        await connection.OpenAsync();
                    }
                    return ConnectionResult.DbAlreadyExists;
                }
                catch
                {
                    return ConnectionResult.Successful;
                }
            }
            catch
            {
                return ConnectionResult.Error;
            }
        }
    }
}
