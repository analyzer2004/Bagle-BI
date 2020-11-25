using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace Bagel.BI.Olap
{
    public class QuerySets : List<QuerySet>
    {

        public QuerySets() : base() { }

        public QuerySets(IEnumerable<QuerySet> collection) : base(collection) { }

        public static QuerySets Empty
        {
            get { return new QuerySets(); }
        }

        public Measure[] Measures
        {
            get
            {
                List<Measure> list = new List<Measure>();
                this.ForEach(_ => {
                    Measure[] ms = _.Measures;
                    if (ms != null)
                    {
                        list.AddRange(ms);
                        return;
                    }
                });
                return list.ToArray();
            }
        }

        public Attribute[] Attributes
        {
            get
            {
                List<Attribute> list = new List<Attribute>();
                this.ForEach(_ => {
                    Attribute[] attrs = _.Attributes;
                    if (attrs != null) list.AddRange(attrs);
                });
                return list.ToArray();
            }
        }

        public bool ShouldReverse
        {
            get { return Rearrange().ShouldReverse; }
        }

        public List<Attribute[]> Permutate()
        {
            Attribute[] attrs = Attributes;
            List<Attribute[]> indices = new List<Attribute[]>();
            for (int i = 0; i < attrs.Length; i++)
            {
                for (int j = i; j < attrs.Length; j++)
                {
                    List<Attribute> index = new List<Attribute>();
                    for (int k = i; k <= j; k++)
                    {
                        index.Add(attrs[k]);
                    }
                    indices.Add(index.ToArray());
                }
            }
            return indices;
        }

        private RearrangedSets Rearrange()        
        {
            RearrangedSets sets = new RearrangedSets();
            Attribute measures = GetMeasuresAttribute();
            foreach(QuerySet set in this)
            {
                if (set.Attributes != null) sets.AddRange(set.GetQuerySets());
            }

            if (measures.Levels[0].Members.Count > 0)
            {
                sets.Add(new QuerySet(measures.Levels[0].Members));
            }

            return sets;
        }

        // Generates a pseudo Measures attribute
        // Each members represent a measure by using the LinkedMeasure property
        private Attribute GetMeasuresAttribute()
        {
            Attribute measures = new Attribute(null, "Measures");
            Measure[] ms = Measures;
            foreach (Measure m in ms)
            {
                AttributeMember mm = new AttributeMember(m.Name);
                mm.LinkedMeasure = m;
                measures.Levels[0].Members.Add(mm);
            }
            return measures;

        }

        public JoinedMembers JoinMembers(Aggregation agg, bool shouldCleanUp = false)
        {
            if (agg == null && Attributes.Length == 0)
            {
                if (Measures.Length > 0)
                {
                    Measure[] ms = Measures;
                    JoinedMembers result = new JoinedMembers();
                    foreach (Measure m in ms)
                    {
                        AttributeMember mm = new AttributeMember(m.Name);
                        mm.LinkedMeasure = m;
                        result.Add(new AttributeMember[] { mm });
                    }
                    return result;
                }
                else
                    return JoinedMembers.Empty;
            }
            else
            {
                RearrangedSets sets = Rearrange();
                if (shouldCleanUp) sets.CleanUp();

                // return empty if no sets left after cleaned up
                if (sets.Count == 0) return JoinedMembers.Empty;

                int depth = sets.Count;
                int[] indices = new int[depth];
                int[] ceilings = new int[depth];
                List<List<AttributeMember>> list = new List<List<AttributeMember>>();
                for (int i = 0; i < depth; i++)
                {
                    indices[i] = 0;
                    ceilings[i] = sets[i].Count;
                }

                Join(0);

                void Join(int level)
                {
                    for (int i = 0; i < ceilings[level]; i++)
                    {
                        indices[level] = i;
                        if (level == depth - 1)
                        {
                            List<AttributeMember> members = new List<AttributeMember>();
                            for (int j = 0; j < depth; j++)
                            {
                                IQueryMember member = sets[j][indices[j]];
                                members.Add(member is AttributeMember ? (AttributeMember)member : ((HierarchyMember)member).Origin);
                            }

                            if (agg.GetIndex(members) > -1) list.Add(members);                            
                        }
                        else
                        {
                            Join(level + 1);
                        }
                    }
                }

                JoinedMembers result = new JoinedMembers();
                list.ForEach(_ => result.Add(_.ToArray()));
                return result;
            }
        }

        /// <summary>
        /// Reverse member selection of each attributes
        /// </summary>
        public QuerySets ReverseMembers()
        {
            return Rearrange().ReverseMembers();
        }

        class RearrangedSets: List<QuerySet>
        {
            /// <summary>
            /// Determines if selection count > attr.MemberCount / 2
            /// </summary>
            public bool ShouldReverse
            {
                get
                {
                    int c = 0;
                    int m = 0;
                    for (int i = 0; i < this.Count; i++)
                    {
                        QuerySet set = this[i];
                        if (set.Count > 0)
                        {
                            Attribute attr = set[0] is AttributeMember ? 
                                ((AttributeMember)set[0]).Attribute : 
                                ((HierarchyMember)set[0]).Attribute;

                            int mc = attr.MemberCount;
                            if (mc > m)
                            {
                                m = mc;
                                c = set.Count;
                            }
                        }
                    }
                    return c != m && c > m / 2;
                }
            }

            /// <summary>
            /// Reverse member selection of each attributes
            /// </summary>
            public QuerySets ReverseMembers()
            {
                List<QuerySet> list = new List<QuerySet>();                
                this.ForEach(set =>
                {
                    Attribute attr = null;
                    if (set.Count > 0)
                    {
                        IQueryMember first = set[0];
                        attr = first is AttributeMember ? ((AttributeMember)first).Attribute : ((HierarchyMember)first).Attribute;

                        if (attr != null)
                        {
                            QuerySet qs = new QuerySet();
                            qs.AddRange(attr.Levels[1].Members.Except(set));
                            list.Add(qs);
                        }
                    }
                });
                return list.Count > 0 ? new QuerySets(list) : QuerySets.Empty;
            }

            /// <summary>
            /// ** FOR FILTERS ONLY **
            /// Remove empty or fully selected attribute member set
            /// </summary>
            public void CleanUp()
            { 
                for(int i=this.Count - 1; i >= 0; i--)
                {
                    QuerySet set = this[i];
                    if (set.Count > 0)
                    {
                        Attribute attr = set[0] is AttributeMember ?
                            ((AttributeMember)set[0]).Attribute :
                            ((HierarchyMember)set[0]).Attribute;

                        if (set.Count == attr.MemberCount) this.RemoveAt(i);
                    }
                    else
                        this.RemoveAt(i);
                }
            }
        }

        public class JoinedMembers: List<AttributeMember[]>
        {
            public Attribute[] Attributes
            {
                get
                {
                    List<Attribute> attrs = new List<Attribute>();
                    if (this.Count > 0)
                    {
                        AttributeMember[] members = this[0];
                        foreach (AttributeMember m in members)
                            if (m.LinkedMeasure == null)
                                attrs.Add(m.Attribute);
                    }
                    return attrs.ToArray();
                }
            }

            public static JoinedMembers Empty
            {
                get { return new JoinedMembers(); }
            }
        }
    }
}
