namespace Scorpio
{
    using System;

    public abstract class ScriptUserdata : ScriptObject
    {
        protected object m_Value;
        protected System.Type m_ValueType;

        public ScriptUserdata(Script script) : base(script)
        {
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
                return ObjectType.UserData;
            }
        }

        public object Value
        {
            get
            {
                return this.m_Value;
            }
        }

        public System.Type ValueType
        {
            get
            {
                return this.m_ValueType;
            }
        }
    }
}

