using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Bagel.BI.Olap
{
    public class AttributeLevel : NamedObject, ILevel
    {
        public AttributeLevel(string name) : base(name) 
        {
            Members = new AttributeMemberCollection(Attribute);
        }
        
        private Attribute _attribute = null;

        [JsonIgnore()]
        public Attribute Attribute 
        {
            get { return _attribute; }
            set
            {
                _attribute = value;
                ((AttributeMemberCollection)Members).Attribute = value;
            }
        }
        [JsonIgnore()]
        public IMemberCollection Members { get; }

    }
}
