using System;
using System.Collections.Generic;
using System.Text;

namespace restep.Framework.Exceptions
{
    internal class ShaderCompileException : Exception
    {
        public ShaderCompileException(string error)
            : base(error)
        { }
    }
}
