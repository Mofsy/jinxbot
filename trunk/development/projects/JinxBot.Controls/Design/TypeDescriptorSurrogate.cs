using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using System.Drawing;

namespace JinxBot.Controls.Design
{
    /// <summary>
    /// This type implements the <see cref="ICustomTypeDescriptor">ICustomTypeDescriptor</see> interface, and allows any object annotated with the special
    /// design attributes to be designed within a standard <see cref="System.Windows.Forms.PropertyGrid">PropertyGrid</see> control and appear as if it is
    /// contained within a <see cref="PropertyGridEx">PropertyGridEx</see> control, except without multiple property tabs.
    /// </summary>
    /// <remarks>
    /// <para>To use this object, create a new instance, passing into the constructor the object you wish to design.  Then, pass the surrogate object
    /// into the <see cref="System.Windows.Forms.PropertyGrid.SelectedObject">SelectedObject</see> property of the <b>PropertyGrid</b> control.</para>
    /// </remarks>
    public sealed class TypeDescriptorSurrogate : ICustomTypeDescriptor
    {
        private object m_objToRepresent;
        private Dictionary<string, PropertyDescriptorEx> m_descriptors;
        private string m_defProp;

        /// <summary>
        /// Creates a new TypeDescriptorSurrogate to represent a specified object within a PropertyGrid control.
        /// </summary>
        /// <param name="value">The object to represent.</param>
        public TypeDescriptorSurrogate(object value)
        {
            if (value == null)
                throw new ArgumentNullException("value", "Cannot represent a non-existent object.");
            m_objToRepresent = value;

            m_descriptors = new Dictionary<string, PropertyDescriptorEx>();

            Type typeToModel = value.GetType();

            #region determine if the type has a default property
            string defaultPropertyName = null;
            DefaultPropertyAttribute[] dpatt = typeToModel.GetCustomAttributes(typeof(DefaultPropertyAttribute), true) as DefaultPropertyAttribute[];
            if (dpatt != null && dpatt.Length > 0)
            {
                defaultPropertyName = dpatt[0].Name;
                m_defProp = defaultPropertyName;
            }
            #endregion

            PropertyInfo[] properties = typeToModel.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            ProcessProperties(properties);
        }

        private void ProcessProperties(PropertyInfo[] properties)
        {
            foreach (PropertyInfo property in properties)
            {
                List<Attribute> additionalAttributes = new List<Attribute>();

                #region determine if the property should be displayed
                BrowsableAttribute[] batt = property.GetCustomAttributes(typeof(BrowsableAttribute), true) as BrowsableAttribute[];
                if (batt == null || batt.Length == 0 || batt[0].Browsable == false)
                    continue; // skip the rest of the property processing
                additionalAttributes.Add(batt[0]);
                #endregion
                #region determine if the property has a friendly name
                string propertyName = property.Name;
                NameAttribute[] natt = property.GetCustomAttributes(typeof(NameAttribute), true) as NameAttribute[];
                if (natt != null && natt.Length > 0 && natt[0].Name != null && natt[0].Name.Length > 0)
                    propertyName = natt[0].Name;
                #endregion

                Type propertyType = property.PropertyType;
                string category = null, description = null, typeEditorName = null, typeConverterName = null;
                object defaultValue = null;
                bool isReadOnly = false;
                #region determine if the property is read-only
                MethodInfo miGetProp = property.GetGetMethod(true);
                MethodInfo miSetProp = property.GetSetMethod(true);
                if (miSetProp == null)
                {
                    ReadOnlyAttribute ro = new ReadOnlyAttribute(true);
                    isReadOnly = true;
                    additionalAttributes.Add(ro);
                }
                #endregion
                #region determine if it has a Category
                CategoryAttribute[] catt = property.GetCustomAttributes(typeof(CategoryAttribute), true) as CategoryAttribute[];
                if (catt != null && catt.Length > 0)
                    category = catt[0].Category;
                #endregion
                #region determine if it has a Description
                DescriptionAttribute[] datt = property.GetCustomAttributes(typeof(DescriptionAttribute), true) as DescriptionAttribute[];
                if (datt != null && datt.Length > 0)
                    description = datt[0].Description;
                #endregion
                #region determine if it has a default value
                DefaultValueAttribute[] dvatt = property.GetCustomAttributes(typeof(DefaultValueAttribute), true) as DefaultValueAttribute[];
                if (dvatt != null && dvatt.Length > 0)
                    defaultValue = dvatt[0].Value;
                #endregion
                #region determine if it has a type editor
                EditorAttribute[] eatt = property.GetCustomAttributes(typeof(EditorAttribute), true) as EditorAttribute[];
                if (eatt != null && eatt.Length > 0)
                    typeEditorName = eatt[0].EditorTypeName;
                #endregion
                #region determine if it has a type converter
                TypeConverterAttribute[] tcatt = property.GetCustomAttributes(typeof(TypeConverterAttribute), true) as TypeConverterAttribute[];
                if (tcatt != null && tcatt.Length > 0)
                    typeConverterName = tcatt[0].ConverterTypeName;
                #endregion

                List<Attribute> otherAttributes = CheckForAdditionalAttributes(property.GetCustomAttributes(true));

                additionalAttributes.AddRange(otherAttributes);

                PropertyDescriptorEx pdex = new PropertyDescriptorEx(propertyName, property.Name, category, defaultValue, description,
                    typeEditorName, typeConverterName, propertyType, isReadOnly, additionalAttributes.ToArray(), miGetProp, miSetProp, this);

                this.m_descriptors.Add(propertyName, pdex);
 
            }
        }

        private static List<Attribute> CheckForAdditionalAttributes(object[] p)
        {
            List<Attribute> atts = new List<Attribute>();
            foreach (Attribute att in p)
            {
                Type attType = att.GetType();
                if (attType == typeof(ReadOnlyAttribute) || attType == typeof(NameAttribute) || attType == typeof(BrowsableAttribute) ||
                    attType == typeof(CategoryAttribute) || attType == typeof(DescriptionAttribute) || attType == typeof(DefaultValueAttribute) ||
                    attType == typeof(EditorAttribute) || attType == typeof(TypeConverterAttribute))
                    continue;

                atts.Add(att);
            }
            return atts;
        }

        #region ICustomTypeDescriptor Members

        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        string ICustomTypeDescriptor.GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        string ICustomTypeDescriptor.GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            if (m_defProp != null && m_descriptors.ContainsKey(m_defProp))
                return m_descriptors[m_defProp];
            return null;
        }

        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            return (this as ICustomTypeDescriptor).GetProperties();
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            List<PropertyDescriptorEx> list = new List<PropertyDescriptorEx>(m_descriptors.Count);
            foreach (string key in m_descriptors.Keys)
            {
                list.Add(m_descriptors[key]);
            }

            return new PropertyDescriptorCollection(list.ToArray());
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        #endregion

        internal object ActualObject
        {
            get { return m_objToRepresent; }
        }
    }
}
