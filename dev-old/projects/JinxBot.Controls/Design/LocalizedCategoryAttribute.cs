using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Resources;

namespace JinxBot.Controls.Design
{
    [AttributeUsage(AttributeTargets.All)]
    internal sealed class LocalizedCategoryAttribute : CategoryAttribute
    {
        public LocalizedCategoryAttribute(string cat)
            : base(cat)
        {
        }

        protected override string GetLocalizedString(string value)
        {
            ResourceManager manager = new ResourceManager("Resources", GetType().Assembly);
            try
            {
                return manager.GetString(value);
            }
            catch (MissingManifestResourceException)
            {
                return value;
            }
        }
    }
}
