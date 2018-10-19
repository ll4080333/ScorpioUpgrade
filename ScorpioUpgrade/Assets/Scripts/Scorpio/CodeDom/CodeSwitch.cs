namespace Scorpio.CodeDom
{
    using Scorpio.CodeDom.Temp;
    using System;
    using System.Collections.Generic;

    public class CodeSwitch : CodeObject
    {
        internal TempCase[] Cases;
        internal CodeObject Condition;
        internal TempCase Default;

        internal void SetCases(List<TempCase> Cases)
        {
            this.Cases = Cases.ToArray();
        }
    }
}

