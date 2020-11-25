using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bagel.BI.Olap
{
    public class QuerySet : List<IQueryMember>
    {
        public QuerySet() { }

        public QuerySet(IEnumerable<IQueryMember> collection)
        {
            this.AddRange(collection);
        }

        public Attribute[] Attributes
        {
            get
            {
                if (Count > 0)
                {
                    if (this[0] is AttributeMember)
                        return new Attribute[] { ((AttributeMember)this[0]).Attribute };
                    else if (this[0] is HierarchyMember)
                    {
                        List<Attribute> attrs = new List<Attribute>();
                        Hierarchy hier = ((HierarchyMember)this[0]).Hierarchy;
                        for (int i = 1; i < hier.Levels.Count; i++)
                        {
                            HierarchyLevel level = (HierarchyLevel)hier.Levels[i];
                            if (level.Members.Count > 0)
                            {
                                attrs.Add(level.Members[0].Attribute);
                            }
                        }
                        return attrs.ToArray();
                    }
                }

                return null;
            }
        }

        public Measure[] Measures
        {
            get
            {
                if (Count > 0 && this[0] is Measure)
                    return this.Select(_ => (Measure)_).ToArray();
                return null;
            }
        }        

        public List<QuerySet> GetQuerySets()
        {
            List<QuerySet> list = new List<QuerySet>();
            if (this[0] is HierarchyMember)
            {
                QuerySet set = null;
                Attribute attr = null;
                foreach(HierarchyMember member in this)
                {
                    if (attr == null || member.Attribute != attr)
                    {
                        if (set != null) list.Add(set);
                        set = new QuerySet();
                        attr = member.Attribute;
                    }

                    set.Add(member);
                }
                list.Add(set);
            }
            else
                list.Add(this);
            return list;
        }

        public new void Add(IQueryMember item)
        {
            if (Count > 0)
            {
                IQueryMember first = this[0];
                if (item.GetType() != first.GetType())
                    throw new TypeMismatchException(item.Name);
                else if (item is AttributeMember && ((AttributeMember)first).AttributeDef != ((AttributeMember)item).AttributeDef)
                    throw new AttributeMismatchException(item.Name);
                else if (item is HierarchyMember && ((HierarchyMember)first).HierarchyDef != ((HierarchyMember)item).HierarchyDef)
                    throw new AttributeMismatchException(item.Name);
                else
                    base.Add(item);
            }
            else
            {
                base.Add(item);
            }
        }

        public new void AddRange(IEnumerable<IQueryMember> collection)
        {
            if (collection != null)
                foreach (IQueryMember item in collection)
                    this.Add(item);
        }
    }
}
