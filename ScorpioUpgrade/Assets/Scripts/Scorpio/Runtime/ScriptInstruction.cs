namespace Scorpio.Runtime
{
    using Scorpio.CodeDom;
    using System;

    public class ScriptInstruction
    {
        public Opcode opcode;
        public CodeObject operand0;
        public CodeObject operand1;
        public string opvalue;

        public ScriptInstruction(Opcode opcode, CodeObject operand0) : this(opcode, operand0, null)
        {
        }

        public ScriptInstruction(Opcode opcode, string opvalue)
        {
            this.opcode = opcode;
            this.opvalue = opvalue;
        }

        public ScriptInstruction(Opcode opcode, CodeObject operand0, CodeObject operand1)
        {
            this.opcode = opcode;
            this.operand0 = operand0;
            this.operand1 = operand1;
        }
    }
}

