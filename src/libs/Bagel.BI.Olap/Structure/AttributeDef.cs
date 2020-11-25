using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bagel.BI.Olap
{
    public enum AttributeOrderBy
    {
        Key,
        Name
    }

    public class AttributeDef : DimensionDefObject, IDataElement
    {        
        public AttributeDef(string name, Column column) : this(name, column, column) { }
        public AttributeDef(string name, Column keyColumn, Column nameColumn) : this(name, new Column[] { keyColumn }, nameColumn) { }
        public AttributeDef(string name, Column[] keyColumns, Column nameColumn) : base(name)
        {
            KeyColumns.AddRange(keyColumns);
            NameColumn = nameColumn;
        }
        
        public AttributeOrderBy OrderBy { get; set; } = AttributeOrderBy.Name;
        public SourceColumnCollection KeyColumns { get; set; } = new SourceColumnCollection();
        public Column NameColumn { get; set; }
        public bool KeyIsName
        {
            get { return KeyColumns[0] == NameColumn; }
        }
    }
}
