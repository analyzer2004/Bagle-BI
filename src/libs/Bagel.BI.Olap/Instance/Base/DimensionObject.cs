using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bagel.BI.Olap
{
    public class DimensionObject : NamedObject
    {
        public DimensionObject(string name) : base(name) { }

        [JsonIgnore()]
        public Dimension Dimension { get; set; }
    }
}
