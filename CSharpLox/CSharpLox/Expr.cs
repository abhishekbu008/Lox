namespace CSharpLox;

using System.Collections;

public abstract class Expr {
	public interface IVisitor<R>
	{
		R VisitAssignExpr(Assign expr);
		R VisitBinaryExpr(Binary expr);
		R VisitGroupingExpr(Grouping expr);
		R VisitLiteralExpr(Literal expr);
		R VisitUnaryExpr(Unary expr);
		R VisitVariableExpr(Variable expr);
	}

	public class Assign : Expr
	{
		public Assign (Token name, Expr value)
		{
			this.name = name;
			this.value = value;
		}

		public override R Accept<R>(IVisitor<R> visitor)
		{
			return visitor.VisitAssignExpr(this);
		}

		public Token name ;
		public Expr value ;
	}

	public class Binary : Expr
	{
		public Binary (Expr left, Token operatorToken, Expr right)
		{
			this.left = left;
			this.operatorToken = operatorToken;
			this.right = right;
		}

		public override R Accept<R>(IVisitor<R> visitor)
		{
			return visitor.VisitBinaryExpr(this);
		}

		public Expr left ;
		public Token operatorToken ;
		public Expr right ;
	}

	public class Grouping : Expr
	{
		public Grouping (Expr expression)
		{
			this.expression = expression;
		}

		public override R Accept<R>(IVisitor<R> visitor)
		{
			return visitor.VisitGroupingExpr(this);
		}

		public Expr expression ;
	}

	public class Literal : Expr
	{
		public Literal (object? value)
		{
			this.value = value;
		}

		public override R Accept<R>(IVisitor<R> visitor)
		{
			return visitor.VisitLiteralExpr(this);
		}

		public object? value ;
	}

	public class Unary : Expr
	{
		public Unary (Token operatorToken, Expr right)
		{
			this.operatorToken = operatorToken;
			this.right = right;
		}

		public override R Accept<R>(IVisitor<R> visitor)
		{
			return visitor.VisitUnaryExpr(this);
		}

		public Token operatorToken ;
		public Expr right ;
	}

	public class Variable : Expr
	{
		public Variable (Token name)
		{
			this.name = name;
		}

		public override R Accept<R>(IVisitor<R> visitor)
		{
			return visitor.VisitVariableExpr(this);
		}

		public Token name ;
	}

	public abstract R Accept<R>(IVisitor<R> visitor);
}
