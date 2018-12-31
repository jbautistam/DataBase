using System;

using Bau.Libraries.LibCommonHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibReports.Renderer.Models.Contents;

namespace Bau.Libraries.LibReports.Renderer.Parser.Tools
{
	/// <summary>
	///		Intérprete de <see cref="LinkReport"/>
	/// </summary>
	internal class LinkParser : ParserBase<LinkReport>
	{
		internal LinkParser(ReportParser reportParser) : base(reportParser) { }

		/// <summary>
		///		Interpreta el contenido del nodo
		/// </summary>
		protected override LinkReport ParseInner(MLNode nodeML)
		{
			LinkReport link = new LinkReport(Parent);

				// Asigna el tipo
				link.Type = nodeML.Attributes["Type"].Value.GetEnum(LinkReport.LinkType.Anchor);
				// Asigna las propiedades
				link.Text = nodeML.Nodes["Text"].Value;
				link.Target = nodeML.Nodes["Target"].Value;
				// Devuelve el informe
				return link;
		}
	}
}
