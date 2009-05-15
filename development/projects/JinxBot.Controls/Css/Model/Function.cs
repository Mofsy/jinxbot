using System;
using System.Xml.Serialization;

namespace JinxBot.Controls.CSS
{
    /// <summary></summary>
    internal class Function
    {
        private string name;
        private Expression expression;

        /// <summary></summary>
        [XmlAttribute("name")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary></summary>
        [XmlElement("Expression")]
        public Expression Expression
        {
            get { return expression; }
            set { expression = value; }
        }

        /// <summary></summary>
        /// <returns></returns>
        public override string ToString()
        {
            /*
function<out Function func> =	(. func = new Function();
                                    Expression exp = null;
                                .)
    ident						(. func.Name = t.val; .)
    '(' expr<out exp>			(. func.Expression = exp; .)
    ')'
.
            */
            System.Text.StringBuilder txt = new System.Text.StringBuilder();
            txt.AppendFormat("{0}(", name);
            if (expression != null)
            {
                bool first = true;
                foreach (Term t in expression.Terms)
                {
                    //if (first) { first = false; } else { txt.Append(" "); }
                    txt.Append(t.ToString());
                }
            }
            txt.Append(")");
            return txt.ToString();
        }
    }
}