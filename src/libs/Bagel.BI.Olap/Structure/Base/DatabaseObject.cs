using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Bagel.BI.Olap
{
    public class DatabaseObject : NamedObject
    {
        public DatabaseObject(string name) : base(name) { }

        [JsonIgnore()]
        public Database Database { get; set; }
        [JsonIgnore()]
        public DatabaseRuntime Runtime 
        {
            get { return Database.Runtime; }
        }
    }
}
