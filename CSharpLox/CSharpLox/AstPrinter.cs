using System.Text;

namespace CSharpLox
{
    //public class AstPrinter : Expr.IVisitor<string>
    //{
    //    public string Print(Expr expr)
    //    {
    //        return expr.Accept(this);
    //    }

    //    private string Parenthesize(string name, params Expr[] exprs)
    //    {
    //        StringBuilder builder = new StringBuilder();
    //        builder.Append("(").Append(name);
    //        foreach (Expr expr in exprs)
    //        {
    //            builder.Append(" ");
    //            builder.Append(expr.Accept(this));
    //        }
    //        builder.Append(")");
    //        return builder.ToString();
    //    }

    //    public string VisitBinaryExpr(Expr.Binary expr)
    //    {
    //        return Parenthesize(expr.operatorToken.Lexeme, expr.left, expr.right);
    //    }

    //    public string VisitGroupingExpr(Expr.Grouping expr)
    //    {
    //        return Parenthesize("group", expr.expression);
    //    }

    //    public string VisitLiteralExpr(Expr.Literal expr)
    //    {
    //        if (expr.value == null) return "nil";
    //        return expr.value.ToString();
    //    }

    //    public string VisitUnaryExpr(Expr.Unary expr)
    //    {
    //        return Parenthesize(expr.operatorToken.Lexeme, expr.right);
    //    }
    //}
}
