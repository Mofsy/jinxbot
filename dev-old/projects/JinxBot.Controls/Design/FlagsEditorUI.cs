using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace JinxBot.Controls.Design
{
    internal partial class FlagsEditorUI<T> : UserControl
    {
        private EnumTypeConverter<T> m_converter;

        public FlagsEditorUI()
        {
            InitializeComponent();

            m_converter = new EnumTypeConverter<T>();

            foreach (object item in m_converter.GetStandardValues())
            {
                clbFlags.Items.Add(item, false);
            }
        }

        public T Value
        {
            get
            {
                return GetCheckboxedValue();
            }
            set
            {
                SetCheckboxes(value);
            }
        }

        private T GetCheckboxedValue()
        {
            // this is kind of a cheat.  it uses the type converter after building a string with each check box item.
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < clbFlags.Items.Count; i++)
            {
                if (clbFlags.GetItemChecked(i))
                {
                    if (sb.Length == 0)
                        sb.Append(clbFlags.Items[i].ToString());
                    else
                        sb.AppendFormat(",{0}", clbFlags.Items[i]);
                }
            }

            if (sb.Length == 0)
                sb.AppendFormat("{0}", EnumTypeConverter<T>.GetNameForEmptyValue());

            return (T)m_converter.ConvertFrom(sb.ToString());
        }

        private void SetCheckboxes(T value)
        {
            string name = EnumTypeConverter<T>.GetNameForEmptyValue();
            int index = clbFlags.Items.IndexOf(name);

            ulong val = Convert.ToUInt64(value, CultureInfo.InvariantCulture);
            if (val == 0)
            {
                for (int i = 0; i < clbFlags.Items.Count; i++)
                {
                    m_settingChecked = true;
                    clbFlags.SetItemChecked(i, i == index);
                }
            }
            else
            {
                for (int i = 0; i < clbFlags.Items.Count; i++)
                {
                    string currentItemName = clbFlags.Items[i].ToString();
                    ulong nameValue = Convert.ToUInt64((T)m_converter.ConvertFrom(currentItemName), CultureInfo.InvariantCulture);
                    m_settingChecked = true;
                    if ((nameValue & val) != 0)
                    {
                        clbFlags.SetItemChecked(i, true);
                    }
                    else
                    {
                        clbFlags.SetItemChecked(i, false);
                    }
                }
            }

            m_settingChecked = false;
        }

        private bool m_settingChecked = false;
        private void clbFlags_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (m_settingChecked)
            {
                m_settingChecked = false;
                return;
            }

            // check the Zero value when others are unchecked
            // or uncheck the zero value when others are checked
            string name = EnumTypeConverter<T>.GetNameForEmptyValue();
            if (name != null)
            {
                int index = clbFlags.Items.IndexOf(EnumTypeConverter<T>.GetNameForEmptyValue());
                if (e.Index == index && e.NewValue == CheckState.Checked)
                {
                    for (int i = 0; i < clbFlags.Items.Count; i++)
                    {
                        if (i != index)
                        {
                            m_settingChecked = true;
                            clbFlags.SetItemChecked(i, false);
                        }
                    }
                }
                else if (e.Index != index && e.NewValue == CheckState.Checked)
                {
                    m_settingChecked = true;
                    clbFlags.SetItemChecked(index, false);
                }
            }

            m_settingChecked = false;
        }
    }
}
