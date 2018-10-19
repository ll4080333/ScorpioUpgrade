namespace Scorpio.Variable
{
    using Scorpio;
    using System;

    public class ScriptNumberULong : ScriptNumber
    {
        private ulong m_Value;

        public ScriptNumberULong(Script script, ulong value) : base(script)
        {
            this.m_Value = value;
        }

        public override ScriptObject Clone()
        {
            return new ScriptNumberULong(base.m_Script, this.m_Value);
        }

        public override int BranchType
        {
            get
            {
                return 8;
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

        public ulong Value
        {
            get
            {
                return this.m_Value;
            }
        }
    }
}

