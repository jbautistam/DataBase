using System;

namespace Bau.Libraries.LibReports.Renderer.Models.Contents
{
	/// <summary>
	///		Colección de <see cref="ContentReportBase"/>
	/// </summary>
	public class HeaderFootReportsCollection : Base.ClassBaseCollection<HeaderFootReport>
	{
		/// <summary>
		///		Busca la cabecera o el pie más apropiado para una página
		/// </summary>
		public HeaderFootReport Search(int pageIndex, HeaderFootReport.HeaderFootType type)
		{
			HeaderFootReport result = null;

				// Comprueba la cabecera / pie que se corresponde con la página
				foreach (HeaderFootReport headerFoot in this)
					if (headerFoot.CheckPage(pageIndex, type))
						if (result == null || (headerFoot.StartPage ?? 0) >= (result.StartPage ?? 0))
							result = headerFoot;
				// Devuelve la cabecera
				return result;
		}
	}
}
