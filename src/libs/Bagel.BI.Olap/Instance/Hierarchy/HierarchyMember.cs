using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Bagel.BI.Olap
{
    public class HierarchyMember : IQueryMember, IMember
    {
        public HierarchyMember(AttributeMember member)
        {
            Origin = member;
        }

        public HierarchyDef HierarchyDef
        {
            get { return Hierarchy.Definition; }
        }
        public AttributeMember Origin { get; }        
        public Hierarchy Hierarchy { get; set; }
        public Attribute Attribute
        {
            get { return Origin.Attribute; }
        }

        public object Key 
        {
            get { return Origin.Key; }
            set { }
        }
        public string Name
        {
            get { return Origin.Name; }
            set { }
        }
        public List<IMember> Children { get; } = new List<IMember>();

    }
}
