using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bagel.BI.Olap
{
    public class QueryCommand
    {
        public QuerySets Columns { get; } = new QuerySets();
        public QuerySets Rows { get; } = new QuerySets();
        public QuerySets Filters { get; } = new QuerySets();

        public Attribute[] Attributes
        {
            get
            {
                List<Attribute> list = new List<Attribute>();
                list.AddRange(Filters.Attributes);
                list.AddRange(Columns.Attributes);
                list.AddRange(Rows.Attributes);                
                return list.ToArray();
            }
        }

        public Measure[] Measures
        {
            get
            {
                List<Measure> list = new List<Measure>();
                list.AddRange(Columns.Measures);
                list.AddRange(Rows.Measures);
                list.AddRange(Filters.Measures);
                return list.ToArray();
            }
        }

        public List<Attribute[]> Permutate(bool pivot)
        {
            Attribute[] cols = Columns.Attributes;
            Attribute[] rows = Rows.Attributes;
            Attribute[] filters = Filters.Attributes;

            int max = (new int[] { cols.Length, rows.Length, filters.Length }).Max();             
            List<Attribute> c = GetFilledList(pivot ? rows : cols, max);
            List<Attribute> r = GetFilledList(pivot ? cols : rows, max);
            List<Attribute> f = GetFilledList(filters, max);

            List<Attribute[]> combinations = new List<Attribute[]>();
            FirstPass();
            SecondPass();
            ThirdPass();
            ForthPass();
            return combinations;

            // Individuals
            void FirstPass()
            {
                c.ForEach(_ => { if (_ != null) combinations.Add(new Attribute[] { _ }); });
                r.ForEach(_ => { if (_ != null) combinations.Add(new Attribute[] { _ }); });
                f.ForEach(_ => { if (_ != null) combinations.Add(new Attribute[] { _ }); });
            }

            // Columns + Rows combinations
            void SecondPass()
            {
                List<Attribute> combo = new List<Attribute>();
                for (int i = 0; i < c.Count; i++)
                {
                    if (c[i] != null) combo.Add(c[i]);
                    for (int j = 0; j < r.Count; j++)
                    {
                        if (r[j] != null) combo.Add(r[j]);
                        AddToCombos(combo.ToArray());
                    }
                    combo.RemoveAll(_ => r.Contains(_));
                }
            }

            // Filters + Columns + Rows combinations
            void ThirdPass()
            {
                List<Attribute> combo = new List<Attribute>();

                for (int i = 0; i < f.Count; i++)
                {
                    if (f[i] != null) combo.Add(f[i]);
                    for (int j = 0; j < c.Count; j++)
                    {
                        if (c[j] != null) combo.Add(c[j]);
                        AddToCombos(combo.ToArray());
                        for (int k = 0; k < r.Count; k++)
                        {
                            if (r[k] != null) combo.Add(r[k]);
                            AddToCombos(combo.ToArray());
                        }
                        combo.RemoveAll(_ => r.Contains(_));
                    }
                    combo.RemoveAll(_ => c.Contains(_));
                }
            }

            // Individual axis combinations
            void ForthPass()
            {
                Columns.Permutate().ForEach(_ => AddToCombos(_));
                Rows.Permutate().ForEach(_ => AddToCombos(_));
                Filters.Permutate().ForEach(_ => AddToCombos(_));
            }

            void AddToCombos(Attribute[] attrs)
            {

                bool found = false;
                foreach (Attribute[] curr in combinations)
                {
                    if (curr.Length == attrs.Length)
                    {
                        bool match = true;
                        foreach (Attribute a in attrs)
                        {
                            if (!curr.Contains(a))
                            {
                                match = false;
                                break;
                            }
                        }

                        if (match)
                        {
                            found = true;
                            break;
                        }
                    }
                }

                if (!found || combinations.Count == 0) combinations.Add(attrs);
            }

            List<Attribute> GetFilledList(Attribute[] origin, int length)
            {
                List<Attribute> list = new List<Attribute>();
                foreach (Attribute attr in origin)
                {
                    list.Add(attr);
                }
                
                int count = list.Count;
                for (int i = 0; i < length - count; i++)
                {
                    list.Add(null);
                }
                return list;
            }
        }
    }
}
