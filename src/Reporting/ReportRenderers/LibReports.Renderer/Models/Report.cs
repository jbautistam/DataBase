using System;

namespace Bau.Libraries.LibReports.Renderer.Models
{
	/// <summary>
	///		Datos y estructura del informe
	/// </summary>
	public class Report : Base.ClassBase
	{ 
		// Enumerados privados
		/// <summary>
		///		Tipo de página
		/// </summary>
		public enum ReportPageType
		{
			/// <summary>Desconocido. No se debería utilizar</summary>
			Unknown,
			/// <summary>Página en A4</summary>
			A4,
			/// <summary>Página Letter</summary>
			Letter,
			/// <summary>Definida por el usuario</summary>
			User
		}

		public Report()
		{
			PageType = ReportPageType.A4;
			Landscape = false;
			Parameters = new ReportInformation();
			Header = new Contents.HeaderFootReport(null, Contents.HeaderFootReport.HeaderFootType.Header);
			Body = new Contents.SectionReport(null);
			Foot = new Contents.HeaderFootReport(null, Contents.HeaderFootReport.HeaderFootType.Foot);
			HeadersAndFoots = new Contents.HeaderFootReportsCollection();
			Styles = new Styles.StylesReportCollection();
			Margin = new Styles.MarginStyleReport { Top = 36, Right = 36, Bottom = 36, Left = 36 };
		}

		/// <summary>
		///		Clona la definición del informe (márgenes y estilos)
		/// </summary>
		public Report CloneDefinition()
		{
			Report report = new Report();

				// Clona los márgenes
				report.Margin = Margin.Clone();
				// Clona las fuentes embebidas
				//report.ExternalFonts = ExternalFonts.Clone();
				// Asigna los estilos
				report.Styles = Styles.Clone();
				// Asigna los estilos
				report.Landscape = Landscape;
				// Devuelve el informe clonado
				return report;
		}

		/// <summary>
		///		Tipo de página
		/// </summary>
		public ReportPageType PageType { get; set; }

		/// <summary>
		///		Orientación
		/// </summary>
		public bool Landscape { get; set; }

		/// <summary>
		///		Parámetros del informe
		/// </summary>
		public ReportInformation Parameters { get; set; }

		/// <summary>
		///		Cabecera de la sección
		/// </summary>
		public Contents.HeaderFootReport Header { get; set; }

		/// <summary>
		///		Cuerpo de la sección
		/// </summary>
		public Contents.SectionReport Body { get; set; }

		/// <summary>
		///		Pie de la sección
		/// </summary>
		public Contents.HeaderFootReport Foot { get; set; }

		/// <summary>
		///		Cabeceras y pies del informe
		/// </summary>
		public Contents.HeaderFootReportsCollection HeadersAndFoots { get; set; }

		/// <summary>
		///		Estilos del informe
		/// </summary>
		public Styles.StylesReportCollection Styles { get; set; }

		/// <summary>
		///		Márgenes del informe
		/// </summary>
		public Styles.MarginStyleReport Margin { get; set; }
	}
}
