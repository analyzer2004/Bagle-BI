using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bagel.BI.Olap
{
    public class AggregationPool
    {
        private static Attribute[] _root = new Attribute[] { };

        public List<Aggregation> Aggregations { get; } = new List<Aggregation>();

        public Aggregation GetRoot()
        {
            foreach (Aggregation agg in Aggregations)
            {
                if (agg.Index.Length == 0) return agg;
            }
            return null;
        }
        
        public Aggregation FindAggregation(Attribute[] index, bool absolute = true)
        {
            foreach (Aggregation agg in Aggregations)
                if (agg.Match(index, absolute))
                    return agg;
            return null;
        }

        public Aggregation FindAggregation(string[] attrNames, bool absolute = true)
        {
            foreach (Aggregation agg in Aggregations)
                if (agg.Match(attrNames, absolute))
                    return agg;
            return null;
        }

        /*
        public MeasureValues GetValues(Measure measure)
        {
            return GetValues(new Attribute[] { }, measure);
        }

        public MeasureValues GetValues(Attribute[] index, Measure measure)
        {
            foreach(Aggregation agg in Aggregations)
            {
                if (agg.Match(index)) return agg[measure];
            }
            return null;
        }
        */
    }
}
