using System;
using System.Collections.Generic;
using System.Text;

namespace Bagel.BI.Olap
{
    public class DatabaseRuntimeObject : NamedObject
    {
        public DatabaseRuntimeObject(string name) : base(name) { }

        internal DatabaseRuntime Database { get; set; }
    }
}
