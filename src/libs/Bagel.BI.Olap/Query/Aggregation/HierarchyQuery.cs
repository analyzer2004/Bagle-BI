using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Runtime.InteropServices;
using System.Linq;

namespace Bagel.BI.Olap
{
    public class HierarchyQuery : BaseQuery
    {
        public HierarchyQuery(Hierarchy hierarchy, IDbConnection cn) : base(cn)
        {
            Hierarchy = hierarchy;
        }
        public Hierarchy Hierarchy { get; set; }

        private string GetSql()
        {
            HierarchyDef def = Hierarchy.Definition;            
            
            List<Table> tables = new List<Table>();
            def.Levels.ForEach(_ =>
            {
                Table t = _.Attribute.KeyColumns[0].Table;
                if (!tables.Contains(t))
                    tables.Add(t);
            });

            List<Relationship> path = new List<Relationship>();
            for (int i = tables.Count - 1; i > 0; i--)
            {
                Table table = tables[i];
                Relationship rel = table.FindRelationshipByDestination(tables[i - 1]);
                if (rel != null) path.Add(rel);
            }
            
            for (int i = 0; i < tables.Count; i++)
            {
                tables[i].Alias = "t" + (i + 1).ToString();
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("select distinct ");
            int index = 0;
            List<string> names = new List<string>();
            foreach(LevelDef level in def.Levels)
            {
                AttributeDef attr = level.Attribute;
                if (index > 0) sb.Append(",");
                sb.AppendFormat("{0} as [___hq_key_{1}]", attr.KeyColumns.Expression, index);
                if (attr.OrderBy == AttributeOrderBy.Name) 
                    names.Add(string.Format("{0} as [___hq_name_{1}]", attr.NameColumn.Name, index));
                index++;
            }

            names.ForEach(_ => sb.AppendFormat(",{0}", _));

            sb.Append(" from ");
            index = 0;
            foreach (Table table in tables)
            {
                if (index > 0) sb.Append(",");
                sb.AppendFormat("[{0}] AS {1}", table.Name, table.Alias);
                index++;
            }

            if (path.Count > 0)
            {
                sb.Append(" where ");

                index = 0;
                foreach (Relationship rel in path)
                {
                    if (rel.SourceColumns.Count == rel.DestinationColumns.Count)
                    {
                        for (int i = 0; i < rel.SourceColumns.Count; i++)
                        {
                            if (index > 0) sb.Append(" AND ");
                            sb.AppendFormat(
                                "{0} = {1} ",
                                rel.SourceColumns[i].QualifiedName,
                                rel.DestinationColumns[i].QualifiedName
                                );
                            index++;
                        }
                    }
                    else
                    {
                        throw new Exception("Inconsistent column counts in relationship.");
                    }
                }
            }

            sb.Append(" order by ");
            for (int i = 0; i < def.Levels.Count; i++)
            {
                AttributeDef attr = def.Levels[i].Attribute;

                if (i > 0) sb.Append(",");
                if (attr.OrderBy == AttributeOrderBy.Key)
                    sb.AppendFormat("[___hq_key_{0}]", i);
                else
                    sb.AppendFormat("[___hq_name_{0}]", i);
            }


            return sb.ToString();
        }

        public void QueryMembers()
        {
            IDataReader reader = null;
            try
            {
                Attribute first = Hierarchy.Dimension.Attributes[Hierarchy.Definition.Levels[0].Name];                
                Hierarchy.Clear();
                HierarchyMember all = new HierarchyMember((AttributeMember)first.AllMember);
                Hierarchy.Levels[0].Members.Add(all);

                Attribute[] attrs = Hierarchy.Definition.Levels.Select(_ => Hierarchy.Dimension.Attributes[_.Attribute.Name]).ToArray();
                int attrLen = attrs.Length;
                HierarchyMember[] last = new HierarchyMember[attrLen];

                reader = ExecuteReader(GetSql());
                while (reader.Read())
                {
                    for (int i = 0; i <= attrLen - 1; i++)
                    {
                        AttributeMember curr = (AttributeMember)attrs[i].Levels[1].Members[reader[i]];
                        if (curr != null && (last[i] == null || last[i].Origin != curr))
                        {
                            HierarchyMember hm = new HierarchyMember(curr);
                            last[i] = hm;
                            if (i == 0)
                                all.Children.Add(hm);
                            else
                                last[i - 1].Children.Add(hm);
                            Hierarchy.Levels[i + 1].Members.Add(hm);
                        }
                    }
                }
            }
            finally
            {
                CloseReader(reader);
            }

        }
    }
}
