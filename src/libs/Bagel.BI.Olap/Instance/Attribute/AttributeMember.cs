using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace Bagel.BI.Olap
{
    public class AttributeMember : IQueryMember, IMember
    {
        public AttributeMember()
        {

        }

        public AttributeMember(object key)
        {
            Key = key;
            Name = key != null ? key.ToString() : "";
        }
        
        public AttributeMember(object key, string name)
        {
            Key = key;
            Name = name;
        }

        [JsonIgnore()]
        public AttributeDef AttributeDef 
        {
            get { return Attribute.Definition; }
        }

        [JsonIgnore()]
        public Attribute Attribute { get; set; }        

        public object Key { get; set; }
        public string Name { get; set; }
        [JsonIgnore()]
        public List<IMember> Children { get; } = new List<IMember>();
        [JsonIgnore()]
        public Measure LinkedMeasure { get; set; } = null;
        [JsonIgnore()]
        public bool IsTotal { get; set; } = false;
    }
}
