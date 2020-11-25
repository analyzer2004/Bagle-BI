using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bagel.BI.Olap
{
    public class AttributeMemberCollection : MemberCollection
    {
        public AttributeMemberCollection(Attribute attr) : base()
        {
            Attribute = attr;
        }

        public Attribute Attribute { get; internal set;  }

        public override void Add(IMember item)
        {             
            ((AttributeMember)item).Attribute = Attribute;
            base.Add(item);            
        }
    }
}
