using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bagel.BI.Olap
{
    public class Attribute : DimensionObject, IHierarchical
    {
        public Attribute(AttributeDef definition, string name) : base(name)
        {
            Definition = definition;
            Levels = new AttributeLevelCollection(this);
            Levels.Add(new AttributeLevel(AllLevelName));
            Levels.Add(new AttributeLevel(Name));
        }

        protected const string AllLevelName = "(All)";
                
        internal AttributeDef Definition { get; }

        [JsonConverter(typeof(CustomConverter<AttributeLevelCollection>))]
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
                foreach (AttributeLevel level in Levels)
                    list.AddRange(level.Members);
                return list.ToArray();
            }
        }

        [JsonIgnore()]
        public IMember this[object keyOrName]
        {
            get
            {
                return Levels[1].Members[keyOrName];
            }
        }

        [JsonIgnore()]
        public int MemberCount
        {
            get { return Levels[1].Members.Count; }
        }

        public void Clear()
        {
            foreach (AttributeLevel level in Levels)
                level.Members.Clear();
        }
    }
}
