using System.Collections.Generic;

namespace Bagel.BI.Olap
{
    public class ColumnCollection : NamedObjectCollection<Column>
    {
        public ColumnCollection(Table table) : base()
        {
            Table = table;
        }

        public Table Table { get; private set; }
        public new void Add(Column item)
        {
            item.Table = Table;
            base.Add(item);
        }

        public new void AddRange(IEnumerable<Column> collection)
        {
            if (collection != null)
                foreach (Column item in collection)
                    this.Add(item);
        }
    }
}
