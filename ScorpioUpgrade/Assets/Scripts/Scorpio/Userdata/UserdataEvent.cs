namespace Scorpio.Userdata
{
    using Scorpio;
    using Scorpio.Exception;
    using System;
    using System.Reflection;

    public class UserdataEvent : UserdataVariable
    {
        private EventInfo m_Event;

        public UserdataEvent(Script script, EventInfo info)
        {
            base.m_Script = script;
            base.Name = info.Name;
            base.FieldType = info.EventHandlerType;
            this.m_Event = info;
        }

        public override object GetValue(object obj)
        {
            return new BridgeEventInfo(obj, this.m_Event);
        }

        public override void SetValue(object obj, object val)
        {
            throw new ExecutionException(base.m_Script, "Event [" + base.Name + "] 不支持SetValue");
        }
    }
}

