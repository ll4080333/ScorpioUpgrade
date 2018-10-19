namespace Scorpio.Variable
{
    using Scorpio;
    using System;

    public class ScriptNumberUInt : ScriptNumber
    {
        private uint m_Value;

        public ScriptNumberUInt(Script script, uint value) : base(script)
        {
            this.m_Value = value;
        }

        public override ScriptObject Clone()
        {
            return new ScriptNumberUInt(base.m_Script, this.m_Value);
        }

        public override int BranchType
        {
            get
            {
                return 7;
            }
        }

        public override object KeyValue
        {
            get
            {
                return this.m_Value;
            }
        }

        public override object ObjectValue
        {
            get
            {
                return this.m_Value;
            }
        }

        public override ObjectType Type
        {
            get
            {
                return ObjectType.Number;
            }
        }

        public uint Value
        {
            get
            {
                return this.m_Value;
            }
        }
    }
}

