using System;
using System.Collections.Generic;
using System.Text;

namespace Bagel.BI.Olap
{
    public class HierarchyMemberCollection : MemberCollection
    {
        public HierarchyMemberCollection(Hierarchy hierarchy) : base()
        {
            Hierarchy = hierarchy;
        }

        public Hierarchy Hierarchy { get; internal set; }        

        public override void Add(IMember item)
        {
            ((HierarchyMember)item).Hierarchy = Hierarchy;
            base.Add(item);            
        }
    }
}
