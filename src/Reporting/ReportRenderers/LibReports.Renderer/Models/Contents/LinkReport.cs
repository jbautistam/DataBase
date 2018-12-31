using System;

using Bau.Libraries.LibReports.Renderer.Models.Base;

namespace Bau.Libraries.LibReports.Renderer.Models.Contents
{
	/// <summary>
	///		Clase con los datos de un enlace para un informe
	/// </summary>
	public class LinkReport : Base.ContentReportBase
	{
		/// <summary>
		///		Tipo del vínculo
		/// </summary>
		public enum LinkType
		{
			/// <summary>Marcador</summary>
			Bookmark,
			/// <summary>Hipervínculo</summary>
			Anchor
		}

		public LinkReport(ContentReportBase parent) : base(parent) { }

		/// <summary>
		///		Clona el objeto
		/// </summary>
		public LinkReport Clone(ContentReportBase parent)
		{
			LinkReport link = new LinkReport(parent);

				// Clona las propiedades comunes
				base.CloneToTarget(link);
				// Clona las propiedades
				link.Type = Type;
				link.Text = Text;
				link.Target = Target;
				// Devuelve el vínculo
				return link;
		}

		/// <summary>
		///		Tipo de vínculo
		/// </summary>
		public LinkType Type { get; set; }

		/// <summary>
		///		Texto del vínculo
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		///		Destino del vínculo
		/// </summary>
		public string Target { get; set; }
	}
}
