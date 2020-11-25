using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Bagel.BI.Olap
{
    public class LevelDefCollection : NamedObjectCollection<LevelDef>
    {
        public LevelDefCollection(HierarchyDef hierarchy) : base()
        {
            Hierarchy = hierarchy;
        }

        public HierarchyDef Hierarchy { get; }

        public new void Add(LevelDef item)
        {
            item.Hierarchy = Hierarchy;
            base.Add(item);
        }

        public new void AddRange(IEnumerable<LevelDef> collection)
        {
            if (collection != null)
                foreach (LevelDef item in collection)
                    this.Add(item);
        }
    }
}
