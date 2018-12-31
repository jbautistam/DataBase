using System;

using iTextSharp.text.pdf;

namespace Bau.Libraries.LibReports.Renderer.PDF.Tools
{
	/// <summary>
	///		Clase con los datos de una fuente embebida
	/// </summary>
	internal class FontEmbeddedPdf
	{ 
		// Variables privadas
		private BaseFont font = null;

		/// <summary>
		///		Nombre de la fuente
		/// </summary>
		internal string Name { get; set; }

		/// <summary>
		///		Nombre de archivo
		/// </summary>
		internal string FileName { get; set; }

		/// <summary>
		///		Fuente base
		/// </summary>
		internal BaseFont Font
		{
			get
			{ 
				// Obtiene la fuente
				if (font == null)
					if (System.IO.File.Exists(FileName))
						try // ... intenta crear la fuente cargándola de un archivo
						{
							font = BaseFont.CreateFont(FileName, BaseFont.WINANSI, BaseFont.EMBEDDED);
						}
						catch
						{
							font = null;
						}
					else
						font = null;
				// Devuelve la fuente
				return font;
			}
		}
	}
}
