using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bagel.BI.Olap {
    public class TableTree
    {
        private List<TableTreeNode> _allNodes = null;
        public List<TableTreeNode> Nodes { get; } = new List<TableTreeNode>();
              
        private List<TableTreeNode> GetAllNodes()
        {
            if (_allNodes == null)
            {
                List<TableTreeNode> list = new List<TableTreeNode>();
                foreach (TableTreeNode node in Nodes)
                {
                    list.Add(node);
                    Walkthrough(node);
                }
                _allNodes = list;

                void Walkthrough(TableTreeNode node)
                {
                    foreach (TableTreeNode child in node.Children)
                    {
                        list.Add(child);
                        Walkthrough(child);
                    }
                }
            }
            return _allNodes;
        }

        public TableTreeNode FindNodeByTable(Table table)
        {
            List<TableTreeNode> all = GetAllNodes();
            return all.Find(_ => _.Table == table);
        }
    }    

    public class TableTreeNode
    {
        public TableTreeNode(Table table)
        {
            Table = table;
        }

        public TableTreeNode Parent { get; set; } = null;
        public Table Table { get; set; } = null;
        public List<TableTreeNode> Children { get; } = new List<TableTreeNode>();

        public TableTreeNode Add(TableTreeNode node)
        {
            node.Parent = this;            
            Children.Add(node);
            return node;
        }

        public JoinPath GetJoinPath()
        {
            List<Relationship> list = new List<Relationship>();
            TableTreeNode current = this;
            Table last = null;
            // Trace back to root node which is a fact table and add all the paths(relationships) to the list
            while(current != null)
            {
                TableTreeNode parent = current.Parent;
                if (parent != null)
                {
                    Relationship rel = parent.Table.FindRelationshipByDestination(current.Table);
                    if (rel != null) list.Add(rel);
                    last = parent.Table;
                }
                current = parent;
            }
            return new JoinPath(last, Table, list);
        }

    }
}
