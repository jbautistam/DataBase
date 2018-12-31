using System;

namespace Bau.Libraries.LibReports.Renderer.Models.Contents
{
	/// <summary>
	///		Colección de <see cref="ChartSerieColumnReport"/>
	/// </summary>
	public class ChartSerieColumnReportCollection : Base.ClassBaseCollection<ChartSerieColumnReport>
	{
		/// <summary>
		///		Añde un dato a la colección
		/// </summary>
		internal void Add(ChartSerieReport serie, string title, string subTitle)
		{
			ChartSerieColumnReport data = new ChartSerieColumnReport(serie);

				// Asigna las propiedades
				data.Title = title;
				data.SubTitle = subTitle;
				// Añade el objeto
				Add(data);
		}
	}
}
