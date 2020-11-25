using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Runtime.CompilerServices;

namespace Bagel.BI.Olap
{
    public class QueryContext
    {
        public QueryContext(Cube cube, IDbConnection connection)
        {
            Cube = cube;
            _connection = connection;
        }
        
        private IDbConnection _connection = null;

        public Cube Cube { get; }

        public Dimension this[string name]
        {
            get { return Cube[name]; }
        }
        
        public NamedObjectCollection<Measure> Measures 
        { 
            get { return Cube.Measures; }
        }

        public CellSet Execute(QueryCommand cmd)
        {
            Query qry = new Query(Cube.Database.Runtime.Pool, _connection);
            return qry.Execute(cmd);
        }

        public void Close()
        {
            if (_connection != null && _connection.State != ConnectionState.Closed)
            {
                _connection.Close();
            }
        }
    }
}
