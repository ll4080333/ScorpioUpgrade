namespace Scorpio.CodeDom
{
    using System;

    public class CodeRegion : CodeObject
    {
        public CodeObject Context;

        public CodeRegion(CodeObject Context)
        {
            this.Context = Context;
        }
    }
}

