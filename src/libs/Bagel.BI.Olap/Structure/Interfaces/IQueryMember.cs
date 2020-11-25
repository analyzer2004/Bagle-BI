using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bagel.BI.Olap
{
    public interface IQueryMember
    {
        object Key { get; set; }
        string Name { get; set; }
    }
}
