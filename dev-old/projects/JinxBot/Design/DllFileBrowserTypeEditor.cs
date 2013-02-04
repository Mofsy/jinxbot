using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Design;
using System.Windows.Forms;

namespace JinxBot.Design
{
    internal class DllFileBrowserTypeEditor : UITypeEditor
    {
        private OpenFileDialog m_fileDlg;

        public DllFileBrowserTypeEditor()
        {
            m_fileDlg = new OpenFileDialog();
            m_fileDlg.Filter = "Dynamically-linked libraries (*.dll; *.snp)|*.dll;*.snp|All files (*.*)|*.*";
            m_fileDlg.Multiselect = false;
        }

        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (m_fileDlg.ShowDialog() == DialogResult.OK)
            {
                return m_fileDlg.FileName;
            }
            else
            {
                return value;
            }
        }
    }
}
