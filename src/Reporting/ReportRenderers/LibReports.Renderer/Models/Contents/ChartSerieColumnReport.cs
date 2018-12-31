using System;

namespace Bau.Libraries.LibReports.Renderer.Models.Contents
{
	/// <summary>
	///		Clase con un dato de una serie de un informe
	/// </summary>
	public class ChartSerieColumnReport : Base.ContentReportBase
	{
		public ChartSerieColumnReport(Base.ContentReportBase parent) : base(parent) { }

		/// <summary>
		///		Clona los datos de una columna
		/// </summary>
		internal ChartSerieColumnReport CloneDefinition(ChartSerieReport serie)
		{
			ChartSerieColumnReport column = new ChartSerieColumnReport(serie);

				// Clona los datos básicos
				base.CloneToTarget(column);
				// Asigna las propiedades
				column.Title = Title;
				column.SubTitle = SubTitle;
				// Clona las columnas
				foreach (ChartSerieColumnDataReport data in Data)
					column.Data.Add(data.CloneDefinition(column));
				// Devuelve el objeto clonado
				return column;
		}

		/// <summary>
		///		Definición a la que se asocia la columna
		/// </summary>
		public ChartSerieDataDefinition Definition { get; set; }

		/// <summary>
		///		Título del dato
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		///		Subtítulo del dato
		/// </summary>
		public string SubTitle { get; set; }

		/// <summary>
		///		Columnas del gráfico
		/// </summary>
		public ChartSerieColumnDataReportCollection Data { get; } = new ChartSerieColumnDataReportCollection();
	}
}
