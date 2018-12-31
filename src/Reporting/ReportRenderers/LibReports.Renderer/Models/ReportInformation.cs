using System;

namespace Bau.Libraries.LibReports.Renderer.Models
{
	/// <summary>
	///		Información asociada al informe
	/// </summary>
	public class ReportInformation
	{
		/// <summary>
		///		Autor del informe
		/// </summary>
		public string Author { get; set; }

		/// <summary>
		///		Título del informe
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		///		Asunto del informe
		/// </summary>
		public string Subject { get; set; }

		/// <summary>
		///		Comentarios del informe
		/// </summary>
		public string Comments { get; set; }

		/// <summary>
		///		Palabras claves / tags
		/// </summary>
		public string Keywords { get; set; }
	}
}
