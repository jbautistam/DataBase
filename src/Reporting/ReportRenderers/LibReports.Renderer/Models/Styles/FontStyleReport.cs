using System;

using Bau.Libraries.LibCommonHelper.Extensors;

namespace Bau.Libraries.LibReports.Renderer.Models.Styles
{
	/// <summary>
	///		Datos de la fuente para un estilo
	/// </summary>
	public class FontStyleReport
	{
		public FontStyleReport() : this(null, 0) { }

		public FontStyleReport(string fontName, int size, bool bold = false, bool italic = false,
							   bool underline = false, System.Drawing.Color? color = null)
		{
			Name = fontName;
			Size = size;
			Bold = bold;
			Italic = italic;
			Underline = underline;
			Color = color;
		}

		/// <summary>
		///		Clona la fuente
		/// </summary>
		public FontStyleReport Clone()
		{
			FontStyleReport font = new FontStyleReport();

				// Asigna las propiedades
				font.Name = Name;
				font.Bold = Bold;
				font.Italic = Italic;
				font.Underline = Underline;
				font.Size = Size;
				font.Color = Color;
				// Devuelve el estilo de fuente
				return font;
		}

		/// <summary>
		///		Nommbre de la fuente
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///		Indica si el texto está en negrita
		/// </summary>
		public bool Bold { get; set; }

		/// <summary>
		///		Indica si el texto está en cursiva
		/// </summary>
		public bool Italic { get; set; }

		/// <summary>
		///		Indica si el texto está subrayado
		/// </summary>
		public bool Underline { get; set; }

		/// <summary>
		///		Tamaño del texto
		/// </summary>
		public int Size { get; set; }

		/// <summary>
		///		Color del texto
		/// </summary>
		public System.Drawing.Color? Color { get; set; }

		/// <summary>
		///		Comprueba si se ha definido la fuente
		/// </summary>
		public bool IsDefined
		{
			get { return !Name.IsEmpty() || Bold || Italic || Underline || Size > 0 || Color != null; }
		}
	}
}
