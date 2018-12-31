using System;

using Bau.Libraries.LibReports.Renderer.Models.Base;

namespace Bau.Libraries.LibReports.Renderer.Models.Contents
{
	/// <summary>
	///		Colección de <see cref="ListItemReport"/>
	/// </summary>
	public class ListItemsReportCollection : Base.ClassBaseCollection<ListItemReport>
	{
		/// <summary>
		///		Añade un elemento a la lista
		/// </summary>
		public void Add(ContentReportBase parent, string text)
		{
			Add(new ListItemReport(parent) { Text = text });
		}
	}
}