namespace Scorpio.CodeDom
{
    using Scorpio.Exception;
    using System;

    public class CodeObject
    {
        public bool Minus;
        public bool Negative;
        public bool Not;
        public Scorpio.Exception.StackInfo StackInfo;

        public CodeObject()
        {
        }

        public CodeObject(string breviary, int line)
        {
            this.StackInfo = new Scorpio.Exception.StackInfo(breviary, line);
        }
    }
}

