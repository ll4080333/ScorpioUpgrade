namespace Scorpio.Userdata
{
    using Scorpio;
    using System;
    using System.Reflection;

    public class UserdataField : UserdataVariable
    {
        private FieldInfo m_Field;

        public UserdataField(Script script, FieldInfo info)
        {
            base.m_Script = script;
            base.Name = info.Name;
            base.FieldType = info.FieldType;
            this.m_Field = info;
        }

        public override object GetValue(object obj)
        {
            return this.m_Field.GetValue(obj);
        }

        public override void SetValue(object obj, object val)
        {
            this.m_Field.SetValue(obj, val);
        }
    }
}

