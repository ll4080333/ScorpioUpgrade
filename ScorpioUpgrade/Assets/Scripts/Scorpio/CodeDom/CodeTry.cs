namespace Scorpio.CodeDom
{
    using Scorpio.Runtime;
    using System;

    public class CodeTry : CodeObject
    {
        public ScriptExecutable CatchExecutable;
        public string Identifier;
        public ScriptExecutable TryExecutable;
    }
}

