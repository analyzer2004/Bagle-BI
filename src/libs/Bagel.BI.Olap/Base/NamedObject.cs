using System;
using System.Collections.Generic;
using System.Text;

namespace Bagel.BI.Olap
{
    public class NamedObject : INamedObject
    {
        public NamedObject(string name)
        {
            Name = name;
        }

        public virtual string Name { get; set; }
    }
}
