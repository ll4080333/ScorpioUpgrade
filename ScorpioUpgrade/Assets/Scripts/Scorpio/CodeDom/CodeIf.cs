namespace Scorpio.CodeDom
{
    using Scorpio.CodeDom.Temp;
    using System;
    using System.Collections.Generic;

    public class CodeIf : CodeObject
    {
        internal TempCondition Else;
        internal TempCondition[] ElseIf;
        internal int ElseIfCount;
        internal TempCondition If;

        internal void Init(List<TempCondition> ElseIf)
        {
            this.ElseIf = ElseIf.ToArray();
            this.ElseIfCount = ElseIf.Count;
        }
    }
}

