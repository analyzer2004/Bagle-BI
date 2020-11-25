using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Bagel.BI.Olap
{
    public class DatabaseRuntime : DatabaseObject
    {
        public DatabaseRuntime(Database database) : base(database.Name) 
        {
            Database = database;
            Cubes = new DatabaseRuntimeObjectCollection<Cube>(this);
            Dimensions = new DatabaseRuntimeObjectCollection<Dimension>(this);
        }

        [JsonIgnore()]
        public AggregationPool Pool { get; } = new AggregationPool();
        public DatabaseRuntimeObjectCollection<Cube> Cubes { get; }
        public DatabaseRuntimeObjectCollection<Dimension> Dimensions { get; }
    }
}
 