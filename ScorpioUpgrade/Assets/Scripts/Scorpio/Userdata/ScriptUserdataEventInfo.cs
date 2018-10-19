namespace Scorpio.Userdata
{
    using Scorpio;
    using Scorpio.Compiler;
    using Scorpio.Exception;
    using System;
    using System.Reflection;

    public class ScriptUserdataEventInfo : ScriptUserdata
    {
        private EventInfo m_EventInfo;
        private Type m_HandlerType;
        private object m_Target;

        public ScriptUserdataEventInfo(Script script, BridgeEventInfo value) : base(script)
        {
            this.m_Target = value.target;
            this.m_EventInfo = value.eventInfo;
            this.m_HandlerType = this.m_EventInfo.EventHandlerType;
        }

        public override ScriptObject AssignCompute(Scorpio.Compiler.TokenType type, ScriptObject obj)
        {
            Scorpio.Compiler.TokenType type2 = type;
            if (type2 != Scorpio.Compiler.TokenType.AssignPlus)
            {
                if (type2 != Scorpio.Compiler.TokenType.AssignMinus)
                {
                    throw new ExecutionException(base.m_Script, "event 不支持的运算符 " + type);
                }
            }
            else
            {
                this.m_EventInfo.AddEventHandler(this.m_Target, (Delegate) Util.ChangeType(base.m_Script, obj, this.m_HandlerType));
                return base.m_Script.Null;
            }
            this.m_EventInfo.RemoveEventHandler(this.m_Target, (Delegate) Util.ChangeType(base.m_Script, obj, this.m_HandlerType));
            return base.m_Script.Null;
        }

        public override ScriptObject GetValue(object key)
        {
            if (!(key is string) || !key.Equals("Type"))
            {
                throw new ExecutionException(base.m_Script, "EventInfo GetValue只支持 Type 一个变量");
            }
            return base.m_Script.CreateObject(this.m_HandlerType);
        }
    }
}

