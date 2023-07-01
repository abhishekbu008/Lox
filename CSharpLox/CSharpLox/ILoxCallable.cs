﻿namespace CSharpLox
{
    public interface ILoxCallable
    {
        int Arity();
        object? Call(Interpreter interpreter, List<object?> arguments);
    }
}