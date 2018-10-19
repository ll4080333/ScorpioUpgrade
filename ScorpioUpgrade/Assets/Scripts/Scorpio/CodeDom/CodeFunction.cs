namespace Scorpio.CodeDom
{
    using Scorpio.Function;
    using System;

    public class CodeFunction : CodeObject
    {
        public ScriptScriptFunction Func;

        public CodeFunction(ScriptScriptFunction func)
        {
            this.Func = func;
        }

        public CodeFunction(ScriptScriptFunction func, string breviary, int line) : base(breviary, line)
        {
            this.Func = func;
        }
    }
}

