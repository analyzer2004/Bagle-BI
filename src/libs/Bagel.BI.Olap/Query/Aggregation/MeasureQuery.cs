using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Bagel.BI.Olap
{
    class MeasureQuery : BaseQuery
    {
        public MeasureQuery(IDbConnection connection) : base(connection) { }

        public MeasureDef Measure { get; set; }

        private string GetSql()
        {
            return string.Format(
                "select {0}([{1}]) from [{2}]",
                Measure.Aggregation.ToString(),
                Measure.Source.Name,
                Measure.Source.Table.Name
                );
        }

        public object QueryValue()
        {
            return ExecuteScaler(GetSql());
        }

    }
}
