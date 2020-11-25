using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bagel.BI.Olap
{
    public class HierarchyLevel : NamedObject, ILevel
    {
        public HierarchyLevel(string name) : base(name)
        {
            Members = new HierarchyMemberCollection(Hierarchy);
        }
        
        private Hierarchy _hierarchy = null;
        
        [JsonIgnore()]
        public Hierarchy Hierarchy 
        {
            get { return _hierarchy; }
            set
            {
                _hierarchy = value;
                ((HierarchyMemberCollection)Members).Hierarchy = value;
            }
        }
        [JsonIgnore()]
        public IMemberCollection Members { get; }
    }
}
