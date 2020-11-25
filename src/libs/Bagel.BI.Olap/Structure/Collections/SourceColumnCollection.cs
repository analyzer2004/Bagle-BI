using System;
using System.Collections.Generic;
using System.Text;

namespace Bagel.BI.Olap
{
    public class SourceColumnCollection : List<Column>
    {
        public string Expression
        {
            get
            {
                if (this.Count == 1)
                {
                    return this[0].QualifiedName;
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < Count; i++)
                    {
                        if (i > 0) sb.Append(" + '.' + ");
                        sb.AppendFormat("ltrim(rtrim(cast({0} as nvarchar)))", this[i].QualifiedName);
                    }
                    return sb.ToString();
                }
            }
        }

        public new void Add(Column item)
        {
            if (Count > 0)
            {
                Column first = this[0];
                if (item.Table != first.Table)
                    throw new Exception(string.Format("{0} is from a different table.", item.Name));
            }
            base.Add(item);
        }

        public new void AddRange(IEnumerable<Column> collection)
        {
            if (collection != null)
                foreach (Column column in collection)
                    this.Add(column);
        }
    }
}
