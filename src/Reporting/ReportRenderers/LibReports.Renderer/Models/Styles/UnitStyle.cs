using System;

namespace Bau.Libraries.LibReports.Renderer.Models.Styles
{
	/// <summary>
	///		Definición de un ancho
	/// </summary>
	public class UnitStyle
	{
		/// <summary>
		///		Unidad para el ancho
		/// </summary>
		public enum UnitType
		{
			/// <summary>Desconocido. No se debería utilizar</summary>
			Unknown,
			/// <summary>Píxels</summary>
			Pixels,
			/// <summary>Porcentaje</summary>
			Percent
		}

		public UnitStyle() : this(0, UnitType.Unknown) { }

		public UnitStyle(double value, UnitType unit)
		{
			Value = value;
			Unit = unit;
		}

		/// <summary>
		///		Clona el ancho
		/// </summary>
		internal UnitStyle Clone()
		{
			return new UnitStyle(Value, Unit);
		}

		/// <summary>
		///		Ancho
		/// </summary>
		public double Value { get; set; }

		/// <summary>
		///		Unidad con la que se aplica el ancho
		/// </summary>
		public UnitType Unit { get; set; }

		/// <summary>
		///		Indica si se ha definido el ancho
		/// </summary>
		public bool IsDefined
		{
			get { return Value != 0 && Unit != UnitType.Unknown; }
		}
	}
}
