namespace Scorpio
{
    using Scorpio.Compiler;
    using Scorpio.Exception;
    using System;
    using System.Runtime.CompilerServices;

    public abstract class ScriptObject
    {
        protected Scorpio.Script m_Script;
        private static readonly ScriptObject[] NOPARAMETER = new ScriptObject[0];

        public ScriptObject(Scorpio.Script script)
        {
            this.m_Script = script;
        }

        public virtual ScriptObject Assign()
        {
            return this;
        }

        public virtual ScriptObject AssignCompute(Scorpio.Compiler.TokenType type, ScriptObject obj)
        {
            throw new ExecutionException(this.m_Script, this, string.Concat(new object[] { "类型[", this.Type, "]不支持赋值运算符[", type, "]" }));
        }

        public object call(params object[] args)
        {
            int length = args.Length;
            ScriptObject[] parameters = new ScriptObject[length];
            for (int i = 0; i < length; i++)
            {
                parameters[i] = this.m_Script.CreateObject(args[i]);
            }
            return this.Call(parameters);
        }

        public object Call()
        {
            return this.Call(NOPARAMETER);
        }

        public virtual object Call(ScriptObject[] parameters)
        {
            throw new ExecutionException(this.m_Script, this, "类型[" + this.Type + "]不支持函数调用");
        }

        public virtual ScriptObject Clone()
        {
            return this;
        }

        public virtual bool Compare(Scorpio.Compiler.TokenType type, ScriptObject obj)
        {
            throw new ExecutionException(this.m_Script, this, string.Concat(new object[] { "类型[", this.Type, "]不支持值比较[", type, "]" }));
        }

        public virtual ScriptObject Compute(Scorpio.Compiler.TokenType type, ScriptObject obj)
        {
            throw new ExecutionException(this.m_Script, this, string.Concat(new object[] { "类型[", this.Type, "]不支持运算符[", type, "]" }));
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (!(obj is ScriptObject))
            {
                return false;
            }
            if (this.ObjectValue == this)
            {
                return (obj == this);
            }
            return this.ObjectValue.Equals(((ScriptObject) obj).ObjectValue);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public virtual ScriptObject GetValue(object key)
        {
            throw new ExecutionException(this.m_Script, this, string.Concat(new object[] { "类型[", this.Type, "]不支持获取变量[", key, "]" }));
        }

        public virtual bool LogicOperation()
        {
            return true;
        }

        public virtual void SetValue(object key, ScriptObject value)
        {
            throw new ExecutionException(this.m_Script, this, string.Concat(new object[] { "类型[", this.Type, "]不支持设置变量[", key, "]" }));
        }

        public virtual string ToJson()
        {
            return this.ObjectValue.ToString();
        }

        public override string ToString()
        {
            return this.ObjectValue.ToString();
        }

        public virtual int BranchType
        {
            get
            {
                return 0;
            }
        }

        public bool IsArray
        {
            get
            {
                return (this.Type == ObjectType.Array);
            }
        }

        public bool IsBoolean
        {
            get
            {
                return (this.Type == ObjectType.Boolean);
            }
        }

        public bool IsEnum
        {
            get
            {
                return (this.Type == ObjectType.Enum);
            }
        }

        public bool IsFunction
        {
            get
            {
                return (this.Type == ObjectType.Function);
            }
        }

        public bool IsNull
        {
            get
            {
                return (this.Type == ObjectType.Null);
            }
        }

        public bool IsNumber
        {
            get
            {
                return (this.Type == ObjectType.Number);
            }
        }

        public bool IsPrimitive
        {
            get
            {
                if ((!this.IsNull && !this.IsBoolean) && !this.IsNumber)
                {
                    return this.IsString;
                }
                return true;
            }
        }

        public bool IsString
        {
            get
            {
                return (this.Type == ObjectType.String);
            }
        }

        public bool IsTable
        {
            get
            {
                return (this.Type == ObjectType.Table);
            }
        }

        public bool IsUserData
        {
            get
            {
                return (this.Type == ObjectType.UserData);
            }
        }

        public virtual object KeyValue
        {
            get
            {
                return this;
            }
        }

        public string Name { get; set; }

        public virtual object ObjectValue
        {
            get
            {
                return this;
            }
        }

        public Scorpio.Script Script
        {
            get
            {
                return this.m_Script;
            }
        }

        public abstract ObjectType Type { get; }
    }
}

