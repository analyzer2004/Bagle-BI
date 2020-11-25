using System;
using System.Collections.Generic;
using System.Text;

namespace Bagel.BI.Olap
{
    public class HierarchyDef : DimensionDefObject, IDataElement
    {
        public HierarchyDef(string name) : this(name, null) { }

        public HierarchyDef(string name, LevelDef[] levels) : base(name)
        {
            Levels = new LevelDefCollection(this);
            Levels.AddRange(levels);
        }

        public LevelDefCollection Levels { get; }
    }
}
