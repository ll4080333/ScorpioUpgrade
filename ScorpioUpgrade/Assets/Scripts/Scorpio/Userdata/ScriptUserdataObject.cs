namespace Scorpio.Userdata
{
    using Scorpio;
    using Scorpio.Compiler;
    using Scorpio.Exception;
    using Scorpio.Variable;
    using System;
    using System.Collections.Generic;

    public class ScriptUserdataObject : ScriptUserdata
    {
        protected Dictionary<string, ScriptObject> m_Methods;
        protected UserdataType m_UserdataType;

        public ScriptUserdataObject(Script script, object value, UserdataType type) : base(script)
        {
            this.m_Methods = new Dictionary<string, ScriptObject>();
            base.m_Value = value;
            base.m_ValueType = value.GetType();
            this.m_UserdataType = type;
        }

        public override ScriptObject Compute(Scorpio.Compiler.TokenType type, ScriptObject obj)
        {
            ScorpioMethod computeMethod = this.m_UserdataType.GetComputeMethod(type);
            if (computeMethod == null)
            {
                throw new ExecutionException(base.m_Script, "找不到运算符重载 " + type);
            }
            return base.m_Script.CreateObject(computeMethod.Call(new ScriptObject[] { this, obj }));
        }

        public override ScriptObject GetValue(object key)
        {
            string str = key as string;
            if (str == null)
            {
                throw new ExecutionException(base.m_Script, "Object GetValue只支持String类型");
            }
            if (this.m_Methods.ContainsKey(str))
            {
                return this.m_Methods[str];
            }
            object obj2 = this.m_UserdataType.GetValue(base.m_Value, str);
            if (obj2 is UserdataMethod)
            {
                UserdataMethod method = (UserdataMethod) obj2;
                ScriptObject obj3 = base.m_Script.CreateObject(method.IsStatic ? ((object) new ScorpioStaticMethod(str, method)) : ((object) new ScorpioObjectMethod(base.m_Value, str, method)));
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
                throw new ExecutionException(base.m_Script, "Object SetValue只支持String类型");
            }
            this.m_UserdataType.SetValue(base.m_Value, name, value);
        }
    }
}

