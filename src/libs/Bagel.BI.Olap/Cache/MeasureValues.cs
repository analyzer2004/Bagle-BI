using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bagel.BI.Olap
{
    public class MeasureValues
    {
        public MeasureValues(Measure measure)
        {
            Measure = measure;
        }

        public Measure Measure { get; set;  } = null;
        public List<object> Values { get; } = new List<object>();
    }
}
