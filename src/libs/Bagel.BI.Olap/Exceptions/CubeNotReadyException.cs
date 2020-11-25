using System;
using System.Collections.Generic;
using System.Text;

namespace Bagel.BI.Olap
{
    public class CubeNotReadyException : Exception
    {
        public CubeNotReadyException(CubeDef cube) :
            base(string.Format("Cube {0} is not ready.", cube.Name))
        { }
    }
}
