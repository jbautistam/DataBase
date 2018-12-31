using System;

using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibReports.Renderer.Models.Base;
using Bau.Libraries.LibReports.Renderer.Models.Contents;

namespace Bau.Libraries.LibReports.Renderer.Parser.Tools
{
	/// <summary>
	///		Parser de <see cref="ConditionalSectionReport"/>
	/// </summary>
	internal class ConditionalSectionParser : ParserBase<ConditionalSectionReport>
	{
		internal ConditionalSectionParser(ReportParser reportParser) : base(reportParser) { }

		/// <summary>
		///		Interpreta un nodo condicional
		/// </summary>
		protected override ConditionalSectionReport ParseInner(MLNode nodeML)
		{
			ConditionalSectionReport condition = new ConditionalSectionReport(Parent);

				// Añade el contenido de los nodos
				foreach (MLNode childML in nodeML.Nodes)
					switch (childML.Name)
					{
						case "Condition":
							condition.Condition = childML.Value;
							break;
						case "Then":
							condition.ThenContent = ParseContent(condition, childML);
							break;
						case "Else":
							condition.ElseContent = ParseContent(condition, childML);
							break;
					}
				// Devuelve la sección condicional
				return condition;
		}

		/// <summary>
		///		Interpreta el contenido de una condición
		/// </summary>
		private ContentReportBase ParseContent(ConditionalSectionReport condition, MLNode nodeML)
		{
			SectionReport section = new SectionParser(ReportParser).Parse(condition, nodeML);

				// Si hay más de un elemento en la sección, devuelve la sección, si hay
				// un solo elemento, devuelve todos los elementos
				if (section.Contents.Count > 0)
					return section;
				else // ...  devuelve el primer elemento
				{ 
					// Reasigna el padre
					section.Contents[0].Parent = condition;
					// Devuelve el primer elemento 
					return section.Contents[0];
				}
		}

		/// <summary>
		///		Interpreta un span
		/// </summary>
		internal ConditionalSectionReport ParseSpan(SpanReport span, MLNode nodeML)
		{
			ConditionalSectionReport condition = new ConditionalSectionReport(span);

				// Añade el contenido de los nodos
				foreach (MLNode childML in nodeML.Nodes)
					switch (childML.Name)
					{
						case "Condition":
								condition.Condition = childML.Value;
							break;
						case "Then":
								condition.ThenContent = new SpanParser(ReportParser).Parse(condition, childML);
							break;
						case "Else":
								condition.ElseContent = new SpanParser(ReportParser).Parse(condition, childML);
							break;
					}
				// Devuelve la sección condicional
				return condition;
		}
	}
}
