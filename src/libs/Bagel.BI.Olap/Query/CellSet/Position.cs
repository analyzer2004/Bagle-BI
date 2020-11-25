using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bagel.BI.Olap
{
    public class Position
    {
        public Position(AttributeMember[] members, int ordinal)
        {
            Members = members;
            Ordinal = ordinal;
        }

        public AttributeMember[] Members { get; private set; }
        public int Ordinal { get; private set; }
    }
}
