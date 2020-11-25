using System;
using System.Collections.Generic;
using System.Text;

namespace Bagel.BI.Olap
{
    public class AttributeMismatchException : Exception
    {
        public AttributeMismatchException(string message) : base(message) { }
    }
}
