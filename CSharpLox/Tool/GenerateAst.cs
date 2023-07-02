using System.Text;

if (args.Length != 1)
{
    Console.Error.WriteLine("Usage: generate_ast <output directory>");
    Environment.Exit(64);
}

var outputDir  = args[0];
DefineAst(outputDir, "Expr", new List<string>
    {
        "Assign     : Token name, Expr value",
        "Binary     : Expr left, Token operatorToken, Expr right",
        "Call       : Expr callee, Token paren, List<Expr> arguments",
        "Get        : Expr obj, Token name",
        "Grouping   : Expr expression",
        "Literal    : object? value",
        "Logical    : Expr left, Token operatorToken, Expr right",
        "Set        : Expr obj, Token name, Expr value",
        "This       : Token keyword",
        "Unary      : Token operatorToken, Expr right",
        "Variable   : Token name"
    });

DefineAst(outputDir, "Stmt", new List<string>
    {
        "Block      : List<Stmt> statements",
        "Class      : Token name, List<Stmt.Function> methods",
        "Expression : Expr expression",
        "Function   : Token name, List<Token> parameters, List<Stmt> body",
        "If         : Expr condition, Stmt thenBranch, Stmt? elseBranch",
        "Print      : Expr expression",
        "Return     : Token keyword, Expr? value",
        "Var        : Token name, Expr? initializer",
        "While      : Expr condition, Stmt body" 
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
    writer.WriteLine("\tpublic abstract R Accept<R>(IVisitor<R> visitor);");

    writer.WriteLine("}");
    writer.Close();
}

static void DefineVisitor(StreamWriter writer, string baseName, List<string> types)
{
    writer.WriteLine("\tpublic interface IVisitor<R>");
    writer.WriteLine("\t{");

    foreach (var type in types)
    {
        var typeName = type.Split(":")[0].Trim();
        writer.WriteLine($"\t\tR Visit{typeName}{baseName}({typeName} {baseName.ToLower()});");
    }

    writer.WriteLine("\t}");
}

static void DefineType(StreamWriter writer, string baseName, string className, string fieldList)
{
    writer.WriteLine();
    writer.WriteLine($"\tpublic class {className} : {baseName}");
    writer.WriteLine("\t{");

    // Constructor
    writer.WriteLine($"\t\tpublic {className} ({fieldList})");
    writer.WriteLine("\t\t{");

    // Store parameters in fields.
    string[] fields = fieldList.Split(", ");
    foreach (var field in fields)
    {
        var name = field.Split(" ")[1];
        writer.WriteLine($"\t\t\tthis.{name} = {name};");
    }

    writer.WriteLine("\t\t}");

    // Visitor pattern
    writer.WriteLine();
    writer.WriteLine("\t\tpublic override R Accept<R>(IVisitor<R> visitor)");
    writer.WriteLine("\t\t{");
    writer.WriteLine($"\t\t\treturn visitor.Visit{className}{baseName}(this);");
    writer.WriteLine("\t\t}");

    // Fields 
    writer.WriteLine();
    foreach (var field in fields)
    {
        writer.WriteLine($"\t\tpublic {field} ;");
    }

    writer.WriteLine("\t}");
}