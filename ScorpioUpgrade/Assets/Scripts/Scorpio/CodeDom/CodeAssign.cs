namespace Scorpio.CodeDom
{
    using Scorpio.Compiler;
    using System;

    public class CodeAssign : CodeObject
    {
        public Scorpio.Compiler.TokenType AssignType;
        public CodeMember member;
        public CodeObject value;

        public CodeAssign(CodeMember member, CodeObject value, Scorpio.Compiler.TokenType assignType, string breviary, int line) : base(breviary, line)
        {
            this.member = member;
            this.value = value;
            this.AssignType = assignType;
        }
    }
}

