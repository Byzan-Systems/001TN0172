using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace HDFCMSILWebMVC.Models
{
    public class service
    {
       
        public static DataTable getDetails(string Task, string Search1, string Search2, string Search3, string Search4, string Search5, string Search6, string Search7)
        {
            try
            {
                DataTable dataTable = new DataTable();
                //using var cmd = db.LoginMSTs.FromSqlRaw($"SP_FTPayment");
                using (var db = new Entities.DatabaseContext())
                {
                    DbConnection connection = db.Database.GetDbConnection();
                    using var cmd = db.Database.GetDbConnection().CreateCommand();
                    DbProviderFactory dbFactory = DbProviderFactories.GetFactory(connection);
                    cmd.CommandText = "SP_GetDetails";
                    //common
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (cmd.Connection.State != ConnectionState.Open) cmd.Connection.Open();
                    cmd.Parameters.Add(new SqlParameter("@Task", SqlDbType.VarChar) { Value = Task });
                    cmd.Parameters.Add(new SqlParameter("@Search1", SqlDbType.VarChar) { Value = Search1 });
                    cmd.Parameters.Add(new SqlParameter("@Search2", SqlDbType.VarChar) { Value = Search2 });
                    cmd.Parameters.Add(new SqlParameter("@Search3", SqlDbType.VarChar) { Value = Search3 });
                    cmd.Parameters.Add(new SqlParameter("@Search4", SqlDbType.VarChar) { Value = Search4 });
                    cmd.Parameters.Add(new SqlParameter("@Search5", SqlDbType.VarChar) { Value = Search5 });
                    cmd.Parameters.Add(new SqlParameter("@Search6", SqlDbType.VarChar) { Value = Search6 });
                    cmd.Parameters.Add(new SqlParameter("@Search7", SqlDbType.VarChar) { Value = Search7 });
                    //cmd.ExecuteReader();  
                    using (DbDataAdapter adapter = dbFactory.CreateDataAdapter())
                    {
                        adapter.SelectCommand = cmd;
                        adapter.Fill(dataTable);
                    }
                    cmd.Connection.Close();
                    return dataTable;
                }
                
            }
            catch (Exception ex) 
            {
                //clserr.WriteErrorToTxtFile(ex.Message + "Service", "getDetails", ""); return null;
                
                return null;
            }
        }
    }
}
