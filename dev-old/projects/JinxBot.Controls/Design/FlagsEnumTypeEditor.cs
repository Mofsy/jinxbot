using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace JinxBot.Controls.Design
{
    internal class FlagsEnumTypeEditor<T> : UITypeEditor, IDisposable
    {
        private FlagsEditorUI<T> m_ui;
        public FlagsEnumTypeEditor() { }

        public override bool GetPaintValueSupported(System.ComponentModel.ITypeDescriptorContext context)
        {
            return false;
        }

        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                IWindowsFormsEditorService editorService = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
                if (editorService == null) return value;

                if (m_ui == null)
                {
                    this.m_ui = new FlagsEditorUI<T>();
                }

                m_ui.Value = (T)value;

                editorService.DropDownControl(m_ui);
                T result = this.m_ui.Value;

                value = result;

            }

            return value;
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_ui != null)
                {
                    m_ui.Dispose();
                    m_ui = null;
                }
            }
        }

        #endregion
    }
}
