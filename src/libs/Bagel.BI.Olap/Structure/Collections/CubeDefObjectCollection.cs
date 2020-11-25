using System;
using System.Collections.Generic;
using System.Text;

namespace Bagel.BI.Olap
{
    public class CubeDefObjectCollection<T> : NamedObjectCollection<T> where T : CubeDefObject
    {
        public CubeDefObjectCollection(CubeDef cube) : base()
        {
            Cube = cube;
        }

        public CubeDef Cube { get; }

        public new void Add(T item)
        {
            item.Cube = Cube;
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