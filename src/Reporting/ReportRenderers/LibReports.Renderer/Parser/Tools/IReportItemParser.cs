using System;

using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibReports.Renderer.Models.Base;

namespace Bau.Libraries.LibReports.Renderer.Parser.Tools
{
	/// <summary>
	///		Interface para los intérpretes de contenido
	/// </summary>
	public interface IReportItemParser
	{
		/// <summary>
		///		Interpreta el contenido de un nodo
		/// </summary>
		ClassBase Parse(ContentReportBase parent, MLNode nodeML);
	}
}
