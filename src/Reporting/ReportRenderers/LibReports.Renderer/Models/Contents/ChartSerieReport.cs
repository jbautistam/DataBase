using System;

namespace Bau.Libraries.LibReports.Renderer.Models.Contents
{
	/// <summary>
	///		Serie de datos de un informe
	/// </summary>
	public class ChartSerieReport : Base.ContentCommandReportBase
	{
		public ChartSerieReport(Base.ContentReportBase parent) : base(parent) { }

		/// <summary>
		///		Clona la definición de la serie
		/// </summary>
		internal ChartSerieReport CloneDefinition(ChartReport chart)
		{
			ChartSerieReport serie = new ChartSerieReport(chart);

				// Clona la definición
				base.CloneToTarget(serie);
				//serie.Command = Command;
				serie.Code = Code;
				// Clona los datos básicos
				serie.Title = Title;
				serie.SubTitle = SubTitle;
				serie.IDType = IDType;
				serie.FieldColumnTitle = FieldColumnTitle;
				// Clona los datos
				foreach (ChartSerieColumnReport column in Columns)
					serie.Columns.Add(column.CloneDefinition(serie));
				// Clona las definiciones
				foreach (ChartSerieDataDefinition definition in DataDefinitions)
					serie.DataDefinitions.Add(definition.CloneDefinition());
				// Devuelve la serie
				return serie;
		}

		/// <summary>
		///		Título de la serie (por ejemplo: Año)
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		///		Subtítulo de la serie
		/// </summary>
		public string SubTitle { get; set; }

		/// <summary>
		///		Tipo del gráfico
		/// </summary>
		public ChartReport.ChartType IDType { get; set; }

		/// <summary>
		///		Campo del que se recoge el código de la serie
		/// </summary>
		public string FieldColumnTitle { get; set; }

		/// <summary>
		///		Código asociado a la serie
		/// </summary>
		public CodeReport Code { get; set; }

		/// <summary>
		///		Definiciones de los datos de la serie
		/// </summary>
		public ChartSerieDataDefinitionCollection DataDefinitions { get; } = new ChartSerieDataDefinitionCollection();

		/// <summary>
		///		Columnas asociadas a la serie
		/// </summary>
		public ChartSerieColumnReportCollection Columns { get; } = new ChartSerieColumnReportCollection();
	}
}
