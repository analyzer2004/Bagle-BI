using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bagel.BI.Olap
{
    public enum AggregationType
    {
        Sum,
        Average
    }

    public class MeasureDef : CubeDefObject, IDataElement//, IQueryMember
    {
        public MeasureDef(string name, Column source, AggregationType aggregation = AggregationType.Sum) : base(name)
        {            
            Source = source;
            Aggregation = aggregation;
        }        
        
        public Column Source { get; set; }
        public AggregationType Aggregation { get; set; } = AggregationType.Sum;
    }
}
