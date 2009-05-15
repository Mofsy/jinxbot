using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JinxBot.Controls.CSS;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing;

namespace JinxBot.Controls
{
    internal class UbbcHtmlConverter : IHtmlTextConverter
    {
        private static Regex PerLine = new Regex("<P>(?<withinLine>.*?)</P>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static Regex Content = new Regex("<SPAN (?:style=\"COLOR: (?<color>.*?)\"|class=(?<className>.*?)>)(?<textContent>.*?)</SPAN>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        #region IHtmlTextConverter Members

        public string Convert(string html, DisplayBox document)
        {
            CSSParser parser = new CSSParser();
            var doc = parser.ParseText(File.ReadAllText(document.StylesheetUri.OriginalString.Substring(8)));
            return ParseSubtext(html.Replace(Environment.NewLine, string.Empty), doc);
        }

        private string ParseSubtext(string html, CSSDocument doc)
        {
            Dictionary<string, string> selectorToColor = new Dictionary<string, string>();
            StringBuilder result = new StringBuilder();
            MatchCollection matches = PerLine.Matches(html);
            foreach (Match match in matches)
            {
                ParseLine(match.Groups["withinLine"].Value, doc, selectorToColor, result);
            }
            return result.ToString();
        }

        private void ParseLine(string line, CSSDocument doc, Dictionary<string, string> selectorToColor, StringBuilder result)
        {
            MatchCollection matches = Content.Matches(line);
            foreach (Match match in matches)
            {
                if (match.Groups["color"].Success)
                {
                    result.AppendFormat("[color={0}]{1}[/color]", match.Groups["color"].Value, match.Groups["textContent"].Value);
                }
                else
                {
                    string color = GetColorFromCss(match.Groups["className"].Value, selectorToColor, doc);
                    if (color == null) color = "#000000";
                    if (!selectorToColor.ContainsKey(match.Groups["className"].Value))
                        selectorToColor.Add(match.Groups["className"].Value, color);

                    result.AppendFormat("[color={0}]{1}[/color]", color, match.Groups["textContent"].Value);
                }
            }
            result.AppendLine();
        }

        private string GetColorFromCss(string selector, Dictionary<string, string> selectorToColor, CSSDocument doc)
        {
            if (!selectorToColor.ContainsKey(selector))
            {
                foreach (RuleSet ruleSet in doc.RuleSets)
                {
                    foreach (Selector select in ruleSet.Selectors)
                    {
                        foreach (SimpleSelector sel in select.SimpleSelectors)
                        {
                            if (selector.Equals(sel.Class, StringComparison.Ordinal))
                            {
                                var decl = (from d in ruleSet.Declarations
                                            where d.Name.Equals("color", StringComparison.OrdinalIgnoreCase)
                                            select d).FirstOrDefault();
                                if (decl != null)
                                {
                                    string color = decl.Expression.ToString();
                                    Color result = Color.FromName(color);
                                    if (result.A == 0)
                                    {
                                        // Color.FromName returns all 0 if it does not match
                                        return color;
                                    }
                                    else
                                    {
                                        return string.Format("#{0:x2}{1:x2}{2:x2}", result.R, result.G, result.B);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                return selectorToColor[selector];
            }

            return null;
        }

        #endregion
    }
}
