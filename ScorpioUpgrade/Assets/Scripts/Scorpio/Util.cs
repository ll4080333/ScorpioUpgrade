using System.Runtime.CompilerServices;

namespace Scorpio
{
    using Scorpio.Exception;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text;

    public static class Util
    {
        private static readonly Type TYPE_BOOL = typeof(bool);
        private static readonly Type TYPE_BYTE = typeof(byte);
        private static readonly Type TYPE_DECIMAL = typeof(decimal);
        private static readonly Type TYPE_DELEGATE = typeof(Delegate);
        private static readonly Type TYPE_DOUBLE = typeof(double);
        private static readonly Type TYPE_EXTENSIONATTRIBUTE = typeof(ExtensionAttribute);
        private static readonly Type TYPE_FLOAT = typeof(float);
        private static readonly Type TYPE_INT = typeof(int);
        private static readonly Type TYPE_LONG = typeof(long);
        private static readonly Type TYPE_OBJECT = typeof(object);
        private static readonly Type TYPE_PARAMATTRIBUTE = typeof(ParamArrayAttribute);
        private static readonly Type TYPE_SBYTE = typeof(sbyte);
        private static readonly Type TYPE_SHORT = typeof(short);
        private static readonly Type TYPE_STRING = typeof(string);
        private static readonly Type TYPE_TYPE = typeof(Type);
        private static readonly Type TYPE_UINT = typeof(uint);
        private static readonly Type TYPE_USHORT = typeof(ushort);
        private static readonly Type TYPE_VOID = typeof(void);

        public static void Assert(bool b, Script script, string message)
        {
            if (!b)
            {
                throw new ExecutionException(script, message);
            }
        }

        public static bool CanChangeType(ScriptObject par, Type type)
        {
            if (type == TYPE_OBJECT)
            {
                return true;
            }
            if ((((type == TYPE_SBYTE) || (type == TYPE_BYTE)) || ((type == TYPE_SHORT) || (type == TYPE_USHORT))) || ((((type == TYPE_INT) || (type == TYPE_UINT)) || ((type == TYPE_FLOAT) || (type == TYPE_DOUBLE))) || ((type == TYPE_DECIMAL) || (type == TYPE_LONG))))
            {
                return (par is ScriptNumber);
            }
            if (type == TYPE_BOOL)
            {
                return (par is ScriptBoolean);
            }
            if (type.GetTypeInfo().IsEnum)
            {
                return ((par is ScriptEnum) && (((ScriptEnum) par).EnumType == type));
            }
            if (par is ScriptNull)
            {
                return true;
            }
            if (type == TYPE_STRING)
            {
                return (par is ScriptString);
            }
            if (type == TYPE_TYPE)
            {
                return (par is ScriptUserdata);
            }
            if (TYPE_DELEGATE.GetTypeInfo().IsAssignableFrom(type))
            {
                return ((par is ScriptFunction) || ((par is ScriptUserdata) && ((par as ScriptUserdata).ValueType == type)));
            }
            if (par is ScriptUserdata)
            {
                return type.GetTypeInfo().IsAssignableFrom(((ScriptUserdata) par).ValueType);
            }
            return type.GetTypeInfo().IsAssignableFrom(par.GetType());
        }

        public static object ChangeType(Script script, ScriptObject par, Type type)
        {
            if (type != TYPE_OBJECT)
            {
                if ((par is ScriptUserdata) && (type == TYPE_TYPE))
                {
                    return ((ScriptUserdata) par).ValueType;
                }
                if (par is ScriptNumber)
                {
                    return ChangeType_impl(par.ObjectValue, type);
                }
                if (!TYPE_DELEGATE.GetTypeInfo().IsAssignableFrom(type))
                {
                    return par.ObjectValue;
                }
                if (par is ScriptFunction)
                {
                    return script.GetDelegate(type).Call(new ScriptObject[] { par });
                }
            }
            return par.ObjectValue;
        }

        public static object ChangeType_impl(object value, Type conversionType)
        {
            return Convert.ChangeType(value, conversionType);
        }

        public static bool IsDelegateType(Type type)
        {
            return TYPE_DELEGATE.GetTypeInfo().IsAssignableFrom(type);
        }

        public static bool IsExtensionMethod(MemberInfo method)
        {
            return method.IsDefined(TYPE_EXTENSIONATTRIBUTE, false);
        }

        public static bool IsExtensionType(Type type)
        {
            return type.IsDefined(TYPE_EXTENSIONATTRIBUTE, false);
        }

        public static bool IsNullOrEmpty(string value)
        {
            if (value != null)
            {
                return (value.Length == 0);
            }
            return true;
        }

        public static bool IsParamArray(ParameterInfo info)
        {
            return info.IsDefined(TYPE_PARAMATTRIBUTE, false);
        }

        public static bool IsVoid(Type type)
        {
            return (type == TYPE_VOID);
        }

        public static string Join(string separator, string[] stringarray)
        {
            int num = 0;
            int length = stringarray.Length;
            string str = "";
            for (int i = num; i < length; i++)
            {
                if (i > num)
                {
                    str = str + separator;
                }
                str = str + stringarray[i];
            }
            return str;
        }

        public static string ReadString(BinaryReader reader)
        {
            byte num;
            List<byte> list = new List<byte>();
            while ((num = reader.ReadByte()) != 0)
            {
                list.Add(num);
            }
            byte[] bytes = list.ToArray();
            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }

        public static byte ToByte(object value)
        {
            return Convert.ToByte(value);
        }

        public static double ToDouble(object value)
        {
            return Convert.ToDouble(value);
        }

        public static object ToEnum(Type type, int value)
        {
            return Enum.ToObject(type, value);
        }

        public static short ToInt16(object value)
        {
            return Convert.ToInt16(value);
        }

        public static int ToInt32(object value)
        {
            return Convert.ToInt32(value);
        }

        public static long ToInt64(object value)
        {
            return Convert.ToInt64(value);
        }

        public static sbyte ToSByte(object value)
        {
            return Convert.ToSByte(value);
        }

        public static float ToSingle(object value)
        {
            return Convert.ToSingle(value);
        }

        public static ushort ToUInt16(object value)
        {
            return Convert.ToUInt16(value);
        }

        public static uint ToUInt32(object value)
        {
            return Convert.ToUInt32(value);
        }

        public static ulong ToUInt64(object value)
        {
            return Convert.ToUInt64(value);
        }

        public static void WriteString(BinaryWriter writer, string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                writer.Write((byte) 0);
            }
            else
            {
                writer.Write(Encoding.UTF8.GetBytes(str));
                writer.Write((byte) 0);
            }
        }
    }
}

