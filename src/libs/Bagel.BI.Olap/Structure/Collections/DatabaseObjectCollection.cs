using System;
using System.Collections.Generic;
using System.Text;

namespace Bagel.BI.Olap
{
    public class DatabaseObjectCollection<T> : NamedObjectCollection<T> where T: DatabaseObject
    {
        public DatabaseObjectCollection(Database database) : base ()
        {
            Database = database;
        }

        public Database Database { get; }

        public new void Add(T item)
        {
            item.Database = Database;
            base.Add(item);
        }

        public new void AddRange(IEnumerable<T> collection)
        {
            if (collection != null)
                foreach (T item in collection)
                    this.Add(item);
        }

    }
}
