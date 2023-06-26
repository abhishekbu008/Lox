using System.Text;

if (args.Length != 1)
{
    Console.Error.WriteLine("Usage: generate_ast <output directory>");
    Environment.Exit(64);
}
var outputDir  = args[0];
DefineAst(outputDir, "Expr", new List<string>
    {
        "Binary     : Expr left, Token operatorToken, Expr right",
        "Grouping   : Expr expression",
        "Literal    : object value",
        "Unary      : Token operatorToken, Expr right"
    });

static void DefineAst(string outputDir, string baseName, List<string> types)
{
    string path = $"{outputDir}/{baseName}.cs";
    StreamWriter writer = new(path, false, Encoding.UTF8);

    writer.WriteLine("namespace CSharpLox;");
    writer.WriteLine();
    writer.WriteLine("using System.Collections;");
    writer.WriteLine();
    writer.WriteLine($"public abstract class {baseName} {{");

    // Visitor
    DefineVisitor(writer, baseName, types);

    // The AST classes.
    foreach (var type in types)
    {
        string className = type.Split(':')[0].Trim();
        string fields = type.Split(":")[1].Trim();
        DefineType(writer, baseName, className, fields);
    }

    // The base accept() method.
    writer.WriteLine();
    writer.WriteLine("    public abstract R Accept<R>(IVisitor<R> visitor);");

    writer.WriteLine("}");
    writer.Close();
}

static void DefineVisitor(StreamWriter writer, string baseName, List<string> types)
{
    writer.WriteLine("    public interface IVisitor<R>");
    writer.WriteLine("    {");

    foreach (var type in types)
    {
        var typeName = type.Split(":")[0].Trim();
        writer.WriteLine($"        R Visit{typeName}{baseName}({typeName} {baseName.ToLower()});");
    }

    writer.WriteLine("    }");
}

static void DefineType(StreamWriter writer, string baseName, string className, string fieldList)
{
    writer.WriteLine();
    writer.WriteLine($"    public class {className} : {baseName}");
    writer.WriteLine("    {");

    // Constructor
    writer.WriteLine($"        public {className} ({fieldList})");
    writer.WriteLine("        {");

    // Store parameters in fields.
    string[] fields = fieldList.Split(", ");
    foreach (var field in fields)
    {
        var name = field.Split(" ")[1];
        writer.WriteLine($"            this.{name} = {name};");
    }

    writer.WriteLine("        }");

    // Visitor pattern
    writer.WriteLine();
    writer.WriteLine("        public override R Accept<R>(IVisitor<R> visitor)");
    writer.WriteLine("        {");
    writer.WriteLine($"            return visitor.Visit{className}{baseName}(this);");
    writer.WriteLine("        }");

    // Fields 
    writer.WriteLine();
    foreach (var field in fields)
    {
        writer.WriteLine($"        public {field} ;");
    }

    writer.WriteLine("    }");
}