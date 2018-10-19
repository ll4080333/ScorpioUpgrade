namespace Scorpio.Userdata
{
    using System;
    using System.Reflection;

    public class BridgeEventInfo
    {
        public EventInfo eventInfo;
        public object target;

        public BridgeEventInfo(object target, EventInfo eventInfo)
        {
            this.target = target;
            this.eventInfo = eventInfo;
        }
    }
}

