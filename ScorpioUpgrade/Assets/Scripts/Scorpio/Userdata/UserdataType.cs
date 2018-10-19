namespace Scorpio.Userdata
{
    using Scorpio;
    using Scorpio.Compiler;
    using Scorpio.Exception;
    using Scorpio.Variable;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public abstract class UserdataType
    {
        protected Dictionary<string, string> m_Rename = new Dictionary<string, string>();
        protected Script m_Script;
        protected Type m_Type;

        public UserdataType(Script script, Type type)
        {
            this.m_Script = script;
            this.m_Type = type;
        }

        public abstract void AddExtensionMethod(MethodInfo method);
        public abstract object CreateInstance(ScriptObject[] parameters);
        public abstract ScorpioMethod GetComputeMethod(Scorpio.Compiler.TokenType type);
        public object GetValue(object obj, string name)
        {
            if (!this.m_Rename.ContainsKey(name))
            {
                return this.GetValue_impl(obj, name);
            }
            return this.GetValue_impl(obj, this.m_Rename[name]);
        }

        public abstract object GetValue_impl(object obj, string name);
        public ScriptUserdata MakeGenericType(Type[] parameters)
        {
            Type[] genericArguments = this.m_Type.GetTypeInfo().GetGenericArguments();
            if (genericArguments.Length != parameters.Length)
            {
                throw new ExecutionException(this.m_Script, string.Concat(new object[] { this.m_Type, " 泛型类个数错误 需要:", genericArguments.Length, " 传入:", parameters.Length }));
            }
            int length = genericArguments.Length;
            for (int i = 0; i < length; i++)
            {
                if (!genericArguments[i].GetTypeInfo().BaseType.GetTypeInfo().IsAssignableFrom(parameters[i]))
                {
                    throw new ExecutionException(this.m_Script, string.Concat(new object[] { this.m_Type, "泛型类第", i + 1, "个参数失败 需要:", genericArguments[i].GetTypeInfo().BaseType, " 传入:", parameters[i] }));
                }
            }
            return this.m_Script.CreateUserdata(this.m_Type.MakeGenericType(parameters));
        }

        public void Rename(string name1, string name2)
        {
            this.m_Rename[name2] = name1;
        }

        public void SetValue(object obj, string name, ScriptObject value)
        {
            if (this.m_Rename.ContainsKey(name))
            {
                this.SetValue_impl(obj, this.m_Rename[name], value);
            }
            else
            {
                this.SetValue_impl(obj, name, value);
            }
        }

        public abstract void SetValue_impl(object obj, string name, ScriptObject value);
    }
}

