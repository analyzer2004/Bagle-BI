using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Converters;

namespace Bagel.BI.Olap
{
    public class AttributeLevelCollection : NamedObjectCollection<ILevel>, ILevelCollection
    {
        public AttributeLevelCollection() { }

        public AttributeLevelCollection(Attribute attribute) : base()
        {
            Attribute = attribute;
        }

        public Attribute Attribute { get; }

        public new void Add(ILevel item)
        {
            ((AttributeLevel)item).Attribute = Attribute;
            base.Add(item);
        }

        public new void AddRange(IEnumerable<ILevel> collection)
        {
            if (collection != null)
                foreach (AttributeLevel item in collection)
                    this.Add(item);
        }
    }

    public class AttributeLevelCollectionConverter : CustomCreationConverter<AttributeLevelCollection>
    {
        public override AttributeLevelCollection Create(Type objectType)
        {
            return new AttributeLevelCollection();
        }
    }
}
