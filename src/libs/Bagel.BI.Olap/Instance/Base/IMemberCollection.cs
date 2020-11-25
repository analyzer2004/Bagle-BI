using System;
using System.Collections.Generic;
using System.Text;

namespace Bagel.BI.Olap
{
    public interface IMemberCollection : IList<IMember>
    {
        new IMember this[int index] { get; }
        IMember this[object key] { get; }
    }
}
