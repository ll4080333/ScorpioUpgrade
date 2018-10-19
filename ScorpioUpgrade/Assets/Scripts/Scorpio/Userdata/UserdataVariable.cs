namespace Scorpio.Userdata
{
    using Scorpio;
    using System;

    public abstract class UserdataVariable
    {
        public Type FieldType;
        protected Script m_Script;
        public string Name;

        protected UserdataVariable()
        {
        }

        public abstract object GetValue(object obj);
        public abstract void SetValue(object obj, object val);
    }
}

