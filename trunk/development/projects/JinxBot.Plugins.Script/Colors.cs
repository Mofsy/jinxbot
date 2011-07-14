using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic.Library;
using Jurassic;
using System.Drawing;

namespace JinxBot.Plugins.Script
{
    internal class Colors : ObjectInstance
    {
        private static Dictionary<string, Color> _colorSet = typeof(Color).GetProperties(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public).ToDictionary(pi => pi.Name, pi => Color.FromName(pi.Name));

        public Colors(ScriptEngine engine)
            : base(engine)
        {
            engine.SetGlobalValue("Color", new ColorConstructor(engine));

            foreach (string key in _colorSet.Keys)
            {
                ColorInstance instance = new ColorInstance(engine, key, _colorSet[key]);
                this.DefineProperty(key, new PropertyDescriptor(instance, PropertyAttributes.Enumerable | PropertyAttributes.Sealed), false);
            }
        }
    }

    internal class ColorConstructor : ClrFunction
    {
        public ColorConstructor(ScriptEngine engine)
            : base(engine.Function.InstancePrototype, "Color", new ColorInstance(engine.Object.InstancePrototype))
        {

        }

        public ColorInstance Construct()
        {
            return new ColorInstance(this.InstancePrototype);
        }
    }


    internal class ColorInstance : ObjectInstance
    {
        public ColorInstance(ScriptEngine engine, string name, Color color)
            : base(engine.Object.Prototype)
        {
            this.PopulateFunctions();

            this.DefineProperty("hex", new PropertyDescriptor(string.Format("#{0:x2}{1:x2}{2:x2}", color.R, color.G, color.B), PropertyAttributes.Sealed | PropertyAttributes.Enumerable), false);
            this.DefineProperty("r", new PropertyDescriptor((int)color.R, PropertyAttributes.Enumerable | PropertyAttributes.Sealed), false);
            this.DefineProperty("g", new PropertyDescriptor((int)color.G, PropertyAttributes.Sealed | PropertyAttributes.Enumerable), false);
            this.DefineProperty("b", new PropertyDescriptor((int)color.B, PropertyAttributes.Enumerable | PropertyAttributes.Sealed), false);
            this.DefineProperty("rgb", new PropertyDescriptor((int)(color.ToArgb() & 0x00ffffff), PropertyAttributes.Sealed | PropertyAttributes.Enumerable), false);
            this.DefineProperty("name", new PropertyDescriptor(name, PropertyAttributes.Enumerable | PropertyAttributes.Sealed), false);
            this.DefineProperty("__predefined__", new PropertyDescriptor(engine.Object.Prototype, PropertyAttributes.Sealed | PropertyAttributes.NonEnumerable), false);
        }

        public ColorInstance(ObjectInstance prototype)
            : base(prototype)
        {
            this.PopulateFunctions();

            this.SetPropertyValue("r", 0, false);
            this.SetPropertyValue("g", 0, false);
            this.SetPropertyValue("b", 0, false);

            ResetCalculatedProperties();
        }

        public int R { get { return (int)this.GetPropertyValue("r"); } }
        public int G { get { return (int)this.GetPropertyValue("g"); } }
        public int B { get { return (int)this.GetPropertyValue("b"); } }

        [JSFunction(Name = "setR")]
        public void SetR(int r)
        {
            if (this.HasProperty("__predefined__"))
                return;

            if (r < 0) r = 0;
            else if (r > 255) r = 255;

            this.SetPropertyValue("r", r, false);

            ResetCalculatedProperties();
        }

        [JSFunction(Name = "setG")]
        public void SetG(int g)
        {
            if (this.HasProperty("__predefined__"))
                return;

            if (g < 0) g = 0;
            else if (g > 255) g = 255;

            this.SetPropertyValue("g", g, false);

            ResetCalculatedProperties();
        }

        [JSFunction(Name = "setB")]
        public void SetB(int b)
        {
            if (this.HasProperty("__predefined__"))
                return;

            if (b < 0) b = 0;
            else if (b > 255) b = 255;

            this.SetPropertyValue("b", b, false);

            ResetCalculatedProperties();
        }

        private void ResetCalculatedProperties()
        {
            int r = (int)this.GetPropertyValue("r");
            int g = (int)this.GetPropertyValue("g");
            int b = (int)this.GetPropertyValue("b");

            this.SetPropertyValue("hex", string.Format("#{0:x2}{1:x2}{2:x2}", r, g, b), false);
            this.SetPropertyValue("rgb", (int)((r << 16) | (g << 8) | b), false);

        }
    }
}
