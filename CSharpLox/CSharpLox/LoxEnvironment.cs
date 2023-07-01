namespace CSharpLox
{
    public class LoxEnvironment
    {
        public LoxEnvironment? enclosing;
        private readonly Dictionary<string, object?> values = new();

        public LoxEnvironment()
        {
            enclosing = null;
        }

        public LoxEnvironment(LoxEnvironment enclosing)
        {
            this.enclosing = enclosing;
        }

        public object? Get(Token name)
        {
            if (values.ContainsKey(name.Lexeme))
            {
                return values[name.Lexeme];
            }

            if (enclosing != null) return enclosing.Get(name);

            throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
        }

        public void Assign(Token name, object? value)
        {
            if (values.ContainsKey(name.Lexeme))
            {
                values[name.Lexeme] = value;
                return;
            }

            if (enclosing != null)
            {
                enclosing.Assign(name, value);
                return;
            }

            throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
        }

        public void Define(string name, object? value)
        {
            values[name] = value;
        }

        LoxEnvironment? Ancestor(int distance)
        {
            LoxEnvironment? environment = this;
            for(int i = 0; i < distance; i++)
            {
                environment = environment?.enclosing;
            }

            return environment;
        }

        public object? GetAt(int distance, string name)
        {
            return Ancestor(distance)?.values[name];
        }

        public void AssignAt(int distance, Token name, object? value)
        {
            var ancestor = Ancestor(distance);
            if (ancestor == null) return;
            ancestor.values[name.Lexeme] = value;
        }
    }
}   
