using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bagel.BI.Olap
{
    public class Column : NamedObject
    {
        public Column(string name, Type dataType) : base(name)
        {            
            DataType = dataType;
        }

        public Type DataType { get; set; }

        public Table Table { get; set; }

        public string QualifiedName
        {
            get
            {
                if (!string.IsNullOrEmpty(Table.Alias))
                    return string.Format("{0}.[{1}]", Table.Alias, Name);
                else
                    return string.Format("[{0}]", Name);
            }
        }
    }
}
