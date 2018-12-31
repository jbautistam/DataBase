using System;

using Bau.Libraries.LibCommonHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibReports.Renderer.Models.Contents;

namespace Bau.Libraries.LibReports.Renderer.Parser.Tools
{
	/// <summary>
	///		Intérprete para los datos de un selector de cabecera / pie a partir de XML
	/// </summary>
	internal class HeaderFootSelectorParser : ParserBase<HeaderFootSelector>
	{
		internal HeaderFootSelectorParser(ReportParser reportParser) : base(reportParser) { }

		/// <summary>
		///		Interpreta los datos de un selector de cabecera o página
		/// </summary>
		protected override HeaderFootSelector ParseInner(MLNode nodeML)
		{
			HeaderFootSelector selector;

				// Crea el selector a partir del nombre del nodo
				if (nodeML.Name == "HeaderSelector")
					selector = new HeaderFootSelector(Parent, HeaderFootReport.HeaderFootType.Header);
				else
					selector = new HeaderFootSelector(Parent, HeaderFootReport.HeaderFootType.Foot);
				// Obtiene los valores de los atributos
				selector.Visible = nodeML.Attributes["Visible"].Value.GetBool(true);
				selector.Group = nodeML.Attributes["Group"].Value;
				// Devuelve el selector
				return selector;
		}
	}
}
