using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bagel.BI.Olap
{
    public class PathFinder : List<IDataElement>        
    {
        public PathFinder(QueryCommand cmd) : base()
        {
            this.AddRange(cmd.Attributes.Select(_ => _.Definition));
            this.AddRange(cmd.Measures.Select(_ => _.Definition));
            FindPaths();
        }

        public List<JoinPath> Paths { get; } = new List<JoinPath>();

        public new void Add(IDataElement item)
        {
            if (!this.Contains(item))
                base.Add(item);
        }

        public new void AddRange(IEnumerable<IDataElement> collection)
        {
            if (collection != null)
                foreach (IDataElement item in collection)
                    this.Add(item);
        }

        public List<Relationship> GetPath(IEnumerable<Table> dimTables, Table factTable)
        {
            List<Relationship> list = new List<Relationship>();
            foreach (Table dimTable in dimTables)
            {
                foreach (JoinPath jp in Paths)
                {
                    if (jp.Equals(dimTable, factTable))
                    {
                        foreach (Relationship rel in jp.Path)
                            if (!list.Contains(rel)) list.Add(rel);
                    }
                }
            }
            return list;
        }


        private void FindPaths()
        {
            Paths.Clear();

            // Scan measures to find fact tables
            List<Table> facts = new List<Table>();                
            foreach (IDataElement elem in this)
            {
                if (elem is MeasureDef)
                {                    
                    facts.Add(((MeasureDef)elem).Source.Table);
                    
                }
            }

            if (facts.Count > 0)
            {
                // Uses fact tables as root nodes                
                TableTree tree = new TableTree();
                foreach (Table fact in facts)
                {
                    TableTreeNode n = new TableTreeNode(fact);
                    tree.Nodes.Add(n);
                    Walkthrough(n);
                }

                // Scan attributes to find dimension tables
                List<Table> dimTables = new List<Table>();
                foreach (IDataElement elem in this)
                {
                    if (elem is AttributeDef)
                    {
                        AttributeDef attr = (AttributeDef)elem;
                        if (!dimTables.Contains(attr.KeyColumns[0].Table))
                            dimTables.Add(attr.KeyColumns[0].Table);
                    }
                }

                foreach(Table table in dimTables)
                {
                    TableTreeNode n = tree.FindNodeByTable(table);
                    if (n != null)
                    {
                        JoinPath jp = n.GetJoinPath();
                        if (jp != null) Paths.Add(jp);
                    }
                }
            }
        }

        private void Walkthrough(TableTreeNode node)
        {
            // Uses destination of all the relationsips of current table to build child nodes,
            // keeps walkthrough till the end of the branch.
            foreach(Relationship rel in node.Table.Relationships)
            {
                TableTreeNode child = new TableTreeNode(rel.Destination);
                node.Add(child);
                Walkthrough(child);
            }
        }
    }
}
