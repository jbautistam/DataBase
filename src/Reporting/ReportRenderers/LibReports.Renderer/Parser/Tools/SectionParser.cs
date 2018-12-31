using System;

using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibReports.Renderer.Models.Contents;

namespace Bau.Libraries.LibReports.Renderer.Parser.Tools
{
	/// <summary>
	///		Intérprete de los datos de una sección a partir de XML
	/// </summary>
	internal class SectionParser : ParserBase<SectionReport>
	{
		internal SectionParser(ReportParser reportParser) : base(reportParser) { }

		/// <summary>
		///		Interpreta los datos de una sección
		/// </summary>
		protected override SectionReport ParseInner(MLNode nodeML)
		{
			SectionReport section = new SectionReport(Parent);

				// Interpreta los nodos
				foreach (MLNode childML in nodeML.Nodes)
					switch (childML.Name)
					{
						//case "DataTableProvider":
						//		section.Command = ReportParser.ParserLocator.ParseDataComand(section, childML);
						//	break;
						default:
								section.Contents.Add(ReportParser.ParserLocator.Parse<Models.Base.ContentReportBase>(section, childML));
							break;
					}
				// Devuelve la sección interpretada
				return section;
		}
	}
}
