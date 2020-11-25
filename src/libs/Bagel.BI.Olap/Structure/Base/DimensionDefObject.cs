using System;
using System.Collections.Generic;
using System.Text;

namespace Bagel.BI.Olap
{
    public class DimensionDefObject : NamedObject
    {
        public DimensionDefObject(string name) : base(name) { }

        public DimensionDef Dimension { get; set; }
    }
}
