using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bagel.BI.Olap
{
    public class Database : NamedObject
    {
        public Database(string name) : base(name) 
        {
            DataSources = new DatabaseObjectCollection<DataSource>(this);
            DataSourceViews = new DatabaseObjectCollection<DataSourceView>(this);
            Cubes = new DatabaseObjectCollection<CubeDef>(this);
            Dimensions = new DatabaseObjectCollection<DimensionDef>(this);
            Runtime = new DatabaseRuntime(this);
        }

        public DatabaseRuntime Runtime { get; }
        public DatabaseObjectCollection<DataSource> DataSources { get; }
        public DatabaseObjectCollection<DataSourceView> DataSourceViews { get; }
        public DatabaseObjectCollection<CubeDef> Cubes { get; }
        public DatabaseObjectCollection<DimensionDef> Dimensions { get; }
    }
}
