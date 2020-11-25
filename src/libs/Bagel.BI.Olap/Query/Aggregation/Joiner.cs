using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;


namespace Bagel.BI.Olap
{
    public class Joiner : BaseQuery
    {
        public Joiner(AggregationPool pool, IDbConnection connection, QueryCommand cmd) : base(connection)
        {
            _pool = pool;
            _connection = connection;
            _cmd = cmd;
        }


        private AggregationPool _pool = null;
        private IDbConnection _connection = null;
        private QueryCommand _cmd = null;
        
        public void Run()
        {
            if (_connection != null)
            {
                try
                {
                    if (_connection.State != ConnectionState.Open) _connection.Open();
                    // Need to rename QueryAttributeMembers                    
                    List<Measure> measures = QueryMeasureValues();
                    if (measures.Count > 0)
                    {
                        PathFinder pf = new PathFinder(_cmd);
                        if (pf.Paths.Count > 0)
                        {   
                            List<Aggregation> aggs = new List<Aggregation>();
                            // mgroups = measures grouped by fact table
                            IEnumerable<IGrouping<Table, Measure>> mgroups = measures.GroupBy(_ => _.Definition.Source.Table, _ => _);

                            Aggregate(_cmd.Permutate(false));
                            //Aggregate(_cmd.Permutate(true));

                            aggs.ForEach(_ => _.QueryTotals(_pool));

                            void Aggregate(List<Attribute[]> indices)
                            {
                                foreach (Attribute[] index in indices)
                                {
                                    Aggregation agg = _pool.FindAggregation(index);
                                    // All key columns are from the same table
                                    IEnumerable<Table> dimTables = index.Select(_ => _.Definition.KeyColumns[0].Table);
                                    foreach (IGrouping<Table, Measure> mgroup in mgroups)
                                    {
                                        Aggregator ag = new Aggregator(
                                            _connection,
                                            index,
                                            mgroup.ToArray(),
                                            pf.GetPath(dimTables, mgroup.Key)
                                            );

                                        if (agg == null)
                                        {
                                            agg = ag.Query();
                                            aggs.Add(agg);
                                            _pool.Aggregations.Add(agg);
                                        }
                                        else
                                        {
                                            if (ag.QueryMeasures(agg, agg.MergeMeasures(mgroup.ToArray())))
                                            {
                                                aggs.Add(agg);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        private List<Measure> QueryMeasureValues()
        {
            List<Measure> list = new List<Measure>();

            Measure[] measures = _cmd.Measures;
            foreach (Measure m in measures)
            {
                MeasureQuery mq = null;
                Aggregation root = _pool.GetRoot();
                if (root == null)
                {
                    root = new Aggregation(new Attribute[] { });
                    _pool.Aggregations.Add(root);
                }
                
                MeasureValues mvs = root[m];
                if (mvs == null || mvs.Values.Count == 0)
                {
                    if (mq == null) mq = new MeasureQuery(Connection);
                    mq.Measure = m.Definition;

                    if (mvs == null)
                    {
                        mvs = new MeasureValues(m);
                        root.Values.Add(mvs);
                    }

                    mvs.Values.Add(mq.QueryValue());
                }
                list.Add(m);
            }
            return list;
        }
    }
}
