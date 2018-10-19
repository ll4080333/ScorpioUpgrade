namespace Scorpio.Userdata
{
    using Scorpio;
    using Scorpio.Exception;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class UserdataMethod
    {
        private int m_Count;
        private int m_GenericCount;
        private FunctionBase[] m_GenericMethods;
        private bool m_IsStatic;
        private string m_MethodName;
        private FunctionBase[] m_Methods;
        private Script m_Script;
        private Type m_Type;

        public UserdataMethod()
        {
        }

        private UserdataMethod(Script script, Type type, string methodName, List<MethodInfo> methods)
        {
            this.m_Script = script;
            this.m_IsStatic = methods[0].IsStatic;
            List<MethodBase> list = new List<MethodBase>();
            list.AddRange(methods.ToArray());
            this.Initialize_impl(type, methodName, list);
        }

        public object Call(object obj, ScriptObject[] parameters)
        {
            FunctionBase base2 = null;
            FunctionBase base3 = null;
            for (int i = 0; i < this.m_Count; i++)
            {
                base3 = this.m_Methods[i];
                if ((!base3.Params && !base3.HasDefault) && this.CheckNormalType(parameters, base3.ParameterType, base3.ParameterCount))
                {
                    base2 = base3;
                    goto Label_00DF;
                }
            }
            for (int j = 0; j < this.m_Count; j++)
            {
                base3 = this.m_Methods[j];
                if ((!base3.Params && base3.HasDefault) && this.CheckDefaultType(parameters, base3.ParameterType, base3.RequiredNumber, base3.ParameterCount))
                {
                    base2 = base3;
                    goto Label_00DF;
                }
            }
            for (int k = 0; k < this.m_Count; k++)
            {
                base3 = this.m_Methods[k];
                if (base3.Params && this.CheckArgsType(parameters, base3.ParameterType, base3.ParamType, base3.RequiredNumber, base3.ParameterCount))
                {
                    base2 = base3;
                    break;
                }
            }
        Label_00DF:
            try
            {
                if (base2 != null)
                {
                    int requiredNumber = base2.RequiredNumber;
                    int parameterCount = base2.ParameterCount;
                    int length = parameters.Length;
                    object[] args = base2.Args;
                    if (base2.Params)
                    {
                        for (int n = 0; n < parameterCount; n++)
                        {
                            args[n] = (n >= length) ? base2.DefaultParameter[n - requiredNumber] : Util.ChangeType(this.m_Script, parameters[n], base2.ParameterType[n]);
                        }
                        if (length > parameterCount)
                        {
                            Array array = Array.CreateInstance(base2.ParamType, (int) (length - parameterCount));
                            for (int num8 = parameterCount; num8 < length; num8++)
                            {
                                array.SetValue(Util.ChangeType(this.m_Script, parameters[num8], base2.ParamType), (int) (num8 - parameterCount));
                            }
                            args[parameterCount] = array;
                        }
                        else
                        {
                            args[parameterCount] = Array.CreateInstance(base2.ParamType, 0);
                        }
                        return base2.Invoke(obj, this.m_Type);
                    }
                    for (int m = 0; m < parameterCount; m++)
                    {
                        args[m] = (m >= length) ? base2.DefaultParameter[m - requiredNumber] : Util.ChangeType(this.m_Script, parameters[m], base2.ParameterType[m]);
                    }
                    return base2.Invoke(obj, this.m_Type);
                }
            }
            catch (Exception exception)
            {
                throw new ExecutionException(this.m_Script, "Type[" + this.m_Type.ToString() + "] 调用函数出错 [" + this.MethodName + "] : " + exception.ToString());
            }
            throw new ExecutionException(this.m_Script, "Type[" + this.m_Type.ToString() + "] 找不到合适的函数 [" + this.MethodName + "]");
        }

        private bool CheckArgsType(ScriptObject[] parameters, Type[] types, Type paramType, int requiredNumber, int count)
        {
            int length = parameters.Length;
            if (length < requiredNumber)
            {
                return false;
            }
            for (int i = 0; i < length; i++)
            {
                if (!Util.CanChangeType(parameters[i], (i >= count) ? paramType : types[i]))
                {
                    return false;
                }
            }
            return true;
        }

        private bool CheckDefaultType(ScriptObject[] parameters, Type[] types, int requiredNumber, int count)
        {
            int length = parameters.Length;
            if ((length < requiredNumber) || (length > count))
            {
                return false;
            }
            for (int i = 0; i < length; i++)
            {
                if (!Util.CanChangeType(parameters[i], types[i]))
                {
                    return false;
                }
            }
            return true;
        }

        private bool CheckNormalType(ScriptObject[] pars, Type[] types, int count)
        {
            if (pars.Length != count)
            {
                return false;
            }
            for (int i = 0; i < count; i++)
            {
                if (!Util.CanChangeType(pars[i], types[i]))
                {
                    return false;
                }
            }
            return true;
        }

        protected void Initialize(Script script, Type type, string methodName, List<MethodInfo> methods)
        {
            this.m_Script = script;
            List<MethodBase> list = new List<MethodBase>();
            foreach (MethodInfo info in methods)
            {
                if (info.Name.Equals(methodName))
                {
                    list.Add(info);
                }
            }
            this.m_IsStatic = (list.Count != 0) && (!Util.IsExtensionMethod(list[0]) && list[0].IsStatic);
            this.Initialize_impl(type, methodName, list);
        }

        protected void Initialize(Script script, Type type, string methodName, ConstructorInfo[] cons)
        {
            this.m_Script = script;
            this.m_IsStatic = false;
            List<MethodBase> methods = new List<MethodBase>();
            methods.AddRange(cons);
            this.Initialize_impl(type, methodName, methods);
        }

        protected void Initialize(bool isStatic, Script script, Type type, string methodName, ScorpioMethodInfo[] methods, IScorpioFastReflectMethod fastMethod)
        {
            this.m_Script = script;
            this.m_IsStatic = isStatic;
            this.m_Type = type;
            this.m_MethodName = methodName;
            List<FunctionBase> list = new List<FunctionBase>();
            foreach (ScorpioMethodInfo info in methods)
            {
                list.Add(new FunctionFastMethod(fastMethod, info.ParameterType, info.ParamType, info.Params, info.ParameterTypes));
            }
            this.m_Methods = list.ToArray();
            this.m_Count = this.m_Methods.Length;
        }

        private void Initialize_impl(Type type, string methodName, List<MethodBase> methods)
        {
            this.m_Type = type;
            this.m_MethodName = methodName;
            List<FunctionBase> list = new List<FunctionBase>();
            List<FunctionBase> list2 = new List<FunctionBase>();
            bool @params = false;
            Type paramType = null;
            List<Type> list3 = new List<Type>();
            List<object> list4 = new List<object>();
            int count = methods.Count;
            MethodBase method = null;
            for (int i = 0; i < count; i++)
            {
                FunctionBase base3;
                method = methods[i];
                @params = false;
                paramType = null;
                list3.Clear();
                list4.Clear();
                ParameterInfo[] parameters = method.GetParameters();
                if (Util.IsExtensionMethod(method))
                {
                    for (int j = 1; j < parameters.Length; j++)
                    {
                        ParameterInfo info = parameters[j];
                        list3.Add(info.ParameterType);
                        if (info.DefaultValue != DBNull.Value)
                        {
                            list4.Add(info.DefaultValue);
                        }
                        @params = Util.IsParamArray(info);
                        if (@params)
                        {
                            paramType = info.ParameterType.GetElementType();
                        }
                    }
                    base3 = new ExtensionMethod(method as MethodInfo, list3.ToArray(), list4.ToArray(), paramType, @params, "");
                }
                else
                {
                    foreach (ParameterInfo info2 in parameters)
                    {
                        list3.Add(info2.ParameterType);
                        if (info2.DefaultValue != DBNull.Value)
                        {
                            list4.Add(info2.DefaultValue);
                        }
                        @params = Util.IsParamArray(info2);
                        if (@params)
                        {
                            paramType = info2.ParameterType.GetElementType();
                        }
                    }
                    if (method is MethodInfo)
                    {
                        base3 = new FunctionMethod(method as MethodInfo, list3.ToArray(), list4.ToArray(), paramType, @params, "");
                    }
                    else
                    {
                        base3 = new FunctionConstructor(method as ConstructorInfo, list3.ToArray(), list4.ToArray(), paramType, @params, "");
                    }
                }
                if (base3.IsGeneric)
                {
                    list2.Add(base3);
                }
                else
                {
                    list.Add(base3);
                }
            }
            this.m_Methods = list.ToArray();
            this.m_Count = this.m_Methods.Length;
            this.m_GenericMethods = list2.ToArray();
            this.m_GenericCount = this.m_GenericMethods.Length;
        }

        public UserdataMethod MakeGenericMethod(Type[] parameters)
        {
            if ((this.m_GenericCount > 0) && (this.m_GenericMethods != null))
            {
                List<MethodInfo> methods = new List<MethodInfo>();
                for (int i = 0; i < this.m_GenericCount; i++)
                {
                    FunctionMethod method = this.m_GenericMethods[i] as FunctionMethod;
                    if (method != null)
                    {
                        Type[] genericArguments = method.Method.GetGenericArguments();
                        if (genericArguments.Length == parameters.Length)
                        {
                            bool flag = true;
                            int length = genericArguments.Length;
                            for (int j = 0; j < length; j++)
                            {
                                if (!genericArguments[j].GetTypeInfo().BaseType.GetTypeInfo().IsAssignableFrom(parameters[j]))
                                {
                                    flag = false;
                                    break;
                                }
                            }
                            if (flag)
                            {
                                methods.Add(method.Method.MakeGenericMethod(parameters));
                                break;
                            }
                        }
                    }
                }
                if (methods.Count > 0)
                {
                    return new UserdataMethod(this.m_Script, this.m_Type, this.MethodName, methods);
                }
            }
            throw new ExecutionException(this.m_Script, "没有找到合适的泛型函数 " + this.MethodName);
        }

        public bool IsStatic
        {
            get
            {
                return this.m_IsStatic;
            }
        }

        public string MethodName
        {
            get
            {
                return this.m_MethodName;
            }
        }

        private class ExtensionMethod : UserdataMethod.FunctionBase
        {
            private object[] FinishArgs;
            public MethodInfo Method;

            public ExtensionMethod(MethodInfo Method, Type[] ParameterType, object[] DefaultParameter, Type ParamType, bool Params, string ParameterTypes) : base(ParameterType, DefaultParameter, ParamType, Params, ParameterTypes)
            {
                this.FinishArgs = new object[base.Args.Length + 1];
                this.Method = Method;
                base.IsGeneric = Method.IsGenericMethod && Method.ContainsGenericParameters;
            }

            public override object Invoke(object obj, Type type)
            {
                this.FinishArgs[0] = obj;
                Array.Copy(base.Args, 0, this.FinishArgs, 1, base.Args.Length);
                return this.Method.Invoke(null, this.FinishArgs);
            }
        }

        private abstract class FunctionBase
        {
            public object[] Args;
            public object[] DefaultParameter;
            public bool HasDefault;
            public bool IsGeneric;
            public int ParameterCount;
            public Type[] ParameterType;
            public string ParameterTypes;
            public bool Params;
            public Type ParamType;
            public int RequiredNumber;

            public FunctionBase(Type[] ParameterType, object[] DefaultParameter, Type ParamType, bool Params, string ParameterTypes)
            {
                this.ParameterType = ParameterType;
                this.DefaultParameter = DefaultParameter;
                this.ParamType = ParamType;
                this.Params = Params;
                this.ParameterTypes = ParameterTypes;
                this.Args = new object[ParameterType.Length];
                this.HasDefault = false;
                this.RequiredNumber = ParameterType.Length;
                this.ParameterCount = ParameterType.Length;
                if ((DefaultParameter != null) && (DefaultParameter.Length > 0))
                {
                    this.HasDefault = true;
                    this.RequiredNumber -= DefaultParameter.Length;
                }
                if (Params)
                {
                    this.RequiredNumber--;
                    this.ParameterCount--;
                }
            }

            public abstract object Invoke(object obj, Type type);
        }

        private class FunctionConstructor : UserdataMethod.FunctionBase
        {
            public ConstructorInfo Constructor;

            public FunctionConstructor(ConstructorInfo Constructor, Type[] ParameterType, object[] DefaultParameter, Type ParamType, bool Params, string ParameterTypes) : base(ParameterType, DefaultParameter, ParamType, Params, ParameterTypes)
            {
                base.IsGeneric = false;
                this.Constructor = Constructor;
            }

            public override object Invoke(object obj, Type type)
            {
                return this.Constructor.Invoke(base.Args);
            }
        }

        private class FunctionFastMethod : UserdataMethod.FunctionBase
        {
            public IScorpioFastReflectMethod Method;

            public FunctionFastMethod(IScorpioFastReflectMethod Method, Type[] ParameterType, Type ParamType, bool Params, string ParameterTypes) : base(ParameterType, null, ParamType, Params, ParameterTypes)
            {
                base.IsGeneric = false;
                this.Method = Method;
            }

            public override object Invoke(object obj, Type type)
            {
                return this.Method.Call(obj, base.ParameterTypes, base.Args);
            }
        }

        private class FunctionMethod : UserdataMethod.FunctionBase
        {
            public MethodInfo Method;

            public FunctionMethod(MethodInfo Method, Type[] ParameterType, object[] DefaultParameter, Type ParamType, bool Params, string ParameterTypes) : base(ParameterType, DefaultParameter, ParamType, Params, ParameterTypes)
            {
                this.Method = Method;
                base.IsGeneric = Method.IsGenericMethod && Method.ContainsGenericParameters;
            }

            public override object Invoke(object obj, Type type)
            {
                return this.Method.Invoke(obj, base.Args);
            }
        }
    }
}

