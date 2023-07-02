using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CSharpLox.Stmt;


namespace CSharpLox
{
    public class LoxFunction : ILoxCallable
    {
        private Stmt.Function declaration;
        private LoxEnvironment closure;

        private readonly bool isInitializer;

        public LoxFunction(Stmt.Function declaration, LoxEnvironment closure, bool isInitializer)
        {
            this.declaration = declaration;
            this.closure = closure;
            this.isInitializer = isInitializer;
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
                if (isInitializer) return closure.GetAt(0, "this");
                return returnValue.Value;
            }

            if (isInitializer) return closure.GetAt(0, "this");
            return null;
        }

        public LoxFunction Bind(LoxInstance instance)
        {
            var environment = new LoxEnvironment(closure);
            environment.Define("this", instance);
            return new LoxFunction(declaration, environment, isInitializer);
        }

        public override string ToString()
        {
            return $"<fn {declaration.name.Lexeme}>";
        }
    }
}
