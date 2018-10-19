namespace Scorpio
{
    using Scorpio.Exception;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Text;

    public class ScriptArray : ScriptObject
    {
        private static readonly ScriptObject[] _emptyArray = new ScriptObject[0];
        private ScriptObject[] m_listObject;
        private ScriptObject m_null;
        private int m_size;

        public ScriptArray(Script script) : base(script)
        {
            this.m_listObject = _emptyArray;
            this.m_size = 0;
            this.m_null = script.Null;
        }

        public void Add(ScriptObject obj)
        {
            if (this.m_size == this.m_listObject.Length)
            {
                this.EnsureCapacity(this.m_size + 1);
            }
            this.m_listObject[this.m_size] = obj;
            this.m_size++;
        }

        public void Clear()
        {
            if (this.m_size > 0)
            {
                Array.Clear(this.m_listObject, 0, this.m_size);
                this.m_size = 0;
            }
        }

        public override ScriptObject Clone()
        {
            ScriptArray array = base.m_Script.CreateArray();
            array.m_listObject = new ScriptObject[this.m_size];
            array.m_size = this.m_size;
            for (int i = 0; i < this.m_size; i++)
            {
                if (this.m_listObject[i] == this)
                {
                    array.m_listObject[i] = array;
                }
                else if (this.m_listObject[i] == null)
                {
                    array.m_listObject[i] = this.m_null;
                }
                else
                {
                    array.m_listObject[i] = this.m_listObject[i].Clone();
                }
            }
            return array;
        }

        public bool Contains(ScriptObject obj)
        {
            for (int i = 0; i < this.m_size; i++)
            {
                if (obj.Equals(this.m_listObject[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public int Count()
        {
            return this.m_size;
        }

        private void EnsureCapacity(int min)
        {
            if (this.m_listObject.Length < min)
            {
                int num = (this.m_listObject.Length == 0) ? 4 : (this.m_listObject.Length * 2);
                if (num > 0x7fefffff)
                {
                    num = 0x7fefffff;
                }
                if (num < min)
                {
                    num = min;
                }
                this.SetCapacity(num);
            }
        }

        public ScriptObject First()
        {
            if (this.m_size > 0)
            {
                return this.m_listObject[0];
            }
            return this.m_null;
        }

        public Enumerator GetIterator()
        {
            return new Enumerator(this);
        }

        public override ScriptObject GetValue(object index)
        {
            if (((index is double) || (index is int)) || (index is long))
            {
                int num = Util.ToInt32(index);
                if (num < 0)
                {
                    throw new ExecutionException(base.m_Script, this, "Array GetValue索引小于0 index值为:" + index);
                }
                if (num >= this.m_size)
                {
                    return this.m_null;
                }
                return (this.m_listObject[num] ?? this.m_null);
            }
            if (!(index is string) || !index.Equals("length"))
            {
                throw new ExecutionException(base.m_Script, this, "Array GetValue只支持Number类型 index值为:" + index);
            }
            return base.m_Script.CreateDouble(Util.ToDouble(this.m_size));
        }

        public int IndexOf(ScriptObject obj)
        {
            for (int i = 0; i < this.m_size; i++)
            {
                if (obj.Equals(this.m_listObject[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        public void Insert(int index, ScriptObject obj)
        {
            if (this.m_size == this.m_listObject.Length)
            {
                this.EnsureCapacity(this.m_size + 1);
            }
            if (index < this.m_size)
            {
                Array.Copy(this.m_listObject, index, this.m_listObject, index + 1, this.m_size - index);
            }
            this.m_listObject[index] = obj;
            this.m_size++;
        }

        public ScriptObject Last()
        {
            if (this.m_size > 0)
            {
                return this.m_listObject[this.m_size - 1];
            }
            return this.m_null;
        }

        public int LastIndexOf(ScriptObject obj)
        {
            for (int i = this.m_size - 1; i >= 0; i--)
            {
                if (obj.Equals(this.m_listObject[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        public ScriptObject PopFirst()
        {
            if (this.m_size == 0)
            {
                throw new ExecutionException(base.m_Script, this, "Array Pop 数组长度为0");
            }
            ScriptObject obj2 = this.m_listObject[0];
            this.RemoveAt(0);
            return obj2;
        }

        public ScriptObject PopLast()
        {
            if (this.m_size == 0)
            {
                throw new ExecutionException(base.m_Script, this, "Array Pop 数组长度为0");
            }
            int index = this.m_size - 1;
            ScriptObject obj2 = this.m_listObject[index];
            this.RemoveAt(index);
            return obj2;
        }

        public bool Remove(ScriptObject obj)
        {
            int index = this.IndexOf(obj);
            if (index >= 0)
            {
                this.RemoveAt(index);
                return true;
            }
            return false;
        }

        public void RemoveAt(int index)
        {
            this.m_size--;
            if (index < this.m_size)
            {
                Array.Copy(this.m_listObject, index + 1, this.m_listObject, index, this.m_size - index);
            }
            this.m_listObject[this.m_size] = null;
        }

        public void Resize(int length)
        {
            if (length < 0)
            {
                throw new ExecutionException(base.m_Script, this, "Resize长度小于0 length:" + length);
            }
            if (length > this.m_size)
            {
                this.EnsureCapacity(length);
                this.m_size = length;
            }
            else
            {
                Array.Clear(this.m_listObject, length, this.m_size - length);
                this.m_size = length;
            }
        }

        public ScriptObject SafePopFirst()
        {
            if (this.m_size == 0)
            {
                return this.m_null;
            }
            ScriptObject obj2 = this.m_listObject[0];
            this.RemoveAt(0);
            return obj2;
        }

        public ScriptObject SafePopLast()
        {
            if (this.m_size == 0)
            {
                return this.m_null;
            }
            int index = this.m_size - 1;
            ScriptObject obj2 = this.m_listObject[index];
            this.RemoveAt(index);
            return obj2;
        }

        private void SetCapacity(int value)
        {
            if (value > 0)
            {
                ScriptObject[] destinationArray = new ScriptObject[value];
                if (this.m_size > 0)
                {
                    Array.Copy(this.m_listObject, 0, destinationArray, 0, this.m_size);
                }
                this.m_listObject = destinationArray;
            }
            else
            {
                this.m_listObject = _emptyArray;
            }
        }

        public override void SetValue(object index, ScriptObject obj)
        {
            if ((!(index is double) && !(index is int)) && !(index is long))
            {
                throw new ExecutionException(base.m_Script, this, "Array SetValue只支持Number类型 index值为:" + index);
            }
            int num = Util.ToInt32(index);
            if (num < 0)
            {
                throw new ExecutionException(base.m_Script, this, "Array SetValue索引小于0 index值为:" + index);
            }
            if (num >= this.m_size)
            {
                this.EnsureCapacity(num + 1);
                this.m_size = num + 1;
            }
            this.m_listObject[num] = obj;
        }

        public void Sort(ScriptFunction func)
        {
            Array.Sort<ScriptObject>(this.m_listObject, 0, this.m_size, new Comparer(base.m_Script, func));
        }

        public ScriptObject[] ToArray()
        {
            ScriptObject[] destinationArray = new ScriptObject[this.m_size];
            Array.Copy(this.m_listObject, 0, destinationArray, 0, this.m_size);
            return destinationArray;
        }

        public override string ToJson()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("[");
            for (int i = 0; i < this.m_size; i++)
            {
                if (i != 0)
                {
                    builder.Append(",");
                }
                if (this.m_listObject[i] == null)
                {
                    builder.Append(this.m_null.ToJson());
                }
                else
                {
                    builder.Append(this.m_listObject[i].ToJson());
                }
            }
            builder.Append("]");
            return builder.ToString();
        }

        public override string ToString()
        {
            return "Array";
        }

        public override ObjectType Type
        {
            get
            {
                return ObjectType.Array;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Comparer : IComparer<ScriptObject>
        {
            private Script script;
            private ScriptFunction func;
            internal Comparer(Script script, ScriptFunction func)
            {
                this.script = script;
                this.func = func;
            }

            public int Compare(ScriptObject x, ScriptObject y)
            {
                ScriptNumber number = this.func.Call(new ScriptObject[] { x, y }) as ScriptNumber;
                if (number == null)
                {
                    throw new ExecutionException(this.script, "Sort 返回值 必须是Number类型");
                }
                return number.ToInt32();
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Enumerator : IEnumerator
        {
            private ScriptArray list;
            private int index;
            private ScriptObject current;
            internal Enumerator(ScriptArray list)
            {
                this.list = list;
                this.index = 0;
                this.current = null;
            }

            public bool MoveNext()
            {
                if (this.index >= this.list.m_size)
                {
                    return false;
                }
                this.current = this.list.m_listObject[this.index] ?? this.list.m_null;
                this.index++;
                return true;
            }

            public ScriptObject Current
            {
                get
                {
                    return this.current;
                }
            }
            object IEnumerator.Current
            {
                get
                {
                    return this.current;
                }
            }
            public void Reset()
            {
                this.index = 0;
                this.current = null;
            }
        }
    }
}

