using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JinxBot.Controls
{
    /// <summary>
    /// Converts MSHTML-based HTML text into another type of markup or text.
    /// </summary>
    public interface IHtmlTextConverter
    {
        /// <summary>
        /// Converts HTML text into the markup supported by this converter.
        /// </summary>
        /// <param name="html">The source HTML fragment.</param>
        /// <param name="document">The document containing the HTML fragment being converted.</param>
        /// <returns>Text in the target conversion mechanism.</returns>
        /// <remarks>
        /// <para>For more information, review the source code of the <c>UbbcHtmlConverter</c> class.</para>
        /// </remarks>
        string Convert(string html, DisplayBox document);
    }
}
