using System.Collections.Generic;
using System.Data;

namespace Bagel.BI.Olap
{
    public class NamedObjectCollection<T> : List<T> where T : INamedObject
    {
        public NamedObjectCollection() : this(false) { }

        public NamedObjectCollection(bool allowDuplicates = false)
        {
            AllowDuplicates = allowDuplicates;
            if (!AllowDuplicates) _hs = new HashSet<string>();
        }

        private HashSet<string> _hs = null;

        public T this[string name]
        {
            get 
            {
                return this.Find(_ => string.Compare(_.Name, name) == 0);
            }
        }
        public bool AllowDuplicates { get; }

        public new void Add(T item)
        {
            
            if (!AllowDuplicates)
            {
                if (!_hs.Contains(item.Name))
                {
                    _hs.Add(item.Name);
                    base.Add(item);
                }
                else
                    throw new DuplicateNameException(item.Name);
            }
            else
            {
                base.Add(item);
            }
        }

        public new void AddRange(IEnumerable<T> collection)
        {
            if (collection != null)
                foreach (T item in collection) this.Add(item);
        }
    }
}
