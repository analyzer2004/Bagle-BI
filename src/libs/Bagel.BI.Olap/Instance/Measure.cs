using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bagel.BI.Olap
{
    public class Measure : NamedObject, IQueryMember
    {
        public Measure(MeasureDef definition, string name) : base(name) 
        {
            Definition = definition;
        }

        [JsonIgnore()]
        public object Key { get; set; }
        internal MeasureDef Definition { get; }
    }
}
