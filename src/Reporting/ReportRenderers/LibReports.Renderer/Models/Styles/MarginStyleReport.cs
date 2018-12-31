using System;

namespace Bau.Libraries.LibReports.Renderer.Models.Styles
{
	/// <summary>
	///		Margen
	/// </summary>
	public class MarginStyleReport
	{
		/// <summary>
		///		Clona un margen
		/// </summary>
		public MarginStyleReport Clone()
		{
			MarginStyleReport margin = new MarginStyleReport();

				// Asigna los márgenes
				margin.Left = Left;
				margin.Top = Top;
				margin.Right = Right;
				margin.Bottom = Bottom;
				// Devuelve el margen
				return margin;
		}

		/// <summary>
		///		Margen izquierdo
		/// </summary>
		public double? Left { get; set; }

		/// <summary>
		///		Margen superior
		/// </summary>
		public double? Top { get; set; }

		/// <summary>
		///		Margen derecho
		/// </summary>
		public double? Right { get; set; }

		/// <summary>
		///		Margen inferior
		/// </summary>
		public double? Bottom { get; set; }

		/// <summary>
		///		Indica si hay un margen visible
		/// </summary>
		public bool Visible
		{
			get { return Left != null || Top != null || Right != null || Bottom != null; }
		}
	}
}
