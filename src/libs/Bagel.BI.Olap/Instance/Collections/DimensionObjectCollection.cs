using System;
using System.Collections.Generic;
using System.Text;

namespace Bagel.BI.Olap
{
    public class DimensionObjectCollection<T>: NamedObjectCollection<T> where T : DimensionObject
    {
        public DimensionObjectCollection() { }

        public DimensionObjectCollection(Dimension dimension) : base()
        {
            Dimension = dimension;
        }

        public Dimension Dimension { get; }
        public new void Add(T item)
        {
            item.Dimension = Dimension;
            base.Add(item);
        }

        public new void AddRange(IEnumerable<T> collection)
        {
            if (collection != null)
                foreach (T item in collection)
                    this.Add(item);
        }
    }
}
