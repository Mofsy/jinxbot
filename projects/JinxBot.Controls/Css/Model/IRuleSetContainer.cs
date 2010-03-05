using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace JinxBot.Controls.CSS
{
	/// <summary></summary>
    internal interface IRuleSetContainer
    {
		/// <summary></summary>
		List<RuleSet> RuleSets { get; set; }
	}
}