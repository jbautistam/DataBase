using System;

using Bau.Libraries.LibReports.Renderer.Models.Base;

namespace Bau.Libraries.LibReports.Renderer.Models.Contents
{
	/// <summary>
	///		Elemento de una lista
	/// </summary>
	public class ListItemReport : ContentReportBase
	{
		public ListItemReport(ContentReportBase parent) : base(parent)
		{
			List = new ListReport(this);
		}

		/// <summary>
		///		Clona el elemento de la lista
		/// </summary>
		public ListItemReport Clone(ContentReportBase parent)
		{
			ListItemReport item = new ListItemReport(parent);

				// Clona el elemento
				base.CloneToTarget(item);
				// Asigna las propiedades
				item.Text = Text;
				// Devuelve el elemento
				return item;
		}

		/// <summary>
		///		Contenido del elemento de la lista
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		///		Lista asociada al elemento
		/// </summary>
		public ListReport List { get; set; }
	}
}
