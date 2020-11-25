using System;
using System.Collections.Generic;
using System.Text;

namespace Bagel.BI.Olap
{
    public interface ILevel : INamedObject
    {
        IMemberCollection Members { get; }
    }
}
