using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bagel.BI.Olap
{
    public class DimensionDef : DatabaseObject
    {
        public DimensionDef(string name, AttributeDef[] attributes) : this(name, attributes, null)
        {            
        }

        public DimensionDef(string name, AttributeDef[] attributes, HierarchyDef[] hierarchies) : base(name)
        {
            Attributes = new DimensionDefObjectCollection<AttributeDef>(this);
            Hierarchies = new DimensionDefObjectCollection<HierarchyDef>(this);
            Attributes.AddRange(attributes);
            Hierarchies.AddRange(hierarchies);
        }

        private string _name = string.Empty;
        public override string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                if (string.IsNullOrEmpty(AttributeAllMemberName))
                    AttributeAllMemberName = "All " + _name + "s";
            }
        }

        public CubeDef Cube { get; set; }
        public string AttributeAllMemberName { get; set; } = string.Empty;
        public DimensionDefObjectCollection<AttributeDef> Attributes { get; }
        public DimensionDefObjectCollection<HierarchyDef> Hierarchies { get; }
    }
}
