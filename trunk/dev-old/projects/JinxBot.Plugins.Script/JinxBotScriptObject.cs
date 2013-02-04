using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic.Library;
using Jurassic;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections;
using BNSharp.BattleNet;

namespace JinxBot.Plugins.Script
{
    internal class JinxBotScriptObjectInstance : ObjectInstance
    {
        internal JinxBotScriptObjectInstance(ScriptEngine engine, object src) 
            : base(engine.Object.Prototype)
        {
            Type srcType = src.GetType();
            var properties = srcType.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(prop => prop.PropertyType != typeof(BattleNetClient.ParseData));
            foreach (PropertyInfo prop in properties)
            {
                object valSrc = prop.GetValue(src, null);
                object value = ConvertValue(valSrc, engine);
                this.DefineProperty(prop.Name, new PropertyDescriptor(value, Jurassic.Library.PropertyAttributes.Sealed | Jurassic.Library.PropertyAttributes.Enumerable), false);
            }
        }

        private static object ConvertValue(object src, ScriptEngine engine)
        {
            if (src == null)
                return null;
            Type type = src.GetType();
            object value = null;

            if (type.IsEnum)
            {
                // convert to string
                value = src.ToString();
            }
            else if (type == typeof(string))
            {
                value = src;
            }
            else if (type == typeof(int) || type == typeof(short) || type == typeof(byte))
            {
                value = (int)src;
            }
            else if (type == typeof(double) || type == typeof(long) || type == typeof(float))
            {
                value = (double)src;
            }
            else if (type == typeof(bool))
            {
                value = (bool)src;
            }
            else if (type.IsSubclassOf(typeof(ValueType)))
            {
                value = value.ToString();
            }
            else if (typeof(IDictionary<string, object>).IsAssignableFrom(type))
            {
                IDictionary<string, object> dict = value as IDictionary<string, object>;
                ObjectInstance jsObj = engine.Object.Construct();
                foreach (string key in dict.Keys)
                {
                    jsObj.DefineProperty(key, new PropertyDescriptor(ConvertValue(dict[key], engine), Jurassic.Library.PropertyAttributes.Sealed | Jurassic.Library.PropertyAttributes.Enumerable), false);
                }

                value = jsObj;
            }
            else if (typeof(IDictionary).IsAssignableFrom(type))
            {
                IDictionary dict = src as IDictionary;
                ObjectInstance jsObj = engine.Object.Construct();
                foreach (object key in dict.Keys)
                {
                    if (key is string)
                    {
                        jsObj.DefineProperty(key as string, new PropertyDescriptor(ConvertValue(dict[key], engine), Jurassic.Library.PropertyAttributes.Enumerable | Jurassic.Library.PropertyAttributes.Sealed), false);
                    }
                    else
                    {
                        throw new NotSupportedException("Cannot have an object key in a dictionary.");
                    }
                }
                value = jsObj;
            }
            else if (type.IsArray || typeof(IEnumerable).IsAssignableFrom(type))
            {
                IEnumerable collection = src as IEnumerable;
                ArrayInstance collectionResult = engine.Array.Construct();
                foreach (object o in collection)
                    collectionResult.Push(ConvertValue(o, engine));

                value = collectionResult;
            }
            else
            {
                value = new JinxBotScriptObjectInstance(engine, src);
            }

            return value;
        }

        
    }
}
