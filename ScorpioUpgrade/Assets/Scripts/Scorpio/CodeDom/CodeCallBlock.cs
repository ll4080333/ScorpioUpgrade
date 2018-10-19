namespace Scorpio.CodeDom
{
    using Scorpio.Runtime;
    using System;

    public class CodeCallBlock : CodeObject
    {
        public ScriptExecutable Executable;

        public CodeCallBlock(ScriptExecutable executable)
        {
            this.Executable = executable;
        }
    }
}

