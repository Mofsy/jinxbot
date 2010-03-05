using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.ComponentModel;
using System.Drawing.Design;

namespace JinxBot.Controls.Design
{
    internal class PropertyDescriptorEx : PropertyDescriptor
    {
        private static Dictionary<Type, Type> s_enumTypeConverterMap = new Dictionary<Type, Type>();

        #region property data fields
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        private Attribute[] m_atts;
        private string m_category;
        private object m_defVal;
        private string m_desc;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        private string m_editorName;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        private string m_name, m_propertyName;
        private Type m_type;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        private string m_typeConverter;
        private Type m_converterType, m_editorType;
        private bool m_isRO;

        private MethodInfo m_getMethod, m_setMethod;
        #endregion


        private TypeDescriptorSurrogate m_sur;

        public PropertyDescriptorEx(string name, string propertyName, string category, object defaultValue, string description, string editorName,
            string converterName, Type propertyType, bool readOnly, Attribute[] attributes, MethodInfo getMethod, MethodInfo setMethod, TypeDescriptorSurrogate surrogate)
            : base(name, attributes)
        {
            m_sur = surrogate;

            m_name = name;
            m_propertyName = propertyName;
            m_category = category;
            m_defVal = defaultValue;
            m_desc = description;
            m_editorName = editorName;
            m_typeConverter = converterName;
            m_type = propertyType;
            m_isRO = readOnly;
            m_atts = attributes;

            m_getMethod = getMethod;
            m_setMethod = setMethod;

            if (propertyType.IsEnum)
            {
                lock (s_enumTypeConverterMap)
                {
                    if (!s_enumTypeConverterMap.ContainsKey(propertyType))
                    {
                        Type enumTypeConverter = typeof(EnumTypeConverter<int>).GetGenericTypeDefinition();
                        Type constructedConverter = enumTypeConverter.MakeGenericType(propertyType);
                        s_enumTypeConverterMap.Add(propertyType, constructedConverter);
                    }
                    m_converterType = s_enumTypeConverterMap[propertyType];
                }

                if (propertyType.GetCustomAttributes(typeof(FlagsAttribute), false).Length > 0)
                {
                    m_editorType = typeof(FlagsEnumTypeEditor<int>).GetGenericTypeDefinition().MakeGenericType(propertyType);
                }
                else
                {
                    if (!object.ReferenceEquals(null, editorName))
                        m_editorType = Type.GetType(editorName, false);
                }
            }
            else
            {
                if (!object.ReferenceEquals(converterName, null))
                    m_converterType = Type.GetType(converterName, false);
                if (!object.ReferenceEquals(null, editorName))
                    m_editorType = Type.GetType(editorName, false);
            }
        }

        public override bool CanResetValue(object component)
        {
            return (m_defVal != null);
        }

        public override Type ComponentType
        {
            get { return GetType(); }
        }

        [DebuggerHidden]
        public override object GetValue(object component)
        {
            MethodInfo miGet = m_getMethod;
            if (miGet == null)
                return null;
            try
            {
                object result = miGet.Invoke(m_sur.ActualObject, null);
                return result;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        public override bool IsReadOnly
        {
            get { return m_isRO; }
        }

        public override Type PropertyType
        {
            get { return m_type; }
        }

        public override void ResetValue(object component)
        {
            SetValue(component, m_defVal);
        }

        [DebuggerHidden]
        public override void SetValue(object component, object value)
        {
            if (m_isRO || m_setMethod == null)
                throw new InvalidOperationException("Cannot set this property.");
            try
            {
                // This is weird: when I double-click on the property grid drop-down for enum types,
                // it comes up with the value as a string instead of the actual enum value.
                if (value != null && value.GetType() != PropertyType)
                {
                    if (Converter.CanConvertFrom(value.GetType()))
                        value = Converter.ConvertFrom(value);
                }

                m_setMethod.Invoke(m_sur.ActualObject, new object[] { value });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        public override bool ShouldSerializeValue(object component)
        {
            object val = GetValue(component);
            if (val == null && m_defVal != null)
                return true;
            else if (m_defVal == null && val == null)
                return false;
            else
                return !val.Equals(m_defVal);
        }

        private TypeConverter m_converter;
        public override TypeConverter Converter
        {
            get
            {
                if (m_converter != null)
                    return m_converter;

                if (m_converterType != null)
                {
                    TypeConverter converter = Activator.CreateInstance(m_converterType) as TypeConverter;
                    m_converter = converter;
                    return converter;
                }
                return base.Converter;
            }
        }

        private UITypeEditor m_editor;
        public override object GetEditor(Type editorBaseType)
        {
            if (m_editor == null)
            {
                if (m_editorType != null)
                {
                    m_editor = Activator.CreateInstance(m_editorType) as UITypeEditor;
                    return m_editor;
                }
            }
            else
            {
                return m_editor;
            }
            return base.GetEditor(editorBaseType);
        }

        public override string Description
        {
            get
            {
                return m_desc;
            }
        }

        public override string Category
        {
            get
            {
                return m_category;
            }
        }
    }
}
