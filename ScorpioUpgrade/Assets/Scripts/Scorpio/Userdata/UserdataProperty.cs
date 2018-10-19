namespace Scorpio.Userdata
{
    using Scorpio;
    using Scorpio.Exception;
    using System;
    using System.Reflection;

    public class UserdataProperty : UserdataVariable
    {
        private PropertyInfo m_Property;

        public UserdataProperty(Script script, PropertyInfo info)
        {
            base.m_Script = script;
            base.Name = info.Name;
            base.FieldType = info.PropertyType;
            this.m_Property = info;
        }

        public override object GetValue(object obj)
        {
            if (!this.m_Property.CanRead)
            {
                throw new ExecutionException(base.m_Script, "Property [" + base.Name + "] 不支持GetValue");
            }
            return this.m_Property.GetValue(obj, null);
        }

        public override void SetValue(object obj, object val)
        {
            if (!this.m_Property.CanWrite)
            {
                throw new ExecutionException(base.m_Script, "Property [" + base.Name + "] 不支持SetValue");
            }
            this.m_Property.SetValue(obj, val, null);
        }
    }
}

