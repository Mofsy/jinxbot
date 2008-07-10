using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using BNSharp.BattleNet;

namespace JinxBot.Design
{
    internal sealed class ProductStringTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            Product pr = (from p in Product.GetAllProducts()
                          where p.Name == (value as string)
                          select p).FirstOrDefault();
            return pr.ProductCode;
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            Product pr = Product.GetByProductCode(value as string);
            if (pr == null)
                return null;
            else
                return pr.Name;
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            List<string> supportedProducts = new List<string> { 
                Product.StarcraftRetail.ProductCode,
                Product.StarcraftBroodWar.ProductCode, 
                Product.Warcraft2BNE.ProductCode, 
                Product.Diablo2Retail.ProductCode, 
                Product.Diablo2Expansion.ProductCode, 
                Product.Warcraft3Retail.ProductCode, 
                Product.Warcraft3Expansion.ProductCode
            };
            StandardValuesCollection svc = new StandardValuesCollection(supportedProducts);
            return svc;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}
