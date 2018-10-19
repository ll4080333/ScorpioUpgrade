namespace Scorpio.Userdata
{
    using Scorpio;
    using Scorpio.Compiler;
    using Scorpio.Exception;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class ScriptUserdataDelegate : ScriptUserdata
    {
        private Delegate m_Delegate;
        private object[] m_Objects;
        private List<FunctionParameter> m_Parameters;

        public ScriptUserdataDelegate(Script script, Delegate value) : base(script)
        {
            this.m_Parameters = new List<FunctionParameter>();
            this.m_Delegate = value;
            base.m_Value = value;
            base.m_ValueType = value.GetType();
            MethodInfo methodInfo = this.m_Delegate.GetMethodInfo();
            ParameterInfo[] parameters = methodInfo.GetParameters();
            bool flag = methodInfo.Name.Equals("__DynamicDelegate__");
            int num = flag ? (parameters.Length - 1) : parameters.Length;
            this.m_Objects = new object[num];
            for (int i = 0; i < num; i++)
            {
                ParameterInfo info2 = parameters[flag ? (i + 1) : i];
                this.m_Parameters.Add(new FunctionParameter(info2.ParameterType, info2.DefaultValue));
            }
        }

        public override object Call(ScriptObject[] parameters)
        {
            for (int i = 0; i < this.m_Parameters.Count; i++)
            {
                FunctionParameter parameter = this.m_Parameters[i];
                if (i >= parameters.Length)
                {
                    this.m_Objects[i] = parameter.DefaultValue;
                }
                else
                {
                    this.m_Objects[i] = Util.ChangeType(base.m_Script, parameters[i], parameter.ParameterType);
                }
            }
            return this.m_Delegate.DynamicInvoke(this.m_Objects);
        }

        public override ScriptObject Compute(Scorpio.Compiler.TokenType type, ScriptObject obj)
        {
            Scorpio.Compiler.TokenType type2 = type;
            if (type2 != Scorpio.Compiler.TokenType.Plus)
            {
                if (type2 != Scorpio.Compiler.TokenType.Minus)
                {
                    throw new ExecutionException(base.m_Script, "Delegate 不支持的运算符 " + type);
                }
                return base.m_Script.CreateObject(Delegate.Remove(this.m_Delegate, (Delegate) Util.ChangeType(base.m_Script, obj, base.ValueType)));
            }
            return base.m_Script.CreateObject(Delegate.Combine(this.m_Delegate, (Delegate) Util.ChangeType(base.m_Script, obj, base.ValueType)));
        }

        public override ScriptObject GetValue(object key)
        {
            if (!(key is string) || !key.Equals("Type"))
            {
                throw new ExecutionException(base.m_Script, "EventInfo GetValue只支持 Type 一个变量");
            }
            return base.m_Script.CreateObject(base.ValueType);
        }

        private class FunctionParameter
        {
            public object DefaultValue;
            public Type ParameterType;

            public FunctionParameter(Type type, object def)
            {
                this.ParameterType = type;
                this.DefaultValue = def;
            }
        }
    }
}

