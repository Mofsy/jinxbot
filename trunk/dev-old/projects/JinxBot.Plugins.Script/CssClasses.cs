using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic;
using Jurassic.Library;

namespace JinxBot.Plugins.Script
{
    internal class CssClasses : ObjectInstance
    {
        private static Dictionary<string, string> _cssSet = typeof(JinxBot.Views.Chat.CssClasses).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).Where(fi => fi.IsLiteral).ToDictionary(fi => fi.Name, fi => fi.GetValue(null) as string);

        public CssClasses(ScriptEngine engine)
            : base(engine)
        {
            foreach (string key in _cssSet.Keys)
            {
                this.DefineProperty(key, new PropertyDescriptor(_cssSet[key], PropertyAttributes.Enumerable | PropertyAttributes.Sealed), false);
            }
        }
    }
}
