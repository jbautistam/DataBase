using System;
using Bau.Libraries.LibReports.Renderer.Models.Base;

namespace Bau.Libraries.LibReports.Renderer.Models.Contents
{
	/// <summary>
	///		Gráfico de un informe
	/// </summary>
	public class ChartReport : Base.ContentReportBase
	{   
		// Enumerados públicos
		/// <summary>Tipo de gráfico</summary>
		public enum ChartType
		{
			/// <summary>Gráfico de área</summary>
			Area,
			/// <summary>Gráfico de barras verticales</summary>
			ColumnBar,
			/// <summary>Gráfico de barras horizontales</summary>
			HorizontalBar,
			/// <summary>Gráfico de burbujas</summary>
			Bubble,
			/// <summary>Gráfico combinado</summary>
			Combo,
			/// <summary>Gráfico de donut</summary>
			Donut,
			/// <summary>Gráfico de tarta</summary>
			Pie,
			/// <summary>Gráfico de líneas</summary>
			Line,
			/// <summary>Gráfico de splines</summary>
			Spline
		}

		public ChartReport(Base.ContentReportBase parent) : base(parent) { }

		/// <summary>
		///		Clona la definición del informe
		/// </summary>
		public ChartReport CloneDefinition(ContentReportBase parent)
		{
			ChartReport chart = new ChartReport(parent);

				// Clona los datos básicos
				base.CloneToTarget(chart);
				// Clona las propiedades
				chart.Title = Title;
				chart.SubTitle = SubTitle;
				chart.IDType = IDType;
				chart.Is3D = Is3D;
				chart.IsStacked = IsStacked;
				chart.WithLegend = WithLegend;
				chart.Width = Width;
				chart.Height = Height;
				// Clona las series
				foreach (ChartSerieReport serie in Series)
					chart.Series.Add(serie.CloneDefinition(chart));
				// Devuelve el gráfico clonado
				return chart;
		}

		/// <summary>
		///		Título del gráfico
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		///		Subtítulo del gráfico
		/// </summary>
		public string SubTitle { get; set; }

		/// <summary>
		///		Tipo de gráfico
		/// </summary>
		public ChartType IDType { get; set; }

		/// <summary>
		///		Indica si el gráfico es en 3D
		/// </summary>
		public bool Is3D { get; set; }

		/// <summary>
		///		Indica si el gráfico se muestra apilado
		/// </summary>
		public bool IsStacked { get; set; }

		/// <summary>
		///		Indica si el gráfico tiene o no leyenda
		/// </summary>
		public bool WithLegend { get; set; }

		/// <summary>
		///		Ancho del gráfico
		/// </summary>
		public int Width { get; set; }

		/// <summary>
		///		Alto del gráfico
		/// </summary>
		public int Height { get; set; }

		/// <summary>
		///		Series asociadas al gráfico
		/// </summary>
		public ChartSerieReportCollection Series { get; } = new ChartSerieReportCollection();
	}
}
