using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bagel.BI.Olap
{
    public class IndexNotFoundException : Exception
    {
        public IndexNotFoundException(object index) : 
            base(string.Format("{0} not found or out of range.", index))
        { }
    }
}
