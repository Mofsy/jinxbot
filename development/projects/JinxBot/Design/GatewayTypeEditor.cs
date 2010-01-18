using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using JinxBot.Views;
using BNSharp.BattleNet;

namespace JinxBot.Design
{
    internal sealed class GatewayTypeEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(
            ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            GatewayDesigner designer = new GatewayDesigner();
            designer.ChosenGateway = value as Gateway;
            if (designer.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                return designer.ChosenGateway;

            return base.EditValue(context, provider, value);
        }
    }
}
