using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JinxBot.Plugins.UI
{
    /// <summary>
    /// When applied to a class, indicates the class that can be used to render it into a <see>CustomDrawnListBox</see>; when 
    /// applied to an assembly, indicates additional rendering association.  This class cannot be inherited.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly)]
    public sealed class CustomListBoxRendererAttribute : Attribute
    {
        /// <summary>
        /// This constructor is only valid when applied to a type.  Instantiates a new <see>CustomListBoxRendererAttribute</see>.
        /// </summary>
        /// <param name="rendererType">The class that will perform the rendering.</param>
        /// <remarks>
        /// <para>This constructor should only be used when scoped to a class; if applied to an assembly, 
        /// the loader will raise an exception and release the </para>
        /// </remarks>
        public CustomListBoxRendererAttribute(Type rendererType)
        {

        }

        /// <summary>
        /// This constructor is only valid when applied to assemblies.  Instantiates a new <see>CustomListBoxRendererAttribute</see>.
        /// </summary>
        /// <param name="rendererType">The class that will perform the rendering.</param>
        /// <param name="renderedType">The class that will be rendered by the renderer.</param>
        /// <remarks>
        /// <para>This constructor should only be used when scoped to assemblies; if applied to a class, the 
        /// <paramref name="renderedType"/> parameter will be ignored.</para>
        /// </remarks>
        public CustomListBoxRendererAttribute(Type rendererType, Type renderedType)
        {

        }
    }
}