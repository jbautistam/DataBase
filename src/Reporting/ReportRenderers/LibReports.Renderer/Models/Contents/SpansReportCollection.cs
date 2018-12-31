using System;

using Bau.Libraries.LibReports.Renderer.Models.Base;

namespace Bau.Libraries.LibReports.Renderer.Models.Contents
{
	/// <summary>
	///		Colección de <see cref="SpanReport"/>
	/// </summary>
	public class SpansReportCollection : Base.ClassBaseCollection<SpanReport>
	{
		/// <summary>
		///		Añade un span con el texto
		/// </summary>
		public void Add(ContentReportBase parent, string text)
		{
			Add(new SpanReport(parent, text));
		}

		/// <summary>
		///		Clona la colección de <see cref="SpanReport"/>
		/// </summary>
		public SpansReportCollection Clone(ContentReportBase parent)
		{
			SpansReportCollection objColSpans = new SpansReportCollection();

				// Añade los span hijo
				foreach (SpanReport span in this)
					objColSpans.Add(span.Clone(parent));
				// Devuelve la colección clonada
				return objColSpans;
		}
	}
}
