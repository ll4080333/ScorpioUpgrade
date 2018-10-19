namespace Scorpio.CodeDom
{
    using Scorpio.Function;
    using System;
    using System.Collections.Generic;

    public class CodeTable : CodeObject
    {
        public List<ScriptScriptFunction> _Functions = new List<ScriptScriptFunction>();
        public List<TableVariable> _Variables = new List<TableVariable>();
        public ScriptScriptFunction[] Functions;
        public TableVariable[] Variables;

        public void Init()
        {
            this.Variables = this._Variables.ToArray();
            this.Functions = this._Functions.ToArray();
        }

        public class TableVariable
        {
            public object key;
            public CodeObject value;

            public TableVariable(object key, CodeObject value)
            {
                this.key = key;
                this.value = value;
            }
        }
    }
}

