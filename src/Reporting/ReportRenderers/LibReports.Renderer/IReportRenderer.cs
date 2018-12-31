using System;

namespace Bau.Libraries.LibReports.Renderer
{
	/// <summary>
	///		Interface para los conversores de informes
	/// </summary>
	public interface IReportRenderer
	{
		/// <summary>
		///		Convierte el archivo XML en el final
		/// </summary>
		void Render(string fileTarget, string fileXml);
	}
}
