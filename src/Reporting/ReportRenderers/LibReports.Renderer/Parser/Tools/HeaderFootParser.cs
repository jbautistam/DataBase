using System;

using Bau.Libraries.LibCommonHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibReports.Renderer.Models.Contents;

namespace Bau.Libraries.LibReports.Renderer.Parser.Tools
{
	/// <summary>
	///		Intérprete para cabeceras / pies
	/// </summary>
	internal class HeaderFootParser : ParserBase<HeaderFootReport>
	{
		internal HeaderFootParser(ReportParser reportParser) : base(reportParser) { }

		/// <summary>
		///		Interpreta un nodo
		/// </summary>
		protected override HeaderFootReport ParseInner(MLNode nodeML)
		{
			HeaderFootReport header;

				// Interpreta la cabecera o el pie
				switch (nodeML.Name)
				{
					case "Header":
							header = Parse(nodeML, HeaderFootReport.HeaderFootType.Header);
						break;
					case "Foot":
							header = Parse(nodeML, HeaderFootReport.HeaderFootType.Foot);
						break;
					default:
						throw new NotImplementedException("No se reconoce el tipo de cabecera");
				}
				// Devuelve la cabecera o el pie
				return header;
		}

		/// <summary>
		///		Interpreta el XML de una cabecera o pie
		/// </summary>
		private HeaderFootReport Parse(MLNode nodeML, HeaderFootReport.HeaderFootType type)
		{
			HeaderFootReport header = new HeaderFootReport(Parent, type);

				// Obtiene los datos
				header.StartPage = nodeML.Attributes["StartPage"].Value.GetInt();
				header.EndPage = nodeML.Attributes["EndPage"].Value.GetInt();
				header.Group = nodeML.Attributes["Group"].Value;
				// Obtiene el tipo para el que se deben imprimir la cabecera / pie
				nodeML.Attributes["Type"].Value.GetEnum(HeaderFootReport.PageTarget.All);
				// Lee el contenido
				header.Content = new SectionParser(ReportParser).Parse(header, nodeML);
				// Devuelve la cabecera / pie
				return header;
		}
	}
}
