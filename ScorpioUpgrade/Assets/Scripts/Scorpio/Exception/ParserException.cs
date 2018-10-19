namespace Scorpio.Exception
{
    using Scorpio.Compiler;
    using System;

    public class ParserException : ScriptException
    {
        public ParserException(string strMessage, Token token) : base(string.Concat(new object[] { " Line:", token.SourceLine, "  Column:", token.SourceChar, "  Type:", token.Type, "  value[", token.Lexeme, "]    ", strMessage }))
        {
        }
    }
}

