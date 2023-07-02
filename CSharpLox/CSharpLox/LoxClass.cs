using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLox
{
    public class LoxClass : ILoxCallable
    {
        public string name;
        private Dictionary<string, LoxFunction> methods;

        public LoxClass(string name, Dictionary<string, LoxFunction> methods)
        {
            this.name = name;
            this.methods = methods;

        }

        public LoxFunction? FindMethod(string name)
        {
            if (methods.ContainsKey(name)) return methods[name];
            return null;
        }

        public int Arity()
        {
            var initializer = FindMethod("init");
            if (initializer == null) return 0;
            return initializer.Arity();
        }

        public object? Call(Interpreter interpreter, List<object?> arguments)
        {
            var instance = new LoxInstance(this);
            var initializer = FindMethod("init");
            if (initializer != null)
            {
                initializer.Bind(instance).Call(interpreter, arguments);
            }
            return instance;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
