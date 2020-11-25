using System;
using System.Collections.Generic;
using System.Text;

namespace Bagel.BI.Olap
{
    public interface IHierarchical
    {
        ILevelCollection Levels { get; }
        IMember AllMember { get; }
        IMember[] AllMembers { get; }
        IMember this[object keyOrName] { get; }
        int MemberCount { get; }

        void Clear();
    }
}
