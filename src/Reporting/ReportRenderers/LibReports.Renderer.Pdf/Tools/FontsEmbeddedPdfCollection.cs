using System;
using System.Collections.Generic;

using iTextSharp.text.pdf;
using Bau.Libraries.LibCommonHelper.Extensors;

namespace Bau.Libraries.LibReports.Renderer.PDF.Tools
{
	/// <summary>
	///		Colección de <see cref="FontEmbeddedPdf"/>
	/// </summary>
	internal class FontsEmbeddedPdfCollection : List<FontEmbeddedPdf>
	{
		internal FontsEmbeddedPdfCollection(Models.Base.Parameters.AdditionalFileCollection externalFonts)
		{
			if (externalFonts != null)
				foreach (Models.Base.Parameters.AdditionalFile objExternalFont in externalFonts)
					Add(new FontEmbeddedPdf { Name = objExternalFont.ID, FileName = objExternalFont.FileName });
		}

		/// <summary>
		///		Obtiene la fuente base
		/// </summary>
		internal BaseFont GetBaseFont(string fontName)
		{ 
			// Recorre las fuentes embebidas
			foreach (FontEmbeddedPdf font in this)
				if (font.Name.EqualsIgnoreCase(fontName))
					return font.Font;
			// Devuelve una fuente base
			return null;
		}
	}
}
