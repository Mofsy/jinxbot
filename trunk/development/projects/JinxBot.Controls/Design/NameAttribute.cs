using System;
using System.Collections.Generic;
using System.Text;

namespace JinxBot.Controls.Design
{
    /// <summary>
    /// When applied to a property or field, specifies the "friendly name" that the property will register as.
    /// </summary>
    // suppressing CA1813 because keeping this non-sealed allows resource files to be used.
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes")]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class NameAttribute : Attribute
    {
        private string m_name;

        /// <summary>
        /// Initializes a NameAttribute with the specified property name.
        /// </summary>
        /// <param name="name">The name to specify as the custom name of the property.</param>
        public NameAttribute(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            m_name = name;
        }

        /// <summary>
        /// Gets (and in derived classes sets) the Name of the property being described.
        /// </summary>
        /// <remarks>
        /// <para>When overridden in a derived class, can be used to retrieve localized strings.</para>
        /// </remarks>
        public virtual string Name
        {
            get { return m_name; }
        }
    }
}
