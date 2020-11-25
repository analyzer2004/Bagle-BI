using System;
using System.Collections.Generic;
using System.Text;

namespace Bagel.BI.Olap
{
    public class TypeMismatchException : Exception
    {
        public TypeMismatchException(string message) : base(message) { }
    }
}