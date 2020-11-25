﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;

namespace Bagel.BI.Olap
{
    public class KnownTypesBinder : ISerializationBinder
    {
        public IList<Type> KnownTypes { get; set; }

        public Type BindToType(string assemblyName, string typeName)
        {
            return KnownTypes.SingleOrDefault(t => t.Name == typeName);
        }

        public void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = null;
            typeName = serializedType.Name;
        }

        public static KnownTypesBinder Default
        {
            get
            {
                return new KnownTypesBinder {
                    KnownTypes = new List<Type> {
                    typeof(AttributeLevel),
                    typeof(HierarchyLevel)
                }};
            }
        }
    }
}
