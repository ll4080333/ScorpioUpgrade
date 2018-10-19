namespace Scorpio.CodeDom
{
    using Scorpio.Compiler;
    using System;

    public class CodeOperator : CodeObject
    {
        public CodeObject Left;
        public Scorpio.Compiler.TokenType Operator;
        public CodeObject Right;

        public CodeOperator(CodeObject Right, CodeObject Left, Scorpio.Compiler.TokenType type, string breviary, int line) : base(breviary, line)
        {
            this.Left = Left;
            this.Right = Right;
            this.Operator = type;
        }
    }
}

