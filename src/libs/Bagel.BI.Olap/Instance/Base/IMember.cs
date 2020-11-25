using System;
using System.Collections.Generic;
using System.Text;

namespace Bagel.BI.Olap
{
    public interface IMember : IQueryMember
    {
        Attribute Attribute { get; }
        List<IMember> Children { get; }
    }
}
