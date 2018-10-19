namespace Scorpio.CodeDom
{
    using System;
    using System.Collections.Generic;

    public class CodeCallFunction : CodeObject
    {
        public CodeObject Member;
        public CodeObject[] Parameters;
        public int ParametersCount;

        public CodeCallFunction(CodeObject member, List<CodeObject> parameters)
        {
            this.Member = member;
            this.Parameters = parameters.ToArray();
            this.ParametersCount = parameters.Count;
        }
    }
}

