using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bagel.BI.Olap
{
    public class AttributeDefCollection : List<AttributeDef>
    {
        public AttributeDefCollection(DimensionDef dimension) : base ()
        {
            Dimension = dimension;
        }

        public DimensionDef Dimension { get; }

        public AttributeDef this[string name]
        {
            get
            {
                foreach (AttributeDef attr in this)
                {
                    if (string.Compare(attr.Name, name) == 0)
                        return attr;
                }
                throw new IndexNotFoundException(name);
            }
        }

        public new void Add(AttributeDef item)
        {
            item.Dimension = Dimension;
            base.Add(item);
        }

        public new void AddRange(IEnumerable<AttributeDef> collection)
        {
            if (collection != null)
                foreach (AttributeDef item in collection)
                    this.Add(item);
        }
    }
}
