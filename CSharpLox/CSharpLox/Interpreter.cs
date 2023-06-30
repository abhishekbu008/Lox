using System.Diagnostics;

namespace CSharpLox
{
    public class Interpreter : Expr.IVisitor<object?>, Stmt.IVisitor<object?>
    {
        private LoxEnvironment environment = new();
        public void Interpret(List<Stmt> statements)
        {
            try
            {
                foreach (Stmt statement in statements)
                {
                    Execute(statement);
                }
            }
            catch (RuntimeError error)
            {
                Lox.RuntimeError(error);
            }
        }

        public object? VisitBinaryExpr(Expr.Binary expr)
        {
            var left = Evaluate(expr.left);
            var right = Evaluate(expr.right);
            if (left == null || right == null) return null;

            switch (expr.operatorToken.Type)
            {
                case TokenType.GREATER:
                    CheckNumberOperands(expr.operatorToken, left, right);
                    return (double)left > (double)right;
                case TokenType.GREATER_EQUAL:
                    CheckNumberOperands(expr.operatorToken, left, right);
                    return (double)left >= (double)right;
                case TokenType.LESS:
                    CheckNumberOperands(expr.operatorToken, left, right);
                    return (double)left < (double)right;
                case TokenType.LESS_EQUAL:
                    CheckNumberOperands(expr.operatorToken, left, right);
                    return (double)left <= (double)right;
                case TokenType.MINUS:
                    CheckNumberOperands(expr.operatorToken, left, right);
                    return (double)left - (double)right;
                case TokenType.BANG_EQUAL: return !IsEqual(left, right);
                case TokenType.EQUAL_EQUAL: return IsEqual(left, right);
                case TokenType.PLUS:
                    if (left is double && right is double)
                    {
                        return (double)left + (double)right;
                    }

                    if (left is string && right is string)
                    {
                        return (string)left + (string)right;
                    }
                    throw new RuntimeError(expr.operatorToken, "Operator must be two numbers or two strings");
                case TokenType.SLASH:
                    CheckNumberOperands(expr.operatorToken, left, right);
                    return (double)left / (double)right;
                case TokenType.STAR:
                    CheckNumberOperands(expr.operatorToken, left, right);
                    return (double)left * (double)right;
            }

            // Unreachable code
            return null;
        }

        public object? VisitGroupingExpr(Expr.Grouping expr)
        {
            return Evaluate(expr.expression);
        }

        public object? VisitLiteralExpr(Expr.Literal expr)
        {
            return expr.value;
        }

        public object? VisitLogicalExpr(Expr.Logical expr)
        {
            var left = Evaluate(expr.left);

            if (expr.operatorToken.Type == TokenType.OR) 
            {
                if (IsTruthy(left)) return left;
            }
            else
            {
                if (!IsTruthy(left)) return left;
            }

            return Evaluate(expr.right);
        }

        public object? VisitUnaryExpr(Expr.Unary expr)
        {
            var right = Evaluate(expr.right);

            if (right != null)
            {
                switch(expr.operatorToken.Type)
                {
                    case TokenType.BANG: return !IsTruthy(right);
                    case TokenType.MINUS: 
                        CheckNumberOperand(expr.operatorToken, right);
                        return -(double)right;
                }
            }

            return null;
        }

        private void CheckNumberOperand(Token operatorToken, object operand)
        {
            if (operand is double) return;
            throw new RuntimeError(operatorToken, "Operand must be a number");
        }

        private void CheckNumberOperands(Token operatorToken, object left,  object right)
        {
            if (left is double && right is double) return;
            throw new RuntimeError(operatorToken, "Operands must be numbers");
        }

        private bool IsTruthy(object? obj)
        {
            if (obj == null) return false;
            if (obj is bool boolVal) return boolVal;
            if (IsNumberAndZero(obj)) return false;
            return true;
        }

        private bool IsEqual(object a, object b)
        {
            if (a == null || b == null) return true;
            if (a == null) return false;

            return a.Equals(b);
        }

        private string? Stringify(object? obj)
        {
            if (obj == null) return "nil";

            if (obj is double)
            {
                var text = obj.ToString();
                if (text != null && text.EndsWith(".0"))
                {
                    text = text[0..(text.Length - 2)];
                }
                return text;
            }

            return obj.ToString();
        }

        private bool IsNumberAndZero(object obj)
        {
            if (obj is int || obj is double || obj is float || obj is decimal)
            {
                var number = (decimal)obj;
                return number == 0;
            }
            return false;
        }

        private object? Evaluate(Expr expr)
        {
            return expr.Accept(this);
        }

        private void Execute(Stmt stmt)
        {
            stmt.Accept(this);
        }

        void ExecuteBlock(List<Stmt> statements, LoxEnvironment environment)
        {
            var previous = this.environment;
            try
            {
                this.environment = environment;

                foreach (var statement in statements)
                {
                    Execute(statement);
                }
            }
            finally
            {
                this.environment = previous;
            }
        }

        public object? VisitBlockStmt(Stmt.Block stmt)
        {
            ExecuteBlock(stmt.statements, new LoxEnvironment(environment));
            return null;
        }

        public object? VisitExpressionStmt(Stmt.Expression stmt)
        {
            Evaluate(stmt.expression);
            return null;
        }

        public object? VisitIfStmt(Stmt.If stmt)
        {
            var evauateCond = Evaluate(stmt.condition);
            if (evauateCond != null && IsTruthy(evauateCond)) {
                Execute(stmt.thenBranch);
            } else if (stmt.elseBranch != null)
            {
                Execute(stmt.elseBranch);
            }

            return null;
        }

        public object? VisitPrintStmt(Stmt.Print stmt)
        {
            var value = Evaluate(stmt.expression);
            Console.WriteLine(Stringify(value));
            return null;
        }

        public object? VisitVarStmt(Stmt.Var stmt)
        {
            object? value = null;
            if (stmt.initializer != null)
            {
                value = Evaluate(stmt.initializer);
            }

            environment.Define(stmt.name.Lexeme, value);
            return null;
        }

        public object? VisitWhileStmt(Stmt.While stmt)
        {
            while (IsTruthy(Evaluate(stmt.condition)))
            {
                Execute(stmt.body);
            }

            return null;
        }

        public object? VisitAssignExpr(Expr.Assign expr)
        {
            var value = Evaluate(expr.value);
            environment.Assign(expr.name, value);
            return value;
        }

        public object? VisitVariableExpr(Expr.Variable expr)
        {
            return environment.Get(expr.name);
        }
    }
}
