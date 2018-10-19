namespace Scorpio
{
    using System;

    public class ScriptFunction : ScriptObject
    {
        public ScriptFunction(Script script, string name) : base(script)
        {
            base.Name = name;
        }

        public virtual int GetParamCount()
        {
            return 0;
        }

        public virtual ScriptArray GetParams()
        {
            return base.m_Script.CreateArray();
        }

        public virtual bool IsParams()
        {
            return false;
        }

        public virtual bool IsStatic()
        {
            return false;
        }

        public override string ToJson()
        {
            return "\"Function\"";
        }

        public override string ToString()
        {
            return ("Function(" + base.Name + ")");
        }

        public override ObjectType Type
        {
            get
            {
                return ObjectType.Function;
            }
        }
    }
}

