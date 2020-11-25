using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bagel.BI.Olap
{
    public class Relationship
    {
        public Relationship(Table src, Table dest, Column srcColumn, Column destColumn)
        {
            Source = src;
            Destination = dest;
            SourceColumns.Add(srcColumn);
            DestinationColumns.Add(destColumn);
        }

        public Relationship(Table src, Table dest, string srcColumn, string destColumn)
        {
            Source = src;
            Destination = dest;

            Column srcCol = Source[srcColumn];
            Column destCol = Destination[destColumn];
            if (srcCol != null && destCol != null)
            {
                SourceColumns.Add(srcCol);
                DestinationColumns.Add(destCol);
            }
        }

        public Table Source { get; set; }
        public Table Destination { get; set; }
        public List<Column> SourceColumns { get; } = new List<Column>();
        public List<Column> DestinationColumns { get; } = new List<Column>();
    }
}
