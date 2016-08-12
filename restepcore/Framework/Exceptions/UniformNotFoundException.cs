using System;
using System.Collections.Generic;
using System.Text;

namespace restep.Framework.Exceptions
{
    internal class UniformNotFoundException : Exception
    {
        public UniformNotFoundException(string error)
            : base(error) { }
    }
}
