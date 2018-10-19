namespace Scorpio.Variable
{
    using Scorpio;
    using System;

    public class ScriptNumberSByte : ScriptNumber
    {
        private sbyte m_Value;

        public ScriptNumberSByte(Script script, sbyte value) : base(script)
        {
            this.m_Value = value;
        }

        public override ScriptObject Clone()
        {
            return new ScriptNumberSByte(base.m_Script, this.m_Value);
        }

        public override int BranchType
        {
            get
            {
                return 3;
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

        public sbyte Value
        {
            get
            {
                return this.m_Value;
            }
        }
    }
}

