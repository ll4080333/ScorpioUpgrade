namespace Scorpio
{
    using System;

    public class ScriptNull : ScriptObject
    {
        public ScriptNull(Script script) : base(script)
        {
        }

        public override bool Equals(object obj)
        {
            return ((obj == null) || (obj is ScriptNull));
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override bool LogicOperation()
        {
            return false;
        }

        public override string ToJson()
        {
            return "null";
        }

        public override string ToString()
        {
            return "null";
        }

        public override object ObjectValue
        {
            get
            {
                return null;
            }
        }

        public override ObjectType Type
        {
            get
            {
                return ObjectType.Null;
            }
        }
    }
}

