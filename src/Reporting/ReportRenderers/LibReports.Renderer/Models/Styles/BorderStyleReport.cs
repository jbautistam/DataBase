using System;

namespace Bau.Libraries.LibReports.Renderer.Models.Styles
{
	/// <summary>
	///		Estilo para un borde
	/// </summary>
	public class BorderStyleReport
	{ 
		/// <summary>
		///		Tipo de borde
		/// </summary>
		public enum BorderType
		{
			/// <summary>Desconocido (o invisible)</summary>
			Unknown,
			/// <summary>Borde sólido</summary>
			Solid,
			/// <summary>Borde con puntos</summary>
			Dot,
			/// <summary>Borde con líneas</summary>
			Dash,
			/// <summary>Borde con líneas y puntos</summary>
			DotDash,
			/// <summary>Borde doble</summary>
			Double
		}

		public BorderStyleReport()
		{
			Type = BorderType.Unknown;
		}

		/// <summary>
		///		Clona los datos del borde
		/// </summary>
		public BorderStyleReport Clone()
		{
			BorderStyleReport border = new BorderStyleReport();

				// Asigna las propiedades
				border.Type = Type;
				border.Width = Width;
				border.Color = Color;
				// Devuelve el objeto clonado
				return border;
		}

		/// <summary>
		///		Tipo de borde
		/// </summary>
		public BorderType Type { get; set; }

		/// <summary>
		///		Ancho
		/// </summary>
		public double Width { get; set; }

		/// <summary>
		///		Color
		/// </summary>
		public System.Drawing.Color? Color { get; set; }

		/// <summary>
		///		Indica si el borde es visible
		/// </summary>
		public bool Visible
		{
			get { return Type != BorderType.Unknown || Width > 0; }
		}
	}
}
