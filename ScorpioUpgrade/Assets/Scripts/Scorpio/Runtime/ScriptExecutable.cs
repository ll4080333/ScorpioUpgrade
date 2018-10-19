namespace Scorpio.Runtime
{
    using System;
    using System.Collections.Generic;

    public class ScriptExecutable
    {
        private ScriptInstruction[] m_arrayScriptInstructions;
        public Executable_Block m_Block;
        private List<ScriptInstruction> m_listScriptInstructions;

        public ScriptExecutable(Executable_Block block)
        {
            this.m_Block = block;
            this.m_listScriptInstructions = new List<ScriptInstruction>();
        }

        public void AddScriptInstruction(ScriptInstruction val)
        {
            this.m_listScriptInstructions.Add(val);
        }

        public void EndScriptInstruction()
        {
            this.m_arrayScriptInstructions = this.m_listScriptInstructions.ToArray();
        }

        public ScriptInstruction[] ScriptInstructions
        {
            get
            {
                return this.m_arrayScriptInstructions;
            }
        }
    }
}

