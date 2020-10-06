using System;
using System.Collections.Generic;
using System.Text;
using Dapper;

namespace Uplift.DataAccess.Data.Repository.IRepository
{
    public interface IStoredProcedureCall : IDisposable
    {
        IEnumerable<T> ReturnList<T>(string procedureName, DynamicParameters param = null);
        void ExecuteWithoutReturn(string procedureName, DynamicParameters param = null);
        T ExecuteReturnScalar<T>(string procedureName, DynamicParameters param = null);

    }
}
