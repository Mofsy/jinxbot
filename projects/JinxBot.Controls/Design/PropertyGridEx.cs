using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using System.Reflection;
using System.Reflection.Emit;
using System.Diagnostics;
using System.Drawing;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Security.Permissions;

namespace JinxBot.Controls.Design
{
    /// <summary>
    /// Implements a replacement for the Windows Forms <see cref="PropertyGrid">PropertyGrid</see> control.
    /// </summary>
    /// <remarks>
    /// <para>Custom property and enumeration names are supported through the use of the <see cref="JinxBot.Controls.Design.NameAttribute">NameAttribute</see>
    /// attribute, which specifies a friendly name that may include special characters and spaces normally reserved as tokenizers in programming languages
    /// targetting IL.  Enumerations marked with the <see cref="FlagsAttribute">[Flags]</see> attribute also offer a new designer which allow multiple 
    /// values to be selected using checkboxes.  Selecting an value that equates to zero automatically clears the other selected values.</para>
    /// <para>The <b>PropertyGridEx</b> control does not support designing multiple objects.  For more information, see the 
    /// <see cref="SelectedObjects">SelectedObjects</see> property remarks.</para>
    /// <para>As an alternative, if you require the ability to design multiple objects or may not replace the PropertyGrid control in a project already, 
    /// you can wrap the object to be designed within an instance of a <see cref="JinxBot.Controls.Design.TypeDescriptorSurrogate">TypeDescriptorSurrogate</see>
    /// class.  This class supports all of the functionality of the <b>PropertyGridEx</b> control, with the exception of multiple property tabs.</para>
    /// </remarks>
    // Suppressed error "Do not name with 'Ex' suffix."
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix"), PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
    [PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
    public class PropertyGridEx : PropertyGrid
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        private static Dictionary<string, Type> s_dynamicClosedTypes = new Dictionary<string, Type>();

        private object m_obj;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        private TypeDescriptorSurrogate m_surrogate;

        /// <summary>
        /// Creates a new instance of <see>PropertyGridEx</see>.
        /// </summary>
        public PropertyGridEx()
        {

        }

        /// <summary>
        /// Gets or sets the object being designed.
        /// </summary>
        public new object SelectedObject
        {
            get
            {
                return m_obj;
            }
            set
            {
                m_obj = value;
                TypeDescriptorSurrogate surrogate = new TypeDescriptorSurrogate(value);
                base.SelectedObject = surrogate;
                m_surrogate = surrogate;
            }
        }

        /// <summary>
        /// Gets or sets an array of objects being designed.
        /// </summary>
        /// <remarks>
        /// <para>The <b>PropertyGridEx</b> control only allows one object to be designed at a time.  Consequently, setting this property to an array 
        /// of more than one object will result in an exception being raised.</para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <c>value</c> is an array longer than a single item.</exception>
        // Suppressed CA1819 to keep interface consistent from parent.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public new object[] SelectedObjects
        {
            get
            {
                return new object[] { SelectedObject };
            }
            set
            {
                if (value != null && value.Length == 1)
                    SelectedObject = value[0];
                else if (value == null || value.Length == 0)
                    SelectedObject = null;
                else
                    throw new ArgumentOutOfRangeException("value", value, "PropertyGridEx does not support advanced design of multiple objects.");
            }
        }
    }
}
