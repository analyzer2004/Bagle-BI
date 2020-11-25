using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bagel.BI.Olap
{
    public class Axis
    {
        public Axis(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public List<Position> Positions { get; } = new List<Position>();
    }
}
