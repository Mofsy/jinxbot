using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JinxBot.Plugins.UI
{
    public interface ICustomListBoxItemRenderer
    {
        void MeasureItem(CustomMeasurements e);

        void DrawItem(CustomItemDrawData e);
    }
}
