namespace Scorpio
{
    using System;

    public class ScriptBoolean : ScriptObject
    {
        private bool m_Value;

        public ScriptBoolean(Script script, bool value) : base(script)
        {
            this.m_Value = value;
        }

        public override bool LogicOperation()
        {
            return this.m_Value;
        }

        public override string ToJson()
        {
            if (!this.m_Value)
            {
                return "false";
            }
            return "true";
        }

        public override string ToString()
        {
            if (!this.m_Value)
            {
                return "false";
            }
            return "true";
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
                return ObjectType.Boolean;
            }
        }

        public bool Value
        {
            get
            {
                return this.m_Value;
            }
        }
    }
}

