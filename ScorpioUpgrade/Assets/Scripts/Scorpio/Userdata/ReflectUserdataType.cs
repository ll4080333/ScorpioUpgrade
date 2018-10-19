namespace Scorpio.Userdata
{
    using Scorpio;
    using Scorpio.Compiler;
    using Scorpio.Exception;
    using Scorpio.Variable;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class ReflectUserdataType : UserdataType
    {
        private UserdataMethod m_Constructor;
        private Dictionary<string, UserdataMethod> m_Functions;
        private bool m_InitializeConstructor;
        private bool m_InitializeMethods;
        private List<MethodInfo> m_Methods;
        private Dictionary<string, ScriptUserdata> m_NestedTypes;
        private Dictionary<string, ScorpioMethod> m_ScorpioMethods;
        private Dictionary<string, UserdataVariable> m_Variables;

        public ReflectUserdataType(Script script, Type type) : base(script, type)
        {
            this.m_InitializeConstructor = false;
            this.m_InitializeMethods = false;
            this.m_Methods = new List<MethodInfo>();
            this.m_Variables = new Dictionary<string, UserdataVariable>();
            this.m_NestedTypes = new Dictionary<string, ScriptUserdata>();
            this.m_Functions = new Dictionary<string, UserdataMethod>();
            this.m_ScorpioMethods = new Dictionary<string, ScorpioMethod>();
        }

        public override void AddExtensionMethod(MethodInfo method)
        {
            if (!this.m_Methods.Contains(method))
            {
                this.m_Methods.Add(method);
            }
        }

        public override object CreateInstance(ScriptObject[] parameters)
        {
            this.InitializeConstructor();
            return this.m_Constructor.Call(null, parameters);
        }

        public override ScorpioMethod GetComputeMethod(Scorpio.Compiler.TokenType type)
        {
            Scorpio.Compiler.TokenType type2 = type;
            if (type2 != Scorpio.Compiler.TokenType.Plus)
            {
                switch (type2)
                {
                    case Scorpio.Compiler.TokenType.Multiply:
                        return (base.GetValue(null, "op_Multiply") as ScorpioMethod);

                    case Scorpio.Compiler.TokenType.Divide:
                        return (base.GetValue(null, "op_Division") as ScorpioMethod);

                    case Scorpio.Compiler.TokenType.Minus:
                        return (base.GetValue(null, "op_Subtraction") as ScorpioMethod);
                }
            }
            else
            {
                return (base.GetValue(null, "op_Addition") as ScorpioMethod);
            }
            return null;
        }

        private UserdataMethod GetMethod(string name)
        {
            this.InitializeMethods();
            foreach (MethodInfo info in this.m_Methods)
            {
                if (info.Name.Equals(name))
                {
                    ReflectUserdataMethod method = new ReflectUserdataMethod(base.m_Script, base.m_Type, name, this.m_Methods);
                    this.m_Functions.Add(name, method);
                    return method;
                }
            }
            return null;
        }

        private ScriptUserdata GetNestedType(string name)
        {
            Type nestedType = base.m_Type.GetTypeInfo().GetNestedType(name, BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if (nestedType != null)
            {
                ScriptUserdata userdata = base.m_Script.CreateUserdata(nestedType);
                this.m_NestedTypes.Add(name, userdata);
                return userdata;
            }
            return null;
        }

        public override object GetValue_impl(object obj, string name)
        {
            if (this.m_Functions.ContainsKey(name))
            {
                return this.m_Functions[name];
            }
            if (this.m_NestedTypes.ContainsKey(name))
            {
                return this.m_NestedTypes[name];
            }
            UserdataVariable variable = this.GetVariable(name);
            if (variable != null)
            {
                return variable.GetValue(obj);
            }
            ScriptUserdata nestedType = this.GetNestedType(name);
            if (nestedType != null)
            {
                return nestedType;
            }
            UserdataMethod method = this.GetMethod(name);
            if (method == null)
            {
                throw new ExecutionException(base.m_Script, "GetValue Type[" + base.m_Type.ToString() + "] 变量 [" + name + "] 不存在");
            }
            return method;
        }

        private UserdataVariable GetVariable(string name)
        {
            if (this.m_Variables.ContainsKey(name))
            {
                return this.m_Variables[name];
            }
            FieldInfo field = base.m_Type.GetTypeInfo().GetField(name);
            if (field != null)
            {
                UserdataVariable variable;
                this.m_Variables[name] = variable = new UserdataField(base.m_Script, field);
                return variable;
            }
            PropertyInfo property = base.m_Type.GetTypeInfo().GetProperty(name, BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if (property != null)
            {
                UserdataVariable variable2;
                this.m_Variables[name] = variable2 = new UserdataProperty(base.m_Script, property);
                return variable2;
            }
            EventInfo info = base.m_Type.GetTypeInfo().GetEvent(name);
            if (info != null)
            {
                UserdataVariable variable3;
                this.m_Variables[name] = variable3 = new UserdataEvent(base.m_Script, info);
                return variable3;
            }
            return null;
        }

        private void InitializeConstructor()
        {
            if (!this.m_InitializeConstructor)
            {
                this.m_InitializeConstructor = true;
                this.m_Constructor = new ReflectUserdataMethod(base.m_Script, base.m_Type, base.m_Type.ToString(), base.m_Type.GetTypeInfo().GetConstructors());
            }
        }

        private void InitializeMethods()
        {
            if (!this.m_InitializeMethods)
            {
                this.m_InitializeMethods = true;
                this.m_Methods.AddRange(base.m_Type.GetTypeInfo().GetMethods(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance));
            }
        }

        public override void SetValue_impl(object obj, string name, ScriptObject value)
        {
            UserdataVariable variable = this.GetVariable(name);
            if (variable == null)
            {
                throw new ExecutionException(base.m_Script, string.Concat(new object[] { "SetValue Type[", base.m_Type, "] 变量 [", name, "] 不存在" }));
            }
            try
            {
                variable.SetValue(obj, Util.ChangeType(base.m_Script, value, variable.FieldType));
            }
            catch (Exception exception)
            {
                throw new ExecutionException(base.m_Script, "SetValue 出错 源类型:" + (((value == null) || value.IsNull) ? "null" : value.ObjectValue.GetType().Name) + " 目标类型:" + variable.FieldType.Name + " : " + exception.ToString());
            }
        }
    }
}

