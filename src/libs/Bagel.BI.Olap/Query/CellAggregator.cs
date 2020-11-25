using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bagel.BI.Olap
{
    public class CellAggregator : List<Aggregation.ValueInfo>
    {
        public CellAggregator() { }
        public CellAggregator(Aggregation.ValueInfo vinfo)
        {
            this.Add(vinfo);
            MeasureDef md = vinfo.Measure.Definition;
            DataType = md.Source.DataType;
            Aggregation = md.Aggregation;
        }

        public Type DataType { get; }
        public AggregationType Aggregation { get; }

        public object Calculate()
        {
            this.RemoveAll(_ => _.Value == null);
            if (Count > 0)
            {   
                if (Aggregation == AggregationType.Sum)
                {
                    if (DataType == typeof(int))
                        return this.Sum(_ => (int)_.Value);
                    else 
                        return this.Sum(_ => (decimal)_.Value);
                }
                else if (Aggregation == AggregationType.Average)
                {
                    if (DataType == typeof(int))
                        return this.Average(_ => (int)_.Value);
                    else
                        return this.Average(_ => (decimal)_.Value);
                }
                else
                {
                    return this[0].Value;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
