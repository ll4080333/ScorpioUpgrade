namespace Scorpio
{
    using Scorpio.Compiler;
    using Scorpio.Exception;
    using Scorpio.Function;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class ScriptTable : ScriptObject
    {
        private Dictionary<object, ScriptObject> m_listObject;

        public ScriptTable(Script script) : base(script)
        {
            this.m_listObject = new Dictionary<object, ScriptObject>();
        }

        public override ScriptObject AssignCompute(Scorpio.Compiler.TokenType type, ScriptObject value)
        {
            if (type != Scorpio.Compiler.TokenType.AssignPlus)
            {
                return base.AssignCompute(type, value);
            }
            ScriptTable table = value as ScriptTable;
            if (table == null)
            {
                throw new ExecutionException(base.m_Script, this, "table [+=] 操作只限两个[table]之间,传入数据类型:" + value.Type);
            }
            ScriptObject obj2 = null;
            ScriptScriptFunction function = null;
            foreach (KeyValuePair<object, ScriptObject> pair in table.m_listObject)
            {
                obj2 = pair.Value.Clone();
                if (obj2 is ScriptScriptFunction)
                {
                    function = (ScriptScriptFunction) obj2;
                    if (!function.IsStaticFunction)
                    {
                        function.SetTable(this);
                    }
                }
                this.m_listObject[pair.Key] = obj2;
            }
            return this;
        }

        public void Clear()
        {
            this.m_listObject.Clear();
        }

        public override ScriptObject Clone()
        {
            ScriptTable table = base.m_Script.CreateTable();
            ScriptObject obj2 = null;
            ScriptScriptFunction function = null;
            foreach (KeyValuePair<object, ScriptObject> pair in this.m_listObject)
            {
                if (pair.Value == this)
                {
                    table.m_listObject[pair.Key] = table;
                }
                else
                {
                    obj2 = pair.Value.Clone();
                    if (obj2 is ScriptScriptFunction)
                    {
                        function = (ScriptScriptFunction) obj2;
                        if (!function.IsStaticFunction)
                        {
                            function.SetTable(table);
                        }
                    }
                    table.m_listObject[pair.Key] = obj2;
                }
            }
            return table;
        }

        public override ScriptObject Compute(Scorpio.Compiler.TokenType type, ScriptObject value)
        {
            if (type != Scorpio.Compiler.TokenType.Plus)
            {
                return base.Compute(type, value);
            }
            ScriptTable table = value as ScriptTable;
            if (table == null)
            {
                throw new ExecutionException(base.m_Script, this, "table [+] 操作只限两个[table]之间,传入数据类型:" + value.Type);
            }
            ScriptTable table2 = base.m_Script.CreateTable();
            ScriptObject obj2 = null;
            ScriptScriptFunction function = null;
            foreach (KeyValuePair<object, ScriptObject> pair in this.m_listObject)
            {
                obj2 = pair.Value.Clone();
                if (obj2 is ScriptScriptFunction)
                {
                    function = (ScriptScriptFunction) obj2;
                    if (!function.IsStaticFunction)
                    {
                        function.SetTable(table2);
                    }
                }
                table2.m_listObject[pair.Key] = obj2;
            }
            foreach (KeyValuePair<object, ScriptObject> pair2 in table.m_listObject)
            {
                obj2 = pair2.Value.Clone();
                if (obj2 is ScriptScriptFunction)
                {
                    function = (ScriptScriptFunction) obj2;
                    if (!function.IsStaticFunction)
                    {
                        function.SetTable(table2);
                    }
                }
                table2.m_listObject[pair2.Key] = obj2;
            }
            return table2;
        }

        public int Count()
        {
            return this.m_listObject.Count;
        }

        public Dictionary<object, ScriptObject>.Enumerator GetIterator()
        {
            return this.m_listObject.GetEnumerator();
        }

        public ScriptArray GetKeys()
        {
            ScriptArray array = base.m_Script.CreateArray();
            foreach (KeyValuePair<object, ScriptObject> pair in this.m_listObject)
            {
                array.Add(base.m_Script.CreateObject(pair.Key));
            }
            return array;
        }

        public override ScriptObject GetValue(object key)
        {
            if (!this.m_listObject.ContainsKey(key))
            {
                return base.m_Script.Null;
            }
            return this.m_listObject[key];
        }

        public ScriptArray GetValues()
        {
            ScriptArray array = base.m_Script.CreateArray();
            foreach (KeyValuePair<object, ScriptObject> pair in this.m_listObject)
            {
                array.Add(pair.Value.Assign());
            }
            return array;
        }

        public bool HasValue(object key)
        {
            if (key == null)
            {
                return false;
            }
            return this.m_listObject.ContainsKey(key);
        }

        public void Remove(object key)
        {
            this.m_listObject.Remove(key);
        }

        public override void SetValue(object key, ScriptObject value)
        {
            if (value is ScriptScriptFunction)
            {
                ((ScriptScriptFunction) value).SetTable(this);
            }
            this.m_listObject[key] = value.Assign();
        }

        public override string ToJson()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("{");
            bool flag = true;
            foreach (KeyValuePair<object, ScriptObject> pair in this.m_listObject)
            {
                if (flag)
                {
                    flag = false;
                }
                else
                {
                    builder.Append(",");
                }
                builder.Append("\"");
                builder.Append(pair.Key);
                builder.Append("\":");
                builder.Append(pair.Value.ToJson());
            }
            builder.Append("}");
            return builder.ToString();
        }

        public override string ToString()
        {
            return "Table";
        }

        public override ObjectType Type
        {
            get
            {
                return ObjectType.Table;
            }
        }
    }
}

