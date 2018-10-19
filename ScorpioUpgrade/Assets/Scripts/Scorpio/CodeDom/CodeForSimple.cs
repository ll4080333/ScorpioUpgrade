namespace Scorpio.CodeDom
{
    using Scorpio.Runtime;
    using System;

    public class CodeForSimple : CodeObject
    {
        public CodeObject Begin;
        public ScriptExecutable BlockExecutable;
        public CodeObject Finished;
        public string Identifier;
        public CodeObject Step;
    }
}

