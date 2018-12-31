using System;

using Bau.Libraries.LibReports.Renderer.Models.Base;

namespace Bau.Libraries.LibReports.Renderer.Models.Contents
{
	/// <summary>
	///		Selector de cabecera / pie
	/// </summary>
	public class HeaderFootSelector : ContentReportBase
	{
		public HeaderFootSelector(ContentReportBase parent, HeaderFootReport.HeaderFootType type) : base(parent)
		{
			Type = type;
			Visible = true;
			Group = null;
		}

		/// <summary>
		///		Clona el objeto
		/// </summary>
		public HeaderFootSelector Clone(ContentReportBase parent)
		{
			HeaderFootSelector selector = new HeaderFootSelector(parent, Type);

				// Asigna los datos
				selector.Visible = Visible;
				selector.Group = Group;
				// Devuelve el selector
				return selector;
		}

		/// <summary>
		///		Tipo de cabecera / pie al que se aplica el selector
		/// </summary>
		public HeaderFootReport.HeaderFootType Type { get; set; }

		/// <summary>
		///		Indica si a partir de la página donde se muestra esta sección, se deben mostrar cabeceras / pies
		/// </summary>
		public bool Visible { get; set; }

		/// <summary>
		///		Grupo del que se obtienen las cabeceras / pies
		/// </summary>
		public string Group { get; set; }
	}
}
