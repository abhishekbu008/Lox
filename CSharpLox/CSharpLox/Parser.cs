using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLox
{
    public class Parser
    {

        private readonly List<Token> tokens;
        private int current = 0;

        public class ParseError : Exception { }

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        // program -> declaration* EOF;
        public List<Stmt> Parse()
        {
            var statements = new List<Stmt>();
            while (!IsAtEnd())
            {
                statements.Add(Declaration());
            }

            return statements;
        }

        // Expression -> assignment;
        private Expr Expression()
        {
            return Assignment();
        }

        // declaration -> varDecl | statement;
        private Stmt Declaration()
        {
            try
            {
                if (Match(TokenType.VAR)) return VarDeclaration();
                return Statement();
            } 
            catch (ParseError error)
            {
                Synchronize();
                return null;
            }
        }

        // statement -> exprStmt
        //              | printStmt
        //              | block ;
        private Stmt Statement()
        {
            if (Match(TokenType.PRINT)) return PrintStatement();
            if (Match(TokenType.LEFT_BRACE)) return new Stmt.Block(Block());

            return ExpressionStatement();
        }

        // printStmt -> "print" expression ";" ;
        private Stmt PrintStatement()
        {
            Expr value = Expression();
            Consume(TokenType.SEMICOLON, "Expect ';' after value");
            return new Stmt.Print(value);
        }

        // ()? means exactly zero or one time but not more than that
        // varDecl     -> "var" IDENTIFIER ( "=" expression)? ";" 
        private Stmt VarDeclaration()
        {
            Token name = Consume(TokenType.IDENTIFIER, "Expect variable name.");

            Expr? initializer = null;
            if (Match(TokenType.EQUAL))
            {
                initializer = Expression();
            }

            Consume(TokenType.SEMICOLON, "Expect ';' after variable declaration");
            return new Stmt.Var(name, initializer);
        }

        // exprStmt  -> expression ";" ;
        private Stmt ExpressionStatement()
        {
            Expr expr = Expression();
            Consume(TokenType.SEMICOLON, "Expect ';' after expression");
            return new Stmt.Expression(expr);
        }

        // block     -> "{" declaration "}" ;
        private List<Stmt> Block()
        {
            List<Stmt> statements = new();

            while (!Check(TokenType.RIGHT_BRACE) && !IsAtEnd()) 
            {
                statements.Add(Declaration());
            }

            Consume(TokenType.RIGHT_BRACE, "Expect '}' after block");
            return statements;
        }

        // assignment -> IDENTIFIER "=" assignment
        //               | equality
        private Expr Assignment()
        {
            Expr expr = Equality();
            if (Match(TokenType.EQUAL))
            {
                Token equals = Previous();
                Expr value = Assignment();

                if (expr is Expr.Variable variable)
                {
                    Token name = variable.name;
                    return new Expr.Assign(name, value);
                }

                Error(equals, "Invalid assignment target");
            }

            return expr;
        }

        // Equality   -> Comparison ( ( "!=" | "==" ) Comparison )* ;
        private Expr Equality()
        {
            Expr expr = Comparision();

            while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
            {
                Token operatorToken = Previous();
                Expr right = Comparision();
                expr = new Expr.Binary(expr, operatorToken, right);
            }

            return expr;
        }

        // Comparison -> Term ( ( ">" | ">=" | "<" | "<=" ) Term )* ;
        private Expr Comparision()
        {
            Expr expr = Term();

            while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
            {
                Token operatorToken = Previous();
                Expr right = Term();
                expr = new Expr.Binary(expr, operatorToken, right);
            }

            return expr;
        }

        // Term -> Factor (( "-" | "+" ) Factor )* ;
        private Expr Term()
        {
            Expr expr = Factor();

            while(Match(TokenType.MINUS, TokenType.PLUS))
            {
                Token operatorToken = Previous();
                Expr right = Factor();
                expr = new Expr.Binary(expr, operatorToken, right);
            }

            return expr;
        }

        // Factor -> Unary (( "/" | "*" ) Unary )* ;
        private Expr Factor()
        {
            Expr expr = Unary();

            while(Match(TokenType.SLASH, TokenType.STAR))
            {
                Token operatorToken = Previous();
                Expr right = Unary();
                expr = new Expr.Binary(expr, operatorToken, right);
            }

            return expr;
        }

        // Unary      -> ( "!" | "-" ) Unary
        //              | Primary ;
        private Expr Unary()
        {
            if (Match(TokenType.BANG, TokenType.MINUS))
            {
                Token operatorToken = Previous();
                Expr right = Unary();
                return new Expr.Unary(operatorToken, right);
            }

            return Primary();
        }

        // Primary    -> NUMBER | STRING | "true" | "false" | "nil"
        //              | "(" Expression ")" | IDENTIFIER ;
        private Expr Primary()
        {
            if (Match(TokenType.FALSE)) return new Expr.Literal(false);
            if (Match(TokenType.TRUE)) return new Expr.Literal(true);
            if (Match(TokenType.NIL)) return new Expr.Literal(null);

            if (Match(TokenType.NUMBER, TokenType.STRING))
            {
                return new Expr.Literal(Previous().Literal);
            }

            if (Match(TokenType.IDENTIFIER))
            {
                return new Expr.Variable(Previous());
            }

            if (Match(TokenType.LEFT_PAREN))
            {
                Expr expr = Expression();
                Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression");
                return new Expr.Grouping(expr);
            }

            throw Error(Peek(), "Expect expression");
        }

        private bool Match(params TokenType[] types)
        {
            foreach (var type in types)
            {
                if (Check(type))
                {
                    Advance();
                    return true;
                }
            }

            return false;
        }

        private Token Consume(TokenType type, string message)
        {
            if (Check(type)) return Advance();
            throw Error(Peek(), message);
        }

        private bool Check(TokenType type)
        {
            if (IsAtEnd()) return false;
            return Peek().Type == type;
        }

        private Token Advance()
        {
            if (!IsAtEnd()) current++;
            return Previous();
        }

        private bool IsAtEnd()
        {
            return Peek().Type == TokenType.EOF;
        }

        private Token Peek()
        {
            return tokens[current];
        }

        private Token Previous()
        {
            return tokens[current - 1];
        }

        private ParseError Error(Token token, string message)
        {
            Lox.Error(token, message);
            return new ParseError();
        }

        private void Synchronize()
        {
            Advance();

            while(!IsAtEnd())
            {
                if (Previous().Type == TokenType.SEMICOLON) return;

                switch (Peek().Type)
                {
                    case TokenType.CLASS:
                    case TokenType.FUN:
                    case TokenType.VAR:
                    case TokenType.FOR:
                    case TokenType.IF:
                    case TokenType.WHILE:
                    case TokenType.PRINT:
                    case TokenType.RETURN:
                        return;
                }

                Advance();
            }
        }
    }
}
