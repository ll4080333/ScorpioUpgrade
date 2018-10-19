namespace Scorpio.CodeDom
{
    using System;
    using System.Collections.Generic;

    public class CodeArray : CodeObject
    {
        public List<CodeObject> _Elements = new List<CodeObject>();
        public CodeObject[] Elements;

        public void Init()
        {
            this.Elements = this._Elements.ToArray();
        }
    }
}

