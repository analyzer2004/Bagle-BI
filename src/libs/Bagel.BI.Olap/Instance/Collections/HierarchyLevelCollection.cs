using System;
using System.Collections.Generic;
using System.Text;

namespace Bagel.BI.Olap
{
    public class HierarchyLevelCollection : NamedObjectCollection<ILevel>, ILevelCollection
    {
        public HierarchyLevelCollection() { }

        public HierarchyLevelCollection(Hierarchy hierarchy) : base()
        {
            Hierarchy = hierarchy;
        }

        public Hierarchy Hierarchy { get; }

        public new void Add(ILevel item)
        {
            ((HierarchyLevel)item).Hierarchy = Hierarchy;
            base.Add(item);
        }

        public new void AddRange(IEnumerable<ILevel> collection)
        {
            if (collection != null)
                foreach (HierarchyLevel item in collection)
                    this.Add(item);
        }
    }
}
