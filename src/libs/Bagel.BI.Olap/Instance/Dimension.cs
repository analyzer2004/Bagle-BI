using System;
using System.Collections.Generic;
using System.Text;

namespace Bagel.BI.Olap
{
    public class Dimension : DatabaseRuntimeObject
    {
        public Dimension(string name) : base(name) 
        {
            Attributes = new DimensionObjectCollection<Attribute>(this);
            Hierarchies = new DimensionObjectCollection<Hierarchy>(this);
        }

        public IHierarchical this[string name]
        {
            get 
            { 
                Attribute a = Attributes[name];
                if (a == null)
                    return Hierarchies[name];
                else
                    return a;
            }
        }

        public DimensionObjectCollection<Attribute> Attributes { get; }
        public DimensionObjectCollection<Hierarchy> Hierarchies { get; }
    }
}
