using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Bagel.BI.Olap
{
    public class Query
    {
        public Query(AggregationPool pool, IDbConnection connection)
        {
            _pool = pool;
            _connection = connection;
        }

        private AggregationPool _pool = null;
        private IDbConnection _connection = null;

        public CellSet Execute(QueryCommand cmd)
        {
            Joiner joiner = new Joiner(_pool, _connection, cmd);
            joiner.Run();

            if (_pool.Aggregations.Count == 0) return new CellSet();

            CellSet cs = new CellSet();
            DateTime t = DateTime.Now;

            QuerySets.JoinedMembers columns = cmd.Columns.JoinMembers(_pool.FindAggregation(cmd.Columns.Attributes));
            QuerySets.JoinedMembers rows = cmd.Rows.Count > 0 ? 
                cmd.Rows.JoinMembers(_pool.FindAggregation(cmd.Rows.Attributes)) : 
                new QuerySets.JoinedMembers();

            QuerySets.JoinedMembers filters;
            bool reverseFilter = cmd.Filters.ShouldReverse;            
            if (!reverseFilter)
            {
                filters = cmd.Filters.Count > 0 ? 
                    cmd.Filters.JoinMembers(_pool.FindAggregation(cmd.Filters.Attributes), true) : 
                    new QuerySets.JoinedMembers();
            }
            else
            {
                reverseFilter = true;
                QuerySets reversed = cmd.Filters.ReverseMembers();
                filters = reversed.Count > 0 ? 
                    reversed.JoinMembers(_pool.FindAggregation(reversed.Attributes), true) : 
                    new QuerySets.JoinedMembers();
            }

            // AggregationPointer is for total lookup
            AggregationPointer root = null;
            Aggregation defaultAggregation = _pool.FindAggregation(GetAttributes());
            if (defaultAggregation != null)
            {
                List<Position> cps = cs.Axes[0].Positions;
                List<Position> rps = cs.Axes[1].Positions;
                for (int i = 0; i < rows.Count; i++) rps.Add(new Position(rows[i], i));
                for (int i = 0; i < columns.Count; i++) cps.Add(new Position(columns[i], i));

                if (filters.Count > 0)
                {
                    if (reverseFilter)
                    {
                        // Query values without filters first
                        Attribute[] crIndex = cmd.Columns.Attributes.Concat(cmd.Rows.Attributes).ToArray();
                        DirectQuery(_pool.FindAggregation(crIndex));

                        // Then query values with filter
                        if (root != null) root.Clear();
                        root = null;
                        IEnumerable<Cell> filtered = IncrementalQuery(defaultAggregation);

                        // Substract
                        int i = 0;
                        foreach (Cell cell in filtered)
                        {
                            if (cell.Value != null)
                            {
                                if (cell.DataType == typeof(int))
                                    cs.Cells[i].Value = (int)cs.Cells[i].Value - (int)cell.Value;
                                else
                                    cs.Cells[i].Value = (decimal)cs.Cells[i].Value - (decimal)cell.Value;
                            }
                            i++;
                        }
                    }
                    else
                    {
                        IncrementalQuery(defaultAggregation, cs.Cells);
                    }
                }
                else
                {
                    DirectQuery(defaultAggregation);
                }
            }

            cs.QueryTime = (DateTime.Now - t).TotalMilliseconds;
            return cs;

            Attribute[] GetAttributes()
            {
                List<Attribute> attrs = new List<Attribute>();
                attrs.AddRange(filters.Attributes);
                attrs.AddRange(columns.Attributes);
                attrs.AddRange(rows.Attributes);                
                return attrs.ToArray();
            }

            IEnumerable<Cell> IncrementalQuery(Aggregation agg, List<Cell> cells = null)
            {
                List<CellAggregator> cas = new List<CellAggregator>();
                if (rows.Count == 0)
                {
                    for (int i = 0; i < filters.Count; i++)
                    {
                        int pointer = 0;
                        for (int j = 0; j < columns.Count; j++)
                        {
                            if (i == 0)
                            {
                                cas.Add(new CellAggregator(GetValueInfo(agg.GetValue(false, filters[i], columns[j]))));
                            }
                            else
                            {
                                cas[pointer].Add(GetValueInfo(agg.GetValue(false, filters[i], columns[j])));
                                pointer++;
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < filters.Count; i++)
                    {
                        int pointer = 0;
                        for (int j = 0; j < rows.Count; j++)
                        {
                            for (int k = 0; k < columns.Count; k++)
                            {
                                if (i == 0)
                                {
                                    cas.Add(new CellAggregator(GetValueInfo(agg.GetValue(false, filters[i], columns[k], rows[j]))));
                                }
                                else
                                {
                                    cas[pointer].Add(GetValueInfo(agg.GetValue(false, filters[i], columns[k], rows[j])));
                                    pointer++;
                                }
                            }
                        }
                    }
                }
                //cas.ForEach(_ => cs.Cells.Add(new Cell(_.Calculate())));
                if (cells != null)
                {
                    cas.ForEach(_ => cells.Add(new Cell(_.Calculate())));
                    return null;
                }
                else
                    return cas.Select(_ => new Cell(_.Calculate(), _.DataType));
            }

            void DirectQuery(Aggregation agg)
            {
                if (rows.Count == 0)
                {
                    for (int i = 0; i < columns.Count; i++)
                    {
                        cs.Cells.Add(new Cell(GetValueInfo(agg.GetValue(false, columns[i])).Value));
                    }
                }
                else
                {
                    for (int i = 0; i < rows.Count; i++)
                    {
                        for (int j = 0; j < columns.Count; j++)
                        {
                            cs.Cells.Add(new Cell(GetValueInfo(agg.GetValue(false, columns[j], rows[i])).Value));
                        }
                    }
                }
            }

            Aggregation.ValueInfo GetValueInfo(Aggregation.ValueInfo vinfo)
            {
                // vinfo.IsTotal -> Place holder for total
                // AggregationPointer -> Point to total aggregation indexed by total place holder's attributes
                if (vinfo.IsTotal)
                {
                    // Find or create AggregationPointer based on total place holder's attributes
                    int c = 0;
                    AggregationPointer cp = root;
                    foreach (AttributeMember[] members in vinfo.Members)
                    {
                        foreach (AttributeMember m in members)
                        {
                            if (!m.IsTotal && m.LinkedMeasure == null)
                            {
                                if (c == 0)
                                {
                                    if (root == null) root = new AggregationPointer(m.Attribute);
                                    cp = root;
                                }
                                else
                                {
                                    AggregationPointer p = cp[m.Attribute];
                                    if (p == null)
                                    {
                                        p = new AggregationPointer(cp, m.Attribute);
                                        cp.Children.Add(p);
                                    }
                                    cp = p;
                                }
                                c++;
                            }
                        }
                    }

                    // Find and assign total aggregation if it is a new pointer (cp.Aggregation == null)
                    if (cp.Aggregation == null) cp.Aggregation = _pool.FindAggregation(cp.Attributes);

                    return cp.Aggregation.GetValue(true, vinfo.Members);
                }
                else
                {
                    return vinfo;
                }
            }
        }
    }

    /// <summary>
    /// Pointer tree data structure for total aggregation lookup 
    /// </summary>
    public class AggregationPointer
    {
        public AggregationPointer(Attribute attribute) : this(null, attribute) { }
        public AggregationPointer(AggregationPointer parent, Attribute attribute)
        {
            Parent = parent;
            Attribute = attribute;
        }

        public AggregationPointer this[Attribute attribute]
        {
            get
            {
                foreach (AggregationPointer p in Children)
                    if (p.Attribute == attribute)
                        return p;
                return null;
            }
        }

        public AggregationPointer Parent;
        public Attribute Attribute;
        public List<AggregationPointer> Children { get; } = new List<AggregationPointer>();
        public Aggregation Aggregation;
        public Attribute[] Attributes
        {
            get
            {
                List<Attribute> attrs = new List<Attribute>();                
                AggregationPointer ap = this;
                while(ap != null)
                {
                    attrs.Add(ap.Attribute);
                    ap = ap.Parent;
                }

                attrs.Reverse();
                return attrs.ToArray();
            }
        }

        public void Clear()
        {
            Children.ForEach(Walkthrough);
            void Walkthrough(AggregationPointer ap)
            {
                ap.Children.ForEach(Walkthrough);
                ap.Children.Clear();
            }
            Children.Clear();
        }
    }
}