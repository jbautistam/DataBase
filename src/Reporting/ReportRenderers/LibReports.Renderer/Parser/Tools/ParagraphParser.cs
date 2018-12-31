using System;

using Bau.Libraries.LibReports.Renderer.Models.Contents;
using Bau.Libraries.LibMarkupLanguage;

namespace Bau.Libraries.LibReports.Renderer.Parser.Tools
{
	/// <summary>
	///		Intérprete de párrafos
	/// </summary>
	internal class ParagraphParser : ParserBase<ParagraphReport>
	{
		internal ParagraphParser(ReportParser reportParser) : base(reportParser) { }

		/// <summary>
		///		Interpreta un párrafo
		/// </summary>
		protected override ParagraphReport ParseInner(MLNode nodeML)
		{
			ParagraphReport paragraph = new ParagraphReport(Parent);

				// Asigna los datos de los span del párrafo
				foreach (MLNode childML in nodeML.Nodes)
					if (childML.Name == "Span" || childML.Name == "If")
						paragraph.Contents.Add(new SpanParser(ReportParser).Parse(paragraph, childML));
					else if (childML.Name == "Link")
						paragraph.Contents.Add(new LinkParser(ReportParser).Parse(paragraph, childML));
				// Si no se ha asignado ningún span (porque no hay nodos hijo), supone que todo el texto pertenece al párrafo
				if (paragraph.Contents.Count == 0)
					paragraph.Contents.Add(new SpanReport(paragraph, ParserHelper.Normalize(nodeML.Value)));
				// Devuelve el párrafo
				return paragraph;
		}
	}
}
