namespace Scorpio
{
    using Scorpio.Compiler;
    using Scorpio.Exception;
    using Scorpio.Function;
    using Scorpio.Library;
    using Scorpio.Runtime;
    using Scorpio.Serialize;
    using Scorpio.Userdata;
    using Scorpio.Variable;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Sockets;
    using System.Reflection;
    using System.Text;

    public class Script
    {
        public const BindingFlags BindingFlag = (BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
        public const string DynamicDelegateName = "__DynamicDelegate__";
        private const string GLOBAL_SCRIPT = "_SCRIPT";
        private const string GLOBAL_TABLE = "_G";
        private const string GLOBAL_VERSION = "_VERSION";
        private List<Assembly> m_Assembly = new List<Assembly>();
        private List<string> m_Defines = new List<string>();
        private Dictionary<Type, ScriptUserdataDelegateType> m_Delegates = new Dictionary<Type, ScriptUserdataDelegateType>();
        private Dictionary<Type, ScriptUserdataEnum> m_Enums = new Dictionary<Type, ScriptUserdataEnum>();
        private ScriptBoolean m_False;
        private Dictionary<Type, IScorpioFastReflectClass> m_FastReflectClass = new Dictionary<Type, IScorpioFastReflectClass>();
        private ScriptTable m_GlobalTable;
        private ScriptNull m_Null;
        private List<string> m_SearchPath = new List<string>();
        private StackInfo m_StackInfo = new StackInfo();
        private Stack<StackInfo> m_StackInfoStack = new Stack<StackInfo>();
        private ScriptBoolean m_True;
        private Dictionary<Type, ScriptUserdataObjectType> m_Types = new Dictionary<Type, ScriptUserdataObjectType>();
        private Dictionary<Type, UserdataType> m_UserdataTypes = new Dictionary<Type, UserdataType>();
        private static readonly Encoding UTF8 = Encoding.UTF8;
        public const string Version = "master";

        public Script()
        {
            this.m_Null = new ScriptNull(this);
            this.m_True = new ScriptBoolean(this, true);
            this.m_False = new ScriptBoolean(this, false);
            this.m_GlobalTable = this.CreateTable();
            this.m_GlobalTable.SetValue("_G", this.m_GlobalTable);
            this.m_GlobalTable.SetValue("_VERSION", this.CreateString("master"));
            this.m_GlobalTable.SetValue("_SCRIPT", this.CreateObject(this));
            this.PushAssembly(typeof(object).GetTypeInfo().Assembly);
            this.PushAssembly(typeof(Socket).GetTypeInfo().Assembly);
            this.PushAssembly(base.GetType().GetTypeInfo().Assembly);
        }

        public object Call(string strName, ScriptObject[] args)
        {
            ScriptObject obj2 = this.m_GlobalTable.GetValue(strName);
            if (obj2 is ScriptNull)
            {
                throw new ScriptException("找不到变量[" + strName + "]");
            }
            this.m_StackInfoStack.Clear();
            return obj2.Call(args);
        }

        public object Call(string strName, params object[] args)
        {
            ScriptObject obj2 = this.m_GlobalTable.GetValue(strName);
            if (obj2 is ScriptNull)
            {
                throw new ScriptException("找不到变量[" + strName + "]");
            }
            int length = args.Length;
            ScriptObject[] parameters = new ScriptObject[length];
            for (int i = 0; i < length; i++)
            {
                parameters[i] = this.CreateObject(args[i]);
            }
            this.m_StackInfoStack.Clear();
            return obj2.Call(parameters);
        }

        public void ClearStackInfo()
        {
            this.m_StackInfoStack.Clear();
        }

        public bool ContainDefine(string define)
        {
            return this.m_Defines.Contains(define);
        }

        public bool ContainsFastReflectClass(Type type)
        {
            return this.m_FastReflectClass.ContainsKey(type);
        }

        public ScriptArray CreateArray()
        {
            return new ScriptArray(this);
        }

        public ScriptBoolean CreateBool(bool value)
        {
            if (!value)
            {
                return this.False;
            }
            return this.True;
        }

        public ScriptNumber CreateDouble(double value)
        {
            return new ScriptNumberDouble(this, value);
        }

        public ScriptFunction CreateFunction(ScorpioHandle value)
        {
            return new ScriptHandleFunction(this, value);
        }

        public ScriptObject CreateObject(object value)
        {
            if (value == null)
            {
                return this.m_Null;
            }
            if (value is bool)
            {
                return this.CreateBool((bool) value);
            }
            if (value is string)
            {
                return new ScriptString(this, (string) value);
            }
            if (value is long)
            {
                return new ScriptNumberLong(this, (long) value);
            }
            if ((((value is sbyte) || (value is byte)) || ((value is short) || (value is ushort))) || (((value is int) || (value is uint)) || (((value is float) || (value is double)) || (value is decimal))))
            {
                return new ScriptNumberDouble(this, Util.ToDouble(value));
            }
            if (value is ScriptObject)
            {
                return (ScriptObject) value;
            }
            if (value is ScorpioFunction)
            {
                return new ScriptDelegateFunction(this, (ScorpioFunction) value);
            }
            if (value is ScorpioHandle)
            {
                return new ScriptHandleFunction(this, (ScorpioHandle) value);
            }
            if (value is ScorpioMethod)
            {
                return new ScriptMethodFunction(this, (ScorpioMethod) value);
            }
            if (value.GetType().GetTypeInfo().IsEnum)
            {
                return new ScriptEnum(this, value);
            }
            return this.CreateUserdata(value);
        }

        public ScriptString CreateString(string value)
        {
            return new ScriptString(this, value);
        }

        public ScriptTable CreateTable()
        {
            return new ScriptTable(this);
        }

        public ScriptUserdata CreateUserdata(object obj)
        {
            Type type = obj as Type;
            if (type != null)
            {
                if (type.GetTypeInfo().IsEnum)
                {
                    return this.GetEnum(type);
                }
                if (Util.IsDelegateType(type))
                {
                    return this.GetDelegate(type);
                }
                return this.GetType(type);
            }
            if (obj is Delegate)
            {
                return new ScriptUserdataDelegate(this, (Delegate) obj);
            }
            if (obj is BridgeEventInfo)
            {
                return new ScriptUserdataEventInfo(this, (BridgeEventInfo) obj);
            }
            return new ScriptUserdataObject(this, obj, this.GetScorpioType(obj.GetType()));
        }

        public StackInfo GetCurrentStackInfo()
        {
            return this.m_StackInfo;
        }

        public ScriptUserdata GetDelegate(Type type)
        {
            if (this.m_Delegates.ContainsKey(type))
            {
                return this.m_Delegates[type];
            }
            ScriptUserdataDelegateType type2 = new ScriptUserdataDelegateType(this, type);
            this.m_Delegates.Add(type, type2);
            return type2;
        }

        public ScriptUserdata GetEnum(Type type)
        {
            if (this.m_Enums.ContainsKey(type))
            {
                return this.m_Enums[type];
            }
            ScriptUserdataEnum enum2 = new ScriptUserdataEnum(this, type);
            this.m_Enums.Add(type, enum2);
            return enum2;
        }

        public IScorpioFastReflectClass GetFastReflectClass(Type type)
        {
            return this.m_FastReflectClass[type];
        }

        public ScriptTable GetGlobalTable()
        {
            return this.m_GlobalTable;
        }

        public UserdataType GetScorpioType(Type type)
        {
            if (this.m_UserdataTypes.ContainsKey(type))
            {
                return this.m_UserdataTypes[type];
            }
            UserdataType type2 = null;
            if (this.ContainsFastReflectClass(type))
            {
                type2 = new FastReflectUserdataType(this, type, this.GetFastReflectClass(type));
            }
            else
            {
                type2 = new ReflectUserdataType(this, type);
            }
            this.m_UserdataTypes.Add(type, type2);
            return type2;
        }

        public string GetStackInfo()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(string.Concat(new object[] { "Source [", this.m_StackInfo.Breviary, "] Line [", this.m_StackInfo.Line, "]" }));
            foreach (StackInfo info in this.m_StackInfoStack)
            {
                builder.AppendLine(string.Concat(new object[] { "        Source [", info.Breviary, "] Line [", info.Line, "]" }));
            }
            return builder.ToString();
        }

        public Type GetType(string str)
        {
            for (int i = 0; i < this.m_Assembly.Count; i++)
            {
                Type type = this.m_Assembly[i].GetType(str);
                if (type != null)
                {
                    return type;
                }
            }
            return Type.GetType(str, false, false);
        }

        public ScriptUserdataObjectType GetType(Type type)
        {
            if (this.m_Types.ContainsKey(type))
            {
                return this.m_Types[type];
            }
            ScriptUserdataObjectType type2 = new ScriptUserdataObjectType(this, type, this.GetScorpioType(type));
            this.m_Types.Add(type, type2);
            return type2;
        }

        public ScriptObject GetValue(string key)
        {
            return this.m_GlobalTable.GetValue(key);
        }

        public bool HasValue(string key)
        {
            return this.m_GlobalTable.HasValue(key);
        }

        private ScriptObject Load(string strBreviary, List<Token> tokens, ScriptContext context)
        {
            if (tokens.Count == 0)
            {
                return this.m_Null;
            }
            ScriptExecutable scriptExecutable = new ScriptParser(this, tokens, strBreviary).Parse();
            return new ScriptContext(this, scriptExecutable, context, Executable_Block.Context).Execute();
        }

        public ScriptObject LoadBuffer(byte[] buffer)
        {
            return this.LoadBuffer("Undefined", buffer, UTF8);
        }

        public ScriptObject LoadBuffer(string strBreviary, byte[] buffer)
        {
            return this.LoadBuffer(strBreviary, buffer, UTF8);
        }

        public ScriptObject LoadBuffer(string strBreviary, byte[] buffer, Encoding encoding)
        {
            ScriptObject obj2;
            if ((buffer == null) || (buffer.Length == 0))
            {
                return null;
            }
            try
            {
                if (buffer[0] == 0)
                {
                    return this.LoadTokens(strBreviary, ScorpioMaker.Deserialize(buffer));
                }
                obj2 = this.LoadString(strBreviary, encoding.GetString(buffer, 0, buffer.Length));
            }
            catch (System.Exception exception)
            {
                throw new ScriptException("load buffer [" + strBreviary + "] is error : " + exception.ToString());
            }
            return obj2;
        }

        public void LoadExtension(string type)
        {
            this.LoadExtension(this.GetType(type));
        }

        public void LoadExtension(Type type)
        {
            if ((type != null) && Util.IsExtensionType(type))
            {
                foreach (MethodInfo info in type.GetMethods(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance))
                {
                    if (Util.IsExtensionMethod(info))
                    {
                        this.GetScorpioType(info.GetParameters()[0].ParameterType).AddExtensionMethod(info);
                    }
                }
            }
        }

        public ScriptObject LoadFile(string strFileName)
        {
            return this.LoadFile(strFileName, UTF8);
        }

        public ScriptObject LoadFile(string fileName, Encoding encoding)
        {
            using (FileStream stream = File.OpenRead(fileName))
            {
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                return this.LoadBuffer(fileName, buffer, encoding);
            }
        }

        public void LoadLibrary()
        {
            LibraryBasis.Load(this);
            LibraryArray.Load(this);
            LibraryString.Load(this);
            LibraryTable.Load(this);
            LibraryJson.Load(this);
            LibraryMath.Load(this);
            LibraryFunc.Load(this);
            LibraryUserdata.Load(this);
        }

        public ScriptObject LoadSearchPathFile(string fileName)
        {
            for (int i = 0; i < this.m_SearchPath.Count; i++)
            {
                string path = this.m_SearchPath[i] + "/" + fileName;
                if (File.Exists(path))
                {
                    return this.LoadFile(path);
                }
            }
            throw new ExecutionException(this, "require 找不到文件 : " + fileName);
        }

        public ScriptObject LoadString(string strBuffer)
        {
            return this.LoadString("", strBuffer);
        }

        public ScriptObject LoadString(string strBreviary, string strBuffer)
        {
            return this.LoadString(strBreviary, strBuffer, null, true);
        }

        internal ScriptObject LoadString(string strBreviary, string strBuffer, ScriptContext context, bool clearStack)
        {
            ScriptObject obj2;
            try
            {
                if (Util.IsNullOrEmpty(strBuffer))
                {
                    return this.m_Null;
                }
                if (clearStack)
                {
                    this.m_StackInfoStack.Clear();
                }
                ScriptLexer lexer = new ScriptLexer(strBuffer, strBreviary);
                obj2 = this.Load(lexer.GetBreviary(), lexer.GetTokens(), context);
            }
            catch (System.Exception exception)
            {
                throw new ScriptException("load buffer [" + strBreviary + "] is error : " + exception.ToString());
            }
            return obj2;
        }

        public ScriptObject LoadTokens(List<Token> tokens)
        {
            return this.LoadTokens("Undefined", tokens);
        }

        public ScriptObject LoadTokens(string strBreviary, List<Token> tokens)
        {
            ScriptObject obj2;
            try
            {
                if (tokens.Count == 0)
                {
                    return this.m_Null;
                }
                this.m_StackInfoStack.Clear();
                obj2 = this.Load(strBreviary, tokens, null);
            }
            catch (System.Exception exception)
            {
                throw new ScriptException("load tokens [" + strBreviary + "] is error : " + exception.ToString());
            }
            return obj2;
        }

        public ScriptObject LoadType(string str)
        {
            Type type = this.GetType(str);
            if (type == null)
            {
                return this.m_Null;
            }
            return this.CreateUserdata(type);
        }

        internal void PopStackInfo()
        {
            if (this.m_StackInfoStack.Count > 0)
            {
                this.m_StackInfoStack.Pop();
            }
        }

        public void PushAssembly(Assembly assembly)
        {
            if ((assembly != null) && !this.m_Assembly.Contains(assembly))
            {
                this.m_Assembly.Add(assembly);
            }
        }

        public void PushDefine(string define)
        {
            if (!this.m_Defines.Contains(define))
            {
                this.m_Defines.Add(define);
            }
        }

        public void PushFastReflectClass(Type type, IScorpioFastReflectClass value)
        {
            this.m_FastReflectClass[type] = value;
        }

        public void PushSearchPath(string path)
        {
            if (!this.m_SearchPath.Contains(path))
            {
                this.m_SearchPath.Add(path);
            }
        }

        internal void PushStackInfo()
        {
            this.m_StackInfoStack.Push(this.m_StackInfo);
        }

        public void SetObject(string key, object value)
        {
            this.m_GlobalTable.SetValue(key, this.CreateObject(value));
        }

        internal void SetObjectInternal(string key, ScriptObject value)
        {
            this.m_GlobalTable.SetValue(key, value);
        }

        internal void SetStackInfo(StackInfo info)
        {
            this.m_StackInfo = info;
        }

        public void SetValue(string key, object value)
        {
            this.m_GlobalTable.SetValue(key, this.CreateObject(value));
        }

        public ScriptBoolean False
        {
            get
            {
                return this.m_False;
            }
        }

        public ScriptNull Null
        {
            get
            {
                return this.m_Null;
            }
        }

        public ScriptBoolean True
        {
            get
            {
                return this.m_True;
            }
        }
    }
}

