using System;
using System.Collections.Generic;
using System.Text;

namespace Bagel.BI.Olap
{
    public class TableCollection : NamedObjectCollection<Table>
    {
        public TableCollection() : base() { }

        public new void Add(Table item)
        {
            foreach (Table t in this)
                if (t.DataSource != item.DataSource)
                    throw new Exception(string.Format("{0} - Different DataSource.", item.Name));
            base.Add(item);
        }

        public new void AddRange(IEnumerable<Table> collection)
        {
            if (collection != null)
            {
                DataSource ds = null;
                foreach (Table t in collection)
                {
                    if (ds == null)
                        ds = t.DataSource;
                    else if (ds != t.DataSource)
                        throw new Exception("All tables must have the same DataSource.");
                }

                base.AddRange(collection);
            }
        }
    }
}
