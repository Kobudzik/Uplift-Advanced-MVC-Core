using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Uplift.DataAccess.Data.Repository.IRepository;

namespace Uplift.DataAccess.Data.Repository
{
    public class StoredProcedureCal : IStoredProcedureCall
    {
        private readonly ApplicationDbContext _db;
        private static string ConnectionString = "";

        public StoredProcedureCal(ApplicationDbContext db)
        {
            _db = db;
            ConnectionString = db.Database.GetDbConnection().ConnectionString;
        }


        public void ExecuteWithoutReturn(string procedureName, DynamicParameters param = null)
        {
            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();
                sqlConnection.Execute(
                    procedureName,
                    param,
                    commandType: CommandType.StoredProcedure
                );
            }
        }

        public IEnumerable<T> ReturnList<T>(string procedureName, DynamicParameters param = null)
        {
            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();
                return sqlConnection.Query<T>(
                    procedureName,
                    param,
                    commandType: CommandType.StoredProcedure
                );
            }
        }

        public T ExecuteReturnScalar<T>(string procedureName, DynamicParameters param = null)
        {
            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();
                return (T)Convert.ChangeType(
                sqlConnection.ExecuteScalar<T>(
                        procedureName,
                        param,
                        commandType: CommandType.StoredProcedure
                    ),
                    typeof(T));
            }
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
