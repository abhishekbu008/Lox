using System.Data.Common;

namespace CSharpLox
{
    internal class Scanner
    {
        private string Source { get; }
        private List<Token> Tokens = new();
        private int Start = 0;
        private int Current = 0;
        private int Line = 0;

        private static readonly Dictionary<string, TokenType> Keywords = new()
        {
            { "and", TokenType.AND },
            { "class", TokenType.CLASS },
            { "else", TokenType.ELSE },
            { "false", TokenType.FALSE },
            { "for", TokenType.FOR },
            { "fun", TokenType.FUN },
            { "if", TokenType.IF },
            { "nil", TokenType.NIL },
            { "or", TokenType.OR },
            { "print", TokenType.PRINT },
            { "return", TokenType.RETURN },
            { "super", TokenType.SUPER },
            { "this", TokenType.THIS },
            { "true", TokenType.TRUE },
            { "var", TokenType.VAR },
            { "while", TokenType.WHILE }
        };


        public Scanner(string source)
        {
            Source = source;
        }

        public List<Token> ScanTokens() 
        {
            while (!IsAtEnd())
            {
                // we are at the beginning of the next lexeme.
                Start = Current;
                ScanToken();
            }
            Tokens.Add(new Token(TokenType.EOF, "", null, Line));
            return Tokens;
        }

        private void ScanToken()
        {
            char c = Advance();
            switch (c)
            {
                case '(': AddToken(TokenType.LEFT_PAREN); break;
                case ')': AddToken(TokenType.RIGHT_PAREN); break;
                case '{': AddToken(TokenType.LEFT_BRACE); break;
                case '}': AddToken(TokenType.RIGHT_BRACE); break;
                case ',': AddToken(TokenType.COMMA); break;
                case '.': AddToken(TokenType.DOT); break;
                case '-': AddToken(TokenType.MINUS); break;
                case '+': AddToken(TokenType.PLUS); break;
                case ';': AddToken(TokenType.SEMICOLON); break;
                case '*': AddToken(TokenType.STAR); break;
                case '!':
                    AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                    break;
                case '=':
                    AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                    break;
                case '<':
                    AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                    break;
                case '>':
                    AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                    break;
                case '/':
                    if (Match('/'))
                    {
                        // A comment goes until the end of the line.
                        while (Peek() != '\n' && !IsAtEnd()) Advance();
                    } 
                    else
                    {
                        AddToken(TokenType.SLASH);
                    }
                    break;
                case ' ':
                case '\r':
                case '\t':
                    // Ignore whitespace.
                    break;
                case '\n':
                    Line++;
                    break;
                case '"': ScanString(); break;
                
                default:
                    if (IsDigit(c))
                    {
                        Number();
                    }
                    else if (IsAlpha(c))
                    {
                        Identifier();
                    }
                    else
                    {
                        Lox.Error(Line, "Unexpected character.");
                    }
                    break;
            }
        }

        private void Identifier()
        {
            while (IsAlphaNumeric(Peek())) Advance();
            var text = Source[Start..Current];
            bool found = Keywords.TryGetValue(text, out var type);
            if (!found) type = TokenType.IDENTIFIER;
            AddToken(type);
        }

        private void Number()
        {
            while (IsDigit(Peek())) Advance();

            // Look for a fractional part.
            if (Peek() == '.' && IsDigit(PeekNext()))
            {
                // Consume the "."
                Advance();

                while (IsDigit(Peek())) Advance();
            }

            AddToken(TokenType.NUMBER, double.Parse(Source[Start..Current]));
        }

        private void ScanString()
        {
            while (Peek() != '"' &&  !IsAtEnd())
            {
                if (Peek() == '\n') Line++;
                Advance();
            }

            if (IsAtEnd())
            {
                Lox.Error(Line, "Unterminated string.");
                return;
            }

            // The closing ".
            Advance();

            // Trim the surrounding quotes
            var value = Source[(Start + 1)..(Current -1)];
            AddToken(TokenType.STRING, value);
        }

        private bool Match(char expected)
        {
            if (IsAtEnd()) return false;
            if (Source.ElementAt(Current) != expected) return false;

            Current++;
            return true;
        }

        private char Peek()
        {
            if (IsAtEnd()) return '\0';
            return Source.ElementAt(Current);
        }

        private char PeekNext()
        {
            if (Current + 1 >= Source.Length) return '\0';
            return Source.ElementAt(Current + 1);
        }

        private bool IsAlpha(char c)
        {
            return (c >= 'A' && c <= 'Z')
                || (c >= 'a' && c <= 'z')
                || (c == '_');
        }

        private bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }

        private bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private bool IsAtEnd()
        {
            return Current >= Source.Length;
        }

        private char Advance()
        {
            return Source.ElementAt(Current++);
        }

        private void AddToken(TokenType type)
        {
            AddToken(type, null);
        }

        private void AddToken(TokenType type, object? literal)
        {
            var text = Source[Start..Current];
            Tokens.Add(new Token(type, text, literal, Line));
        }
    }
}
