using System;

using Bau.Libraries.LibReports.Renderer.Models.Base;

namespace Bau.Libraries.LibReports.Renderer.Models.Contents
{
	/// <summary>
	///		Secciónn de cabecera / pie de un informe
	/// </summary>
	public class HeaderFootReport : ContentReportBase
	{ 
		// Enumerados públicos
	  /// <summary>
	  ///		Tipo de cabecera / pie
	  /// </summary>
		public enum HeaderFootType
		{
			/// <summary>Cabecera</summary>
			Header,
			/// <summary>Pie</summary>
			Foot
		}
		/// <summary>
		///		Páginas en que se imprime esta sección
		/// </summary>
		public enum PageTarget
		{
			/// <summary>Todas las páginas</summary>
			All,
			/// <summary>Páginas pares</summary>
			Even,
			/// <summary>Páginas impares</summary>
			Odd
		}

		public HeaderFootReport(ContentReportBase parent, HeaderFootType type) : base(parent)
		{
			Type = type;
			Content = new SectionReport(this);
		}

		/// <summary>
		///		Clona la cabecera /pie
		/// </summary>
		public HeaderFootReport Clone()
		{
			HeaderFootReport header = new HeaderFootReport(null, Type);

				// Asigna los datos
				header.StartPage = StartPage;
				header.EndPage = EndPage;
				header.Target = Target;
				header.Content = Content.CloneDefinition(header);
				// Devuelve el objeto clonado
				return header;
		}

		/// <summary>
		///		Comprueba si la página está en el límite definido para la cabecera / pie
		/// </summary>
		public bool CheckPage(int pageIndex, HeaderFootType type)
		{
			return Type == type && pageIndex >= (StartPage ?? 0) && pageIndex <= (EndPage ?? pageIndex + 1) &&
						   (Target == PageTarget.All ||
							(Target == PageTarget.Even && pageIndex % 2 == 0) ||
							  (Target == PageTarget.Odd && pageIndex % 2 != 0));
		}

		/// <summary>
		///		Tipo de cabecera / pie
		/// </summary>
		public HeaderFootType Type { get; set; }

		/// <summary>
		///		Grupo al que se asocia la cabecera / pie
		/// </summary>
		public string Group { get; set; }

		/// <summary>
		///		Página de inicio para impresión de este tipo de cabecera / pie
		/// </summary>
		public int? StartPage { get; set; }

		/// <summary>
		///		Página de fin para impresión de este tipo de cabecera / pie
		/// </summary>
		public int? EndPage { get; set; }

		/// <summary>
		///		Páginas destino
		/// </summary>
		public PageTarget Target { get; set; }

		/// <summary>
		///		Contenido
		/// </summary>
		public SectionReport Content { get; set; }
	}
}
