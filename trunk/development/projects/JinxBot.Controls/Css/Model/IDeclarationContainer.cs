using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace JinxBot.Controls.CSS
{
	/// <summary></summary>
    internal interface IDeclarationContainer
    {
		/// <summary></summary>
		List<Declaration> Declarations { get; set; }
	}
}