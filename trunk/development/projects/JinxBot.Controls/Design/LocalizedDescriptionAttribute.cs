using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Resources;

namespace JinxBot.Controls.Design
{
    [AttributeUsage(AttributeTargets.All)]
    internal sealed class LocalizedDescriptionAttribute : DescriptionAttribute
    {
        public LocalizedDescriptionAttribute(string desc)
            : base(desc)
        {

        }

        public override string Description
        {
            get
            {
                string index = base.Description;
                ResourceManager manager = new ResourceManager("Resources", GetType().Assembly);
                try
                {
                    return manager.GetString(index);
                }
                catch (MissingManifestResourceException)
                {
                    return index;
                }
            }
        }
    }
}
