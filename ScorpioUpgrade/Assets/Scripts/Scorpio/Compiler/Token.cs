namespace Scorpio.Compiler
{
    using System;
    using System.Runtime.CompilerServices;

    public class Token
    {
        public Token(Scorpio.Compiler.TokenType tokenType, object lexeme, int sourceLine, int sourceChar)
        {
            this.Type = tokenType;
            this.Lexeme = lexeme;
            this.SourceLine = sourceLine + 1;
            this.SourceChar = sourceChar;
        }

        public override string ToString()
        {
            return (this.Type.ToString() + "(" + this.Lexeme.ToString() + ")");
        }

        public object Lexeme { get; private set; }

        public int SourceChar { get; private set; }

        public int SourceLine { get; private set; }

        public Scorpio.Compiler.TokenType Type { get; private set; }
    }
}

