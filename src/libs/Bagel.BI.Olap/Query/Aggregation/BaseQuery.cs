using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Bagel.BI.Olap
{
    public class BaseQuery
    {
        public BaseQuery(IDbConnection connection)
        {
            Connection = connection;
        }

        public IDbConnection Connection { get; }

        protected IDbCommand GetCommand(string cmdText)
        {
            IDbCommand cmd = Connection.CreateCommand();
            cmd.CommandText = cmdText;
            return cmd;
        }

        protected IDataReader ExecuteReader(string cmdText)
        {
            return GetCommand(cmdText).ExecuteReader();
        }

        protected object ExecuteScaler(string cmdText)
        {
            return GetCommand(cmdText).ExecuteScalar();
        }

        protected void CloseReader(IDataReader reader)
        {
            if (reader != null && !reader.IsClosed)
            {
                reader.Close();
            }
        }
    }
}
