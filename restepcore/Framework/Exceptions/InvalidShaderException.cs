using System;
using System.Collections.Generic;
using System.Text;

namespace restep.Framework.Exceptions
{
    class InvalidShaderException : Exception
    {
        public InvalidShaderException(string error)
            : base(error)
        { }
    }
}
