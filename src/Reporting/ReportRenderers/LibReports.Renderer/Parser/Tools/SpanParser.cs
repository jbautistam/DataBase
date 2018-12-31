using System;

using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibReports.Renderer.Models.Contents;

namespace Bau.Libraries.LibReports.Renderer.Parser.Tools
{
	/// <summary>
	///		Parser de <see cref="SpanReport"/>
	/// </summary>
	internal class SpanParser : ParserBase<SpanReport>
	{
		internal SpanParser(ReportParser reportParser) : base(reportParser) { }

		/// <summary>
		///		Interpreta un nodo de Span
		/// </summary>
		protected override SpanReport ParseInner(MLNode nodeML)
		{
			SpanReport span = new SpanReport(Parent);

				// Dependiendo de si es un if o un span
				if (nodeML.Name == "If")
					span.ConditionalContent = new ConditionalSectionParser(ReportParser).ParseSpan(span, nodeML);
				else
				{ 
					// Interpreta los datos de estilo
					base.ParseContentCommon(span, nodeML);
					// Asigna el texto del span
					span.Text = ParserHelper.Normalize(nodeML.Value);
				}
				// Devuelve el objeto de span
				return span;
		}
	}
}
