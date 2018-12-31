using System;

using Bau.Libraries.LibCommonHelper.Extensors;

namespace Bau.Libraries.LibReports.Renderer.Parser.Tools
{
	/// <summary>
	///		Helper para la interpretación
	/// </summary>
	internal class ParserBaseHelper
	{
		/// <summary>
		///		Normaliza un texto
		/// </summary>
		internal string Normalize(string text)
		{ 
			// Normaliza un texto quitándole saltos,espacios dobles
			if (!text.IsEmpty())
			{ 
				// Reemplaza los caracteres extraños
				text = text.Replace(Environment.NewLine, " ");
				text = text.Replace("\r\n", " ");
				text = text.Replace('\r', ' ');
				text = text.Replace('\n', ' ');
				text = text.Replace('\t', ' ');
				// Quita los espacios dobles
				while (!text.IsEmpty() && text.IndexOf("  ") >= 0)
					text = text.Replace("  ", " ");
			}
			// Devuelve el texto normalizado
			return text;
		}

		/// <summary>
		///		Obtiene un color a partir de una cadena
		/// </summary>
		internal System.Drawing.Color? GetColor(string color)
		{ 
			// Obtiene el color
			if (!string.IsNullOrEmpty(color))
				try
				{
					if (color.StartsWith("#"))
						return System.Drawing.Color.FromArgb(Convert.ToInt32(color.Substring(1), 16));
					else
						return System.Drawing.Color.FromName(color);
				}
				catch { }
			// Si ha llegado hasta aquí es porque no ha conseguido ningún color
			return null;
		}

		/// <summary>
		///		Asigna el ancho de una celda
		/// </summary>
		internal Models.Styles.UnitStyle GetUnit(string unit)
		{
			Models.Styles.UnitStyle unitStyle = new Models.Styles.UnitStyle(0, Models.Styles.UnitStyle.UnitType.Pixels);

				// Interpreta la cadena de la unidad
				if (!string.IsNullOrEmpty(unit))
				{
					int width;

						if (unit.EndsWith("px", StringComparison.CurrentCultureIgnoreCase) && unit.Length > 2)
						{
							if (int.TryParse(unit.Substring(0, unit.Length - 2), out width))
								unitStyle = new Models.Styles.UnitStyle(width, Models.Styles.UnitStyle.UnitType.Pixels);
						}
						else if (unit.EndsWith("%") && unit.Length > 1)
						{
							if (int.TryParse(unit.Substring(0, unit.Length - 1), out width))
								unitStyle = new Models.Styles.UnitStyle(width, Models.Styles.UnitStyle.UnitType.Percent);
						}
						else if (int.TryParse(unit, out width))
							unitStyle = new Models.Styles.UnitStyle(width, Models.Styles.UnitStyle.UnitType.Pixels);
				}
				// Devuelve la unidad
				return unitStyle;
		}
	}
}
