﻿using System.Text;

namespace CSharpLox;

public class Lox
{
    static bool hadError = false;

    public static void Main(string[] args)
    {
        if (args.Length > 1) 
        {
            Console.WriteLine("Usage: CSharpLox [script]");
            Environment.Exit(64);
        } 
        else if (args.Length == 1)
        {
            RunFile(args[0]);
        }
        else
        {
            RunPrompt();
        }
    }

    private static void RunFile(string path)
    {
        byte[] bytes = File.ReadAllBytes(path);
        Run(Encoding.Default.GetString(bytes));

        // Indicate an error in the exit code.
        if (hadError) Environment.Exit(65);
    }

    private static void RunPrompt()
    {
        using StreamReader input = new(Console.OpenStandardInput());
        while (true)
        {
            Console.Write("> ");
            string? line = input.ReadLine();
            if (line == null) break;
            Run(line);
            hadError = false;
        }
    }

    private static void Run(string source)
    {
        Scanner scanner = new Scanner(source);
        List<Token> tokens = scanner.ScanTokens();
        Parser parser = new Parser(tokens);
        Expr expression = parser.Parse();

        // Stop if there was a syntax error.
        if (hadError) return;

        Console.WriteLine(new AstPrinter().Print(expression));
    }

    public static void Error(int line, string message)
    {
        Report(line, "", message);
    }

    private static void Report(int line, string where, string message)
    {
        Console.Error.WriteLine($"[line {line} ] Error {where}: ");
        hadError = true;
    }

    public static void Error(Token token, string message)
    {
        if (token.Type == TokenType.EOF)
        {
            Report(token.Line, "at end ", message);
        } 
        else
        {
            Report(token.Line, $" at '{token.Lexeme}' ",message);
        }
    }
}
