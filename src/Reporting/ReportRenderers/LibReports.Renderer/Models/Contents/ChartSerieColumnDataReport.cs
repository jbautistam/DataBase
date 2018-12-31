using System;

namespace Bau.Libraries.LibReports.Renderer.Models.Contents
{
	/// <summary>
	///		Datos de la columna de un informe
	/// </summary>
	public class ChartSerieColumnDataReport : Base.ContentReportBase
	{
		public ChartSerieColumnDataReport(Base.ContentReportBase parent) : base(parent) { }

		/// <summary>
		///		Clona la definición de una columna
		/// </summary>
		internal ChartSerieColumnDataReport CloneDefinition(ChartSerieColumnReport column)
		{
			ChartSerieColumnDataReport data = new ChartSerieColumnDataReport(column);

				// Clona los datos básicos
				base.CloneToTarget(data);
				// Asigna las propiedades
				data.Value = Value;
				data.Minimum = Minimum;
				data.Maximum = Maximum;
				// Devuelve el objeto clonado
				return data;
		}

		/// <summary>
		///		Valor
		/// </summary>
		public double Value { get; set; }

		/// <summary>
		///		Valor mínimo
		/// </summary>
		public double Minimum { get; set; }

		/// <summary>
		///		Valor máximo
		/// </summary>
		public double Maximum { get; set; }
	}
}
