using System;
using System.Collections.Generic;
using System.Text;

namespace JinxBot.Controls
{
    /// <summary>
    /// When applied to a class, indicates the <see>Type</see> that should be used to render a ChatNode to HTML.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ChatNodeRendererAttribute : Attribute
    {
        private Type m_type;
        /// <summary>
        /// Creates a new instance of <see>ChatNodeRendererAttribute</see>.
        /// </summary>
        /// <param name="rendererType">The <see>Type</see> that is a ChatNodeRenderer-derived class.</param>
        public ChatNodeRendererAttribute(Type rendererType)
        {
            Contract.RequireInstance(rendererType, "rendererType");

            m_type = rendererType;
        }

        /// <summary>
        /// Gets the <see>Type</see> specified by the renderer.
        /// </summary>
        public Type RendererType
        {
            get { return m_type; }
        }

        /// <summary>
        /// Gets a <see>Type</see> that is the renderer for a specific chat node Type, as specified by a <see>ChatNodeRendererAttribute</see>
        /// applied to that class.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>A Type specified as a ChatNodeRenderer.</returns>
        public static Type RetrieveFromType(Type type)
        {
            if (!type.IsClass)
                throw new InvalidCastException(Resources.ChatNodeRendererAttributeCanOnlyRetrieveFromClassType);

            ChatNodeRendererAttribute[] cnra = type.GetCustomAttributes(typeof(ChatNodeRendererAttribute), false) as ChatNodeRendererAttribute[];
            if (cnra == null || cnra.Length == 0)
                return typeof(ChatNodeRenderer);

            return cnra[0].RendererType;
        }
    }
}
