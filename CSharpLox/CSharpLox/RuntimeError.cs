using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLox
{
    public class RuntimeError : Exception
    {
        public Token Token { get; set; }
        public RuntimeError(Token token, string message) : base(message)
        {
            Token = token;
        }

    }
}
