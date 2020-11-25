using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bagel.BI.Olap
{
    public class Cell
    {
        [JsonConstructor()]
        public Cell(object value)
        {
            Value = value;
        }

        public Cell(object value, Type dataType)
        {
            Value = value;
            DataType = dataType;
        }

        [JsonIgnore()]
        public Type DataType { get; }
        public object Value { get; set; }
    }
}
