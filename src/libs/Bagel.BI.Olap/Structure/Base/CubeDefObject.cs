using System;
using System.Collections.Generic;
using System.Text;

namespace Bagel.BI.Olap
{
    public class CubeDefObject : NamedObject
    {
        public CubeDefObject(string name) : base(name) { }

        public CubeDef Cube { get; set; }
    }
}
