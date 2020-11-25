using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Newtonsoft.Json;

namespace Bagel.BI.Olap
{
    public class Hierarchy : DimensionObject, IHierarchical
    {
        public Hierarchy(HierarchyDef definition, string name) : base(name)
        {
            Definition = definition;
            Levels = new HierarchyLevelCollection(this);
            // Definition == null - Json deserialization
            if (Definition != null)
            {
                Levels.Add(new HierarchyLevel(AllLevelName));
                foreach (LevelDef level in definition.Levels)
                    Levels.Add(new HierarchyLevel(level.Name));
            }
        }

        protected const string AllLevelName = "(All)";

        internal HierarchyDef Definition { get; }

        [JsonConverter(typeof(CustomConverter<HierarchyLevelCollection>))]
        public ILevelCollection Levels { get; }
        [JsonIgnore()]
        public IMember AllMember
        {
            get
            {                
                if (Levels.Count > 0 && Levels[0].Members.Count > 0)
                    return Levels[0].Members[0];
                else
                    return null;
            }
        }

        [JsonIgnore()]
        public IMember[] AllMembers
        {
            get
            {
                List<IMember> list = new List<IMember>();
                foreach (HierarchyLevel level in Levels)
                    list.AddRange(level.Members);
                return list.ToArray();
            }
        }

        [JsonIgnore()]
        public IMember this[object keyOrName]
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [JsonIgnore()]
        public int MemberCount
        {
            get
            {
                int count = 0;
                for (int i = 1; i < Levels.Count; i++)
                    count += Levels[i].Members.Count;
                return count;
            }
        }


        public void Clear()
        {
            foreach (HierarchyLevel level in Levels)
                level.Members.Clear();
        }
    }
}
