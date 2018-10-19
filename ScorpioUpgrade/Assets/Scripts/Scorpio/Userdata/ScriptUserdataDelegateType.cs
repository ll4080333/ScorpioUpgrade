namespace Scorpio.Userdata
{
    using Scorpio;
    using System;

    public class ScriptUserdataDelegateType : ScriptUserdata
    {
        private static DelegateTypeFactory m_Factory;

        public ScriptUserdataDelegateType(Script script, Type value) : base(script)
        {
            base.m_Value = value;
            base.m_ValueType = value;
        }

        public override object Call(ScriptObject[] parameters)
        {
            if (m_Factory == null)
            {
                return null;
            }
            return m_Factory.CreateDelegate(base.m_Script, base.m_ValueType, parameters[0] as ScriptFunction);
        }

        public static void SetFactory(DelegateTypeFactory factory)
        {
            m_Factory = factory;
        }
    }
}

