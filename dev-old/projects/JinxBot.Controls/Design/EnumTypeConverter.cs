using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Globalization;

namespace JinxBot.Controls.Design
{
    /// <summary>
    /// Implements the type converter for the enumeration specified in the type argument <typeparamref name="T">T</typeparamref>.
    /// </summary>
    /// <typeparam name="T">Specifies a type that should be an enumeration.</typeparam>
    internal class EnumTypeConverter<T> : System.ComponentModel.EnumConverter
    {
        private static object SyncObject = new object();

        private static Dictionary<string, T> s_nameToValueMap;
        private static Dictionary<T, string> s_valueToNameMap;
        private static Dictionary<string, string> s_fieldToNameMap;

        private static bool IsFlagsType, UsesSpecialNames;

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if <typeparamref name="T">T</typeparamref> is not an enumeration.</exception>
        // This code analysis item is suppressed because the ArgumentException refers to the type parameter of the class.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
        public EnumTypeConverter()
            : base(typeof(T))
        {
            Type targetType = typeof(T);
            if (!targetType.IsEnum)
                throw new ArgumentException("Target type must be an enumeration type.", "targetType");

            lock (SyncObject)
            {
                if (s_nameToValueMap == null)
                {
                    InitializeMaps();
                }
            }
        }

        private static void InitializeMaps()
        {
            s_nameToValueMap = new Dictionary<string, T>();
            s_valueToNameMap = new Dictionary<T, string>();
            s_fieldToNameMap = new Dictionary<string, string>();

            FieldInfo[] enumFields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo fi in enumFields)
            {
                string name = fi.Name;
                NameAttribute[] attr = fi.GetCustomAttributes(typeof(NameAttribute), false) as NameAttribute[];
                if (attr != null && attr.Length > 0 && attr[0].Name != null && attr[0].Name.Length > 0)
                {
                    name = attr[0].Name;
                    UsesSpecialNames = true;
                }
                s_fieldToNameMap.Add(fi.Name, name);

                T value = (T)fi.GetValue(null);
                s_nameToValueMap.Add(name, value);
                s_valueToNameMap.Add(value, name);
            }

            IsFlagsType = typeof(T).GetCustomAttributes(typeof(FlagsAttribute), false).Length > 0;
        }

        public override StandardValuesCollection GetStandardValues(System.ComponentModel.ITypeDescriptorContext context)
        {
            if (UsesSpecialNames)
            {
                List<string> values = new List<string>();
                foreach (T key in s_valueToNameMap.Keys)
                {
                    values.Add(s_valueToNameMap[key]);
                }

                return new StandardValuesCollection(values);
            }
            else
            {
                return base.GetStandardValues(context);
            }
        }

        public override bool GetStandardValuesExclusive(System.ComponentModel.ITypeDescriptorContext context)
        {
            return !IsFlagsType;
        }

        public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(string))
                return base.ConvertTo(context, culture, value, destinationType);

            if (!IsFlagsType)
            {
                T val;
                if (value is T)
                    val = (T)value;
                else if (value is string)
                    val = (T)ConvertFrom(context, culture, value);
                else
                    throw new InvalidCastException();

                if (s_valueToNameMap.ContainsKey(val))
                {
                    return s_valueToNameMap[val];
                }
                return null;
            }
            else
            {
                T val;
                if (value is T)
                    val = (T)value;
                else if (value is string)
                    val = (T)ConvertFrom(context, culture, value);
                else if (IsFlagsType && value is ulong)
                {
                    object realSize = SizewiseConversion((ulong)value);
                    val = (T)realSize;
                }
                else
                    throw new InvalidCastException();

                ulong valVal = Convert.ToUInt64(val, CultureInfo.InvariantCulture);

                if (valVal == 0)
                {
                    if (s_valueToNameMap.ContainsKey(default(T)))
                        return s_valueToNameMap[default(T)];
                    else
                        return string.Empty;
                }
                else
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (T key in s_valueToNameMap.Keys)
                    {
                        ulong keyVal = Convert.ToUInt64(key, CultureInfo.InvariantCulture);
                        if (keyVal != 0 && (valVal & keyVal) == keyVal)
                        {
                            if (sb.Length > 0)
                                sb.AppendFormat(",{0}", s_valueToNameMap[key]);
                            else
                                sb.Append(s_valueToNameMap[key]);
                        }
                    }
                    return sb.ToString();
                }
            }
        }

        private static object SizewiseConversion(ulong ul)
        {
            Type t = Enum.GetUnderlyingType(typeof(T));
            if (t == typeof(int))
            {
                return (int)(unchecked((long)ul) & (int)-1);
            }
            else if (t == typeof(uint))
            {
                return (uint)(ul & 0xffffffff);
            }
            else if (t == typeof(short) || t == typeof(ushort))
                return (ushort)(ul & 0xffff);
            else if (t == typeof(byte) || t == typeof(sbyte))
                return (byte)(ul & 0xff);
            else
                return ul;
        }

        public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            string val = value as string;
            if (val == null)
            {
                return base.ConvertFrom(context, culture, value);
            }

            if (!IsFlagsType)
            {
                string src = val;
                if (!s_nameToValueMap.ContainsKey(src))
                    return null;

                return s_nameToValueMap[src];
            }
            else
            {
                if (val.Length == 0)
                    return default(T);

                if (val.IndexOf(',') == -1)
                {
                    if (!s_nameToValueMap.ContainsKey(val))
                        return null;
                    return s_nameToValueMap[val];
                }
                else
                {
                    string[] items = val.Split(',');
                    ulong result = Convert.ToUInt64(default(T), CultureInfo.InvariantCulture);
                    foreach (string v in items)
                    {
                        if (!s_nameToValueMap.ContainsKey(v))
                            throw new FormatException("No matching flag value.");
                        result |= Convert.ToUInt64(s_nameToValueMap[v], CultureInfo.InvariantCulture);
                    }

                    return (T)SizewiseConversion(result);
                }
            }
        }

        internal static string GetNameForEmptyValue()
        {
            lock (SyncObject)
            {
                if (s_nameToValueMap == null)
                {
                    InitializeMaps();
                }
            }

            foreach (string key in s_nameToValueMap.Keys)
            {
                if (Convert.ToUInt64(s_nameToValueMap[key], CultureInfo.InvariantCulture) == 0)
                    return key;
            }

            return null;
        }
    }
}
