namespace Scorpio.Variable
{
    using Scorpio;
    using System;

    public class ScriptNumberShort : ScriptNumber
    {
        private short m_Value;

        public ScriptNumberShort(Script script, short value) : base(script)
        {
            this.m_Value = value;
        }

        public override ScriptObject Clone()
        {
            return new ScriptNumberShort(base.m_Script, this.m_Value);
        }

        public override int BranchType
        {
            get
            {
                return 5;
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

        public short Value
        {
            get
            {
                return this.m_Value;
            }
        }
    }
}

