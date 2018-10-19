namespace Scorpio
{
    using System;

    public class ScriptEnum : ScriptObject
    {
        private System.Type m_EnumType;
        private object m_Value;

        public ScriptEnum(Script script, object obj) : base(script)
        {
            this.m_Value = obj;
            this.m_EnumType = this.m_Value.GetType();
        }

        public System.Type EnumType
        {
            get
            {
                return this.m_EnumType;
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
                return ObjectType.Enum;
            }
        }
    }
}

