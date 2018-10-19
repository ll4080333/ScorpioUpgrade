namespace Scorpio.Userdata
{
    using Scorpio;
    using Scorpio.Exception;
    using Scorpio.Variable;
    using System;
    using System.Collections.Generic;

    public class ScriptUserdataObjectType : ScriptUserdata
    {
        protected Dictionary<string, ScriptObject> m_Methods;
        protected UserdataType m_UserdataType;

        public ScriptUserdataObjectType(Script script, Type value, UserdataType type) : base(script)
        {
            this.m_Methods = new Dictionary<string, ScriptObject>();
            base.m_Value = value;
            base.m_ValueType = value;
            this.m_UserdataType = type;
        }

        public override object Call(ScriptObject[] parameters)
        {
            return this.m_UserdataType.CreateInstance(parameters);
        }

        public override ScriptObject GetValue(object key)
        {
            string str = key as string;
            if (str == null)
            {
                throw new ExecutionException(base.m_Script, "ObjectType GetValue只支持String类型");
            }
            if (this.m_Methods.ContainsKey(str))
            {
                return this.m_Methods[str];
            }
            object obj2 = this.m_UserdataType.GetValue(null, str);
            if (obj2 is UserdataMethod)
            {
                UserdataMethod method = (UserdataMethod) obj2;
                ScriptObject obj3 = base.m_Script.CreateObject(method.IsStatic ? ((object) new ScorpioStaticMethod(str, method)) : ((object) new ScorpioTypeMethod(base.m_Script, str, method, base.m_ValueType)));
                this.m_Methods.Add(str, obj3);
                return obj3;
            }
            return base.m_Script.CreateObject(obj2);
        }

        public override void SetValue(object key, ScriptObject value)
        {
            string name = key as string;
            if (name == null)
            {
                throw new ExecutionException(base.m_Script, "ObjectType SetValue只支持String类型");
            }
            this.m_UserdataType.SetValue(null, name, value);
        }
    }
}

