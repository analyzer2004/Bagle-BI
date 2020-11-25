using System;
using System.Collections.Generic;
using System.Text;

namespace Bagel.BI.Olap
{
    public class LevelDef : NamedObject
    {
        public LevelDef(string name, AttributeDef attribute) : base(name)
        {
            Attribute = attribute;
        }

        public HierarchyDef Hierarchy { get; set; }
        public AttributeDef Attribute { get; }
    }
}
