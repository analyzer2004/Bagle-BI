using System;
using System.Collections.Generic;
using System.Text;

namespace Bagel.BI.Olap
{
    class ObjectDiectionary : Dictionary<object, IMember>
    {
        
    }

    public class MemberCollection: List<IMember>, IMemberCollection
    {
        private bool _keyIsName = false;
        private Dictionary<object, IMember> _keyDictionary = new Dictionary<object, IMember>();
        private Dictionary<string, IMember> _nameDictionary = new Dictionary<string, IMember>();

        public new IMember this[int index]
        {
            get { return base[index]; }
        }

        public IMember this[object key]
        {
            get
            {                
                IMember m;
                if (_keyDictionary.TryGetValue(key, out m))
                    return m;
                else if (!_keyIsName && _nameDictionary.TryGetValue(key.ToString(), out m))
                    return m;
                else
                    return null;
            }
        }

        public new virtual void Add(IMember item)
        {
            if (item != null) {
                if (Count == 0)
                {
                    _keyIsName = item.Attribute.Definition != null && item.Attribute.Definition.KeyIsName;
                }

                base.Add(item);
                _keyDictionary.Add(item.Key, item);
                if (!_keyIsName && !_nameDictionary.ContainsKey(item.Name)) 
                    _nameDictionary.Add(item.Name, item); 
            }
        }

        public new void AddRange(IEnumerable<IMember> collection)
        {
            if (collection != null)
                foreach (AttributeMember m in collection)
                    this.Add(m);
        }

        public new void Clear()
        {
            _keyIsName = false;
            _keyDictionary.Clear();
            _nameDictionary.Clear();
            base.Clear();
        }
    }
}
