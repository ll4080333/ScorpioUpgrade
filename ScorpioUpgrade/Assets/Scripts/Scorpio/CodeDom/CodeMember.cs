namespace Scorpio.CodeDom
{
    using System;

    public class CodeMember : CodeObject
    {
        public CALC Calc;
        public CodeObject MemberObject;
        public object MemberValue;
        public CodeObject Parent;
        public MEMBER_TYPE Type;

        public CodeMember(string name) : this(name, null)
        {
        }

        public CodeMember(CodeObject member, CodeObject parent)
        {
            this.MemberObject = member;
            this.Parent = parent;
            this.Type = MEMBER_TYPE.OBJECT;
        }

        public CodeMember(object value, CodeObject parent)
        {
            this.Parent = parent;
            this.MemberValue = value;
            this.Type = MEMBER_TYPE.VALUE;
        }
    }
}

