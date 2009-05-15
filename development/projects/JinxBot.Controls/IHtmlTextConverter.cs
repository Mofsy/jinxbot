using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JinxBot.Controls
{
    public interface IHtmlTextConverter
    {
        string Convert(string html, DisplayBox document);
    }
}
