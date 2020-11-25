using System;
using System.Collections.Generic;
using System.Text;

namespace Bagel.BI.Olap
{
    public class CubeDimensionDef : CubeDefObject
    {
        public CubeDimensionDef(DimensionDef dimension) : base(dimension.Name)
        {
            Dimension = dimension;            
        }        
        
        public DimensionDef Dimension { get; }   

        public DimensionDefObjectCollection<AttributeDef> Attributes { get { return Dimension.Attributes; } }
        public DimensionDefObjectCollection<HierarchyDef> Hierarchies { get { return Dimension.Hierarchies; } }
    }
}
