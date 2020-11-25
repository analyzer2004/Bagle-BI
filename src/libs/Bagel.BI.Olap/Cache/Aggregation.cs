using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bagel.BI.Olap
{
    public class Aggregation
    {
        public Aggregation(Attribute[] index)
        {
            Index = index;
        }

        public Aggregation(Attribute[] index, Measure[] measures)
        {
            Index = index;
            foreach(Measure measure in measures)
            {
                Values.Add(new MeasureValues(measure));
            }
        }

        public struct ValueInfo
        {
            public ValueInfo(object value, Measure measure) : this(value, measure, false, null) { }
            public ValueInfo(object value, Measure measure, bool isTotal, AttributeMember[][] members)
            {
                Value = value;
                Measure = measure;
                IsTotal = isTotal;
                Members = members;
            }

            public object Value;
            public Measure Measure;
            public bool IsTotal;
            public AttributeMember[][] Members;
        }

        public MeasureValues this[Measure measure]
        {
            get
            {
                foreach(MeasureValues mvs in Values)
                {
                    if (mvs.Measure == measure)
                        return mvs;
                }
                return null;
            }
        }
        public double QueryTime { get; set; } = 0;

        public Attribute[] Index { get; }
        public List<AttributeMember[]> Tuples { get; } = new List<AttributeMember[]>();
        public List<MeasureValues> Values { get; } = new List<MeasureValues>();
        public List<int> TotalIndices { get; } = new List<int>();
        public IndexTree IndexTree { get; } = new IndexTree();
        public string Name
        {
            get { return string.Join("-", Index.Select(_ => _.Name)); }
        }

        public bool Match(Attribute[] index, bool absolute)
        {
            if (Index.Length == 0 || index.Length != Index.Length)
                return false;
            else
            {
                if (absolute)
                {
                    for (int i = 0; i < index.Length; i++)
                    {
                        if (Index[i] != index[i])
                            return false;
                    }
                    return true;
                }
                else
                {
                    int count = 0;
                    foreach (Attribute attr in index)
                    {
                        if (Index.Contains(attr)) count++;
                    }
                    return count == index.Length;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attrNames">Index - attribute names</param>
        /// <param name="absolute">Sequence must be the same if absolute is true</param>
        /// <returns></returns>
        public bool Match(string[] attrNames, bool absolute)
        {

            if (Index.Length == 0 || attrNames.Length != Index.Length)
                return false;
            else
            {
                if (absolute)
                {
                    for (int i = 0; i < attrNames.Length; i++)
                    {
                        if (string.Compare(Index[i].Name, attrNames[i], true) != 0)
                            return false;
                    }
                    return true;
                }
                else
                {
                    int count = 0;
                    foreach (string name in attrNames)
                    {
                        if (Index.Any(_ => string.Compare(_.Name, name, true) == 0)) count++;
                    }
                    return count == attrNames.Length;
                }
            }
        }

        public Measure[] MergeMeasures(Measure[] measures)
        {
            List<Measure> newMeasures = new List<Measure>();
            foreach(Measure m in measures)
            {
                if (this[m] == null)
                {
                    newMeasures.Add(m);
                    Values.Add(new MeasureValues(m));
                }
            }
            return newMeasures.ToArray();
        }

        public int GetIndex(IEnumerable<AttributeMember> members)
        {   
            int index = 0;
            IndexTreeNode cn = null;
            foreach (AttributeMember m in members)
            {
                if (index == 0)
                {
                    foreach (IndexTreeNode n in IndexTree.Nodes)
                    {
                        if (n.Member == m)
                        {
                            cn = n;
                            break;
                        }
                        else
                            cn = null;
                    }
                    index++;
                }
                else if (cn != null)
                {
                    foreach (IndexTreeNode n in cn.Children)
                    {
                        if (n.Member == m)
                        {
                            cn = n;
                            break;
                        }
                        else
                            cn = null;
                    }
                    index++;
                }
            }

            if (cn != null)
                return cn.Index;
            else
                return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skipTotal">Tells GetValue should return a total place holder if tuple is a total</param>
        /// <param name="members"></param>
        /// <returns></returns>
        public ValueInfo GetValue(bool skipTotal, params AttributeMember[][] members)
        {
            int index = 0;
            Measure measure = null;
            IndexTreeNode cn = null;
            foreach (AttributeMember[] curr in members)
            {
                foreach (AttributeMember m in curr)
                {
                    // return a place holder for total 
                    if (m.IsTotal)
                    {                        
                        if (!skipTotal) return new ValueInfo(null, measure, true, members);
                    }
                    // measure
                    else if (m.LinkedMeasure != null)
                    {
                        measure = m.LinkedMeasure;
                    }         
                    // index == 0 -> first level, start searching
                    else if (index == 0)
                    {
                        foreach (IndexTreeNode n in IndexTree.Nodes)
                        {
                            if (n.Member == m)
                            {
                                cn = n;
                                break;
                            }
                            else
                                cn = null;
                        }
                        index++;
                    }
                    // keep searching
                    // cn.Index = rowIndex of the aggregation table
                    else if (cn != null)
                    {
                        foreach (IndexTreeNode n in cn.Children)
                        {
                            if (n.Member == m)
                            {
                                cn = n;
                                break;
                            }
                            else
                                cn = null;
                        }
                        index++;
                    }
                }
            }

            if (cn != null)
                return new ValueInfo(this[measure].Values[cn.Index], measure);
            else
                return new ValueInfo(null, measure);


            /*
             * Search by scan - do not delete this code
             * 
            Measure measure = null;
            int index = Tuples.FindIndex(curr =>
            {                
                foreach (Member m in members1)
                {
                    if (m.LinkedMeasure != null)
                        measure = m.LinkedMeasure;
                    else if (!curr.Contains(m))
                        return false;
                }

                foreach (Member m in members2)
                {
                    if (m.LinkedMeasure != null)
                        measure = m.LinkedMeasure;
                    else if (!curr.Contains(m))
                        return false;
                }

                return true;
            });

            if (index > -1)
                return this[measure].Values[index];
            else
                return null;
            */
        }

        public void QueryTotals(AggregationPool pool)
        {            
            int attrLen = Index.Length;
            List<Attribute> index = new List<Attribute>();
            for (int i = 0; i < attrLen - 1; i++)
            {
                index.Add(Index[i]);
                Aggregation agg = pool.FindAggregation(index.ToArray());
                if (agg != null)
                {
                    foreach(int row in TotalIndices)
                    {
                        AttributeMember[] tuple = Tuples[row];
                        if (!tuple[i].IsTotal && tuple[i + 1].IsTotal)
                        {
                            int targetIndex = agg.GetIndex(tuple.Take(i + 1));
                            if (targetIndex >= 0)
                            {
                                for (int j = 0; j < Values.Count; j++)
                                {
                                    Values[j].Values[row] = agg.Values[j].Values[targetIndex];
                                }
                            }
                        }
                    }
                }
            }

        }
    }
}
