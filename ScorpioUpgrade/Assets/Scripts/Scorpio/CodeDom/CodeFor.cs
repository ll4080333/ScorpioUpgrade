namespace Scorpio.CodeDom
{
    using Scorpio.Runtime;

    public class CodeFor : CodeObject
    {
        public ScriptExecutable BeginExecutable;
        public ScriptExecutable BlockExecutable;
        public CodeObject Condition;
        public ScriptExecutable LoopExecutable;
    }
}

