using System;
using System.Linq;

namespace Bau.Libraries.LibReports.Renderer.Models.Styles
{
	/// <summary>
	///		Colección de <see cref="StyleReport"/>
	/// </summary>
	public class StylesReportCollection : Base.ClassBaseCollection<StyleReport>
	{
		/// <summary>
		///		Añade un estilo
		/// </summary>
		public StyleReport Add(string id, FontStyleReport font = null, System.Drawing.Color? backGround = null,
													 StyleReport.HorizontalAlignType horizontal = StyleReport.HorizontalAlignType.Unkown,
													 StyleReport.VerticalAlignType vertical = StyleReport.VerticalAlignType.Unknown, double? angle = null)
		{
			StyleReport style = new StyleReport(null, id);

				// Asigna las propiedades
				style.Font = font;
				style.BackGround = backGround;
				style.HorizontalAlign = horizontal;
				style.VerticalAlign = vertical;
				style.Angle = angle;
				// Añade el estilo
				Add(style);
				// Devuelve el estilo que se acaba de añadir
				return style;
		}

		/// <summary>
		///		Busca el estilo predeterminado
		/// </summary>
		public StyleReport SearchDefault()
		{
			StyleReport style = this.FirstOrDefault(item => item.IsDefault);

				// Si no se ha encontrado ningún estilo, se devuelve el primero encontrado
				if (style == null)
				{
					if (Count > 0)
						style = this[0];
					else
						style = new StyleReport(null, Guid.NewGuid().ToString());
				}
				// Devuelve el estilo predeterminado
				return style;
		}

		/// <summary>
		///		Clona la colección de estilos
		/// </summary>
		public StylesReportCollection Clone()
		{
			StylesReportCollection styles = new StylesReportCollection();

				// Añade los estilos clonados
				foreach (StyleReport style in this)
					styles.Add(style.Clone());
				// Devuelve la colección de estilos
				return styles;
		}
	}
}
