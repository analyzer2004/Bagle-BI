using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Bagel.BI.Olap
{
    public class Aggregator : BaseQuery
    {
        public Aggregator(
            IDbConnection connection,
            Attribute[] attributes,
            Measure[] measures,
            List<Relationship> path
            ) : base(connection)
        {
            Attributes = attributes;
            Measures = measures;
            Path = path;
        }

        public Attribute[] Attributes { get; }
        public Measure[] Measures { get; }
        public List<Relationship> Path { get; }

        // measures == null => Full query
        // measures specified => Query specified measures only
        private string GetSql(Measure[] measures)
        {
            IEnumerable<AttributeDef> defs = Attributes.Select(_ => _.Definition);

            //正式版本不能這樣用, 目前像是Table, Column這些都是從definition, structure那邊拿過來的
            //每個物件都只有一個instance不能在multithreading的環境下這樣用
            List<Table> tables = new List<Table>();
            if (Path.Count == 0)
            {
                // Path.Count == 0 -> Dimension table is fact table
                tables.Add(defs.First().KeyColumns[0].Table);
            }
            else
            {
                foreach (Relationship rel in Path)
                {
                    if (!tables.Contains(rel.Source)) tables.Add(rel.Source);
                    if (!tables.Contains(rel.Destination)) tables.Add(rel.Destination);
                }
            }


            for (int i = 0; i < tables.Count; i++)
            {
                tables[i].Alias = "t" + (i + 1).ToString();
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT ");

            int index = 0;
            // Attribute columns
            foreach (AttributeDef attr in defs)
            {
                if (index > 0) sb.Append(",");
                sb.Append(attr.KeyColumns.Expression);
                index++;
            }

            // Measure columns
            Measure[] ms = measures != null ? measures : Measures;
            foreach (Measure m in ms)
            {
                if (index > 0) sb.Append(",");
                sb.AppendFormat("{0}({1})", m.Definition.Aggregation.ToString(), m.Definition.Source.QualifiedName);
                index++;
            }

            // measures == null => Full Query
            if (measures == null)
            {
                foreach (AttributeDef attr in defs)
                {
                    if (attr.OrderBy == AttributeOrderBy.Name)
                    {
                        if (index > 0) sb.Append(",");
                        sb.AppendFormat("MIN({0}) AS [__OBC_{1}]", attr.NameColumn.QualifiedName, attr.NameColumn.Name);
                        index++;
                    }
                }
            }

            sb.Append(" FROM ");

            // Tables
            index = 0;
            foreach(Table table in tables)
            {
                if (index > 0) sb.Append(",");
                sb.AppendFormat("[{0}] AS {1}", table.Name, table.Alias);
                index++;
            }

            if (Path.Count > 0)
            {
                sb.Append(" WHERE ");

                index = 0;
                foreach (Relationship rel in Path)
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

            index = 0;
            sb.Append(" GROUP BY ");
            foreach (AttributeDef attr in defs)
            {
                foreach (Column column in attr.KeyColumns)
                {
                    if (index > 0) sb.Append(",");
                    sb.Append(column.QualifiedName);
                    index++;
                }
            }

            // measures == null => Full Query
            if (measures == null)
            {
                index = 0;
                foreach (AttributeDef attr in defs)
                {
                    if (attr.OrderBy == AttributeOrderBy.Name)
                    {
                        if (index == 0) sb.Append(" ORDER BY ");
                        else if (index > 0) sb.Append(",");
                        sb.AppendFormat("[__OBC_{0}]", attr.NameColumn.Name);
                        index++;
                    }
                }
            }

            return sb.ToString();
        }

        public bool QueryMeasures(Aggregation agg, Measure[] measures)
        {
            if (measures.Length > 0)
            {
                IDataReader reader = null;
                try
                {
                    // s = index of the first new measure
                    // e = s + numbers of new measures
                    // s to e => new MeasureValues mapped to new measures
                    int s = Array.IndexOf(Measures, measures[0]);
                    int e = s + measures.Length;
                    int attrLen = Attributes.Length;
                    int rowIndex = 0;
                    reader = ExecuteReader(GetSql(measures));
                    while (reader.Read())
                    {
                        while(agg.TotalIndices.Contains(rowIndex))
                        {
                            for (int i = s; i < e; i++)
                            {
                                agg.Values[i].Values.Add(0);
                            }
                            rowIndex++;
                        }

                        int index = attrLen;
                        for (int i = s; i < e; i++)
                        {
                            agg.Values[i].Values.Add(reader[index]);
                            index++;
                        }
                        rowIndex++;
                    }

                    for (int i = rowIndex; i < agg.Tuples.Count; i++)
                    {
                        for (int j = s; j < e; j++)
                        {
                            agg.Values[j].Values.Add(0);
                        }
                    }
                }
                finally
                {
                    CloseReader(reader);
                }
                return true;
            }
            return false;
        }

        public Aggregation Query()
        {
            DateTime t = DateTime.Now;

            int attrLen = Attributes.Length;
            int ceiling = attrLen - 1;
            int mLen = Measures.Length;
            Aggregation agg = new Aggregation(Attributes, Measures);

            IDataReader reader = null;
            try
            {
                int rowIndex = 0;                
                reader = ExecuteReader(GetSql(null));
                while(reader.Read())
                {
                    AttributeMember[] tuple = new AttributeMember[attrLen];
                    for (int i = 0; i < attrLen; i++)
                    {
                        tuple[i] = (AttributeMember)Attributes[i].Levels[1].Members[reader[i]];
                    }
                    if (rowIndex > 0) AddTotals(tuple, ref rowIndex);
                    agg.Tuples.Add(tuple);

                    int index = attrLen;
                    for(int i = 0; i < mLen; i++)
                    {
                        agg.Values[i].Values.Add(reader[index]);
                        index++;
                    }
                    rowIndex++;
                }
                AddTotals(new AttributeMember[attrLen], ref rowIndex);
                BuildIndexTree();

                agg.QueryTime = (DateTime.Now - t).TotalMilliseconds;
                return agg;
            }
            finally
            {
                CloseReader(reader);
            }

            void AddTotals(AttributeMember[] tuple, ref int ri)
            {
                AttributeMember[] last = agg.Tuples.Last();
                for (int i = tuple.Length - 2; i >= 0; i--)
                {
                    if (tuple[i] != last[i])
                    {
                        AttributeMember[] total = (AttributeMember[])last.Clone();
                        for (int j = i + 1; j < attrLen; j++)
                        {
                            Attribute attr = last[j].Attribute;
                            //Member tm = new Member(attr.Dimension.AttributeAllMemberName);
                            //tm.IsTotal = true;
                            //tm.Attribute = attr;
                            total[j] = (AttributeMember)attr.AllMember;
                        }
                        agg.Tuples.Add(total);
                        agg.TotalIndices.Add(ri);

                        int index = attrLen;
                        for (int j = 0; j < mLen; j++)
                        {
                            agg.Values[j].Values.Add(0);
                            index++;
                        }
                        ri++;
                    }
                }
            }

            void BuildIndexTree()
            {
                IndexTreeNode[] currNodes = new IndexTreeNode[attrLen];
                for (int i = 0; i < agg.Tuples.Count; i++)
                {
                    bool changed = false;
                    for (int j = 0; j < attrLen; j++)
                    {
                        AttributeMember member = agg.Tuples[i][j];

                        // Transfers flat columns to IndexTree structure
                        // currNode == 1 => first pass
                        // currNode.Member != member => current member is different from the previous row
                        // changed => any previous columns is different from the previous row
                        IndexTreeNode currNode = currNodes[j];
                        if (currNode == null || currNode.Member != member || changed)
                        {
                            changed = true;
                            // Only leaf nodes hold row index
                            IndexTreeNode n = new IndexTreeNode(member);
                            if (j == 0)
                            {
                                agg.IndexTree.Nodes.Add(n);
                                if (attrLen == 1) n.Index = i;
                            }
                            else
                            {
                                currNodes[j - 1].Children.Add(n);
                                if (j == ceiling) n.Index = i;
                            }
                            currNodes[j] = n;
                        }
                    }
                }
            }

        }
    }
}
