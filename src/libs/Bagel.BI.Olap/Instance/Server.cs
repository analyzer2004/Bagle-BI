using System;
using System.Collections.Generic;
using System.Text;

namespace Bagel.BI.Olap
{
    public class Server
    {
        public NamedObjectCollection<Database> Databases = new NamedObjectCollection<Database>();
    }
}
