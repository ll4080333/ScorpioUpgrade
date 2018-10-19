namespace Scorpio.CodeDom
{
    using Scorpio;
    using System;

    public class CodeScriptObject : CodeObject
    {
        public ScriptObject Object;

        public CodeScriptObject(Script script, object obj)
        {
            this.Object = script.CreateObject(obj);
            this.Object.Name = null == obj?"":obj.ToString();
        }
    }
}

