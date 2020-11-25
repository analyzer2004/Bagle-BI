using System;
using System.Collections.Generic;
using System.Text;

namespace Bagel.BI.Olap
{
    public class DatabaseRuntimeObjectCollection<T> : NamedObjectCollection<T> where T : DatabaseRuntimeObject
    {
        public DatabaseRuntimeObjectCollection(DatabaseRuntime database) : base()
        {
            Database = database;
        }

        public DatabaseRuntime Database { get; }

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
