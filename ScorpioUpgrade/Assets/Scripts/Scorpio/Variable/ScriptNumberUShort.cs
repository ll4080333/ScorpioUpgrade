namespace Scorpio.Variable
{
    using Scorpio;
    using System;

    public class ScriptNumberUShort : ScriptNumber
    {
        private ushort m_Value;

        public ScriptNumberUShort(Script script, ushort value) : base(script)
        {
            this.m_Value = value;
        }

        public override ScriptObject Clone()
        {
            return new ScriptNumberUShort(base.m_Script, this.m_Value);
        }

        public override int BranchType
        {
            get
            {
                return 6;
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

        public ushort Value
        {
            get
            {
                return this.m_Value;
            }
        }
    }
}

