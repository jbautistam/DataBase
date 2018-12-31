using System;

using Bau.Libraries.LibCommonHelper.Extensors;
using Bau.Libraries.LibReports.Renderer.Models.Base;

namespace Bau.Libraries.LibReports.Renderer.Models.Contents
{
	/// <summary>
	///		Párrafo
	/// </summary>
	public class ParagraphReport : ContentReportBase
	{
		public ParagraphReport(ContentReportBase parent) : this(parent, null) { }

		public ParagraphReport(ContentReportBase parent, string text) : base(parent)
		{
			if (!text.IsEmpty())
				Contents.Add(new SpanReport(this, text));
		}

		/// <summary>
		///		Obtiene el texto del primer span
		/// </summary>
		public string GetText()
		{
			string text = "";

				// Obtiene el texto
				foreach (ContentReportBase content in Contents)
					if (content is SpanReport)
						text = text.AddWithSeparator((content as SpanReport).Text, ",");
				// Devuelve el texto
				return text;
		}

		/// <summary>
		///		Clona los datos de un párrafo
		/// </summary>
		public ParagraphReport Clone(ContentReportBase parent)
		{
			ParagraphReport paragraph = new ParagraphReport(parent);

				// Asigna los datos básicos
				base.CloneToTarget(paragraph);
				// Clona los datos de los span
				foreach (ContentReportBase content in Contents)
					if (content is SpanReport)
						paragraph.Contents.Add((content as SpanReport).Clone(paragraph));
					else if (content is LinkReport)
						paragraph.Contents.Add((content as LinkReport).Clone(paragraph));
				// Devuelve el objeto clonado
				return paragraph;
		}

		/// <summary>
		///		Colección de span del párrafo
		/// </summary>
		public ClassBaseCollection<ContentReportBase> Contents { get; set; } = new ClassBaseCollection<ContentReportBase>();

		/// <summary>
		///		Indica si el párrafo está compuesto por un único contenido de tipo span
		/// </summary>
		public bool IsFullParagraph
		{
			get { return Contents.Count == 1 && Contents[0] is SpanReport; }
		}
	}
}
