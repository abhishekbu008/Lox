using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLox
{
    public class LoxFunction : ILoxCallable
    {
        private Stmt.Function declaration;
        private LoxEnvironment closure;

        public LoxFunction(Stmt.Function declaration, LoxEnvironment closure)
        {
            this.declaration = declaration;
            this.closure = closure;

        }

        public int Arity()
        {
            return declaration.parameters.Count;
        }

        public object? Call(Interpreter interpreter, List<object?> arguments)
        {
            LoxEnvironment environment = new(closure);
            for (int i = 0; i < declaration.parameters.Count; i++)
            {
                environment.Define(declaration.parameters[i].Lexeme, arguments[i]);
            }

            try
            {
                interpreter.ExecuteBlock(declaration.body, environment);
            }
            catch (Return returnValue)
            {
                return returnValue.Value;
            }

            return null;
        }

        public override string ToString()
        {
            return $"<fn {declaration.name.Lexeme}>";
        }
    }
}
