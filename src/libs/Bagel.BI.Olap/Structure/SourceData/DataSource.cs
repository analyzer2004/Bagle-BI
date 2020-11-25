using System.Data.SqlClient;

namespace Bagel.BI.Olap
{
    public class DataSource : DatabaseObject
    {
        public DataSource(string name, string connString) : base(name)
        {
            ConnectionString = connString;
        }

        public string ConnectionString { get; set; }

        public SqlConnection GetConnection()
        {
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = ConnectionString;
            return cn;
        }
    }
}
