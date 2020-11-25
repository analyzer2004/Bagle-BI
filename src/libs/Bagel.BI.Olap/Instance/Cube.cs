using System;
using System.Collections.Generic;
using System.Text;

namespace Bagel.BI.Olap
{
    public class Cube : DatabaseRuntimeObject
    {
        public Cube(string name) : base(name) { }

        public Dimension this[string name]
        {
            get { return Dimensions[name]; }
        }

        public NamedObjectCollection<Dimension> Dimensions { get; } = new NamedObjectCollection<Dimension>();
        public NamedObjectCollection<Measure> Measures { get; } = new NamedObjectCollection<Measure>();

        public static Cube Empty
        {
            get { return new Cube(string.Empty); }
        }
    }
}
