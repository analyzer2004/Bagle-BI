using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bagel.BI.Olap
{
    public class IndexTree
    {
        public List<IndexTreeNode> Nodes { get; } = new List<IndexTreeNode>();
    }

    public class IndexTreeNode
    {
        public IndexTreeNode(AttributeMember member) : this(member, 0) { }

        public IndexTreeNode(AttributeMember member, int index)
        {
            Member = member;
            Index = index;
        }

        public AttributeMember Member { get; private set; }
        public int Index { get; set; }
        public List<IndexTreeNode> Children { get; } = new List<IndexTreeNode>();
    }
}
