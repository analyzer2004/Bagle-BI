using System;
using System.Collections.Generic;
using System.Text;

namespace Bagel.BI.Olap
{
    public interface INamedObject
    {
        string Name { get; set; }
    }
}
