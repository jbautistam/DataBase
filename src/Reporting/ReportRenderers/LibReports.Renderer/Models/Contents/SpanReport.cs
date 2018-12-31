using System;

using Bau.Libraries.LibCommonHelper.Extensors;
using Bau.Libraries.LibReports.Renderer.Models.Base;

namespace Bau.Libraries.LibReports.Renderer.Models.Contents
{
	/// <summary>
	///		Contenido en un span con formato (normalmente para un <see cref="ParagraphReport"/>)
	/// </summary>
	public class SpanReport : ContentReportBase
	{
		public SpanReport(ContentReportBase parent) : this(parent, null) { }

		public SpanReport(ContentReportBase parent, string text) : base(parent)
		{
			Text = text;
		}

		/// <summary>
		///		Clona los datos del span
		/// </summary>
		public SpanReport Clone(ContentReportBase parent)
		{
			SpanReport span = new SpanReport(parent);

				// Asigna los datos básicos
				base.CloneToTarget(span);
				// Asigna las propiedades particulares
				if (span.ClassId.IsEmpty())
					span.ClassId = parent.ClassId;
				span.Text = Text;
				// Devuelve el nuevo span
				return span;
		}

		/// <summary>
		///		Texto del span
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		///		Contenido condicional
		/// </summary>
		public ConditionalSectionReport ConditionalContent { get; set; }
	}
}
