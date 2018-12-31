using System;

using Bau.Libraries.LibReports.Renderer.Models.Base;

namespace Bau.Libraries.LibReports.Renderer.Models.Contents
{
	/// <summary>
	///		Imagen asociada al informe
	/// </summary>
	public class ImageReport : ContentReportBase
	{
		public ImageReport(ContentReportBase parent) : base(parent) { }

		/// <summary>
		///		Clona los datos de una imagen
		/// </summary>
		public ImageReport Clone(ContentReportBase parent)
		{
			ImageReport image = new ImageReport(parent);

				// Clona las propiedades comunes
				base.CloneToTarget(image);
				// Clona el resto de propiedades
				image.Key = Key;
				image.FileName = FileName;
				image.Top = Top.Clone();
				image.Left = Left.Clone();
				image.Width = Width.Clone();
				image.Height = Height.Clone();
				// Devuelve la imagen
				return image;
		}

		/// <summary>
		///		Clave del archivo de imagen
		/// </summary>
		public string Key { get; set; }

		/// <summary>
		///		Nombre del archivo de imagen
		/// </summary>
		public string FileName { get; set; }

		/// <summary>
		///		Posición superior
		/// </summary>
		public Styles.UnitStyle Top { get; set; }

		/// <summary>
		///		Posición izquierda
		/// </summary>
		public Styles.UnitStyle Left { get; set; }

		/// <summary>
		///		Ancho
		/// </summary>
		public Styles.UnitStyle Width { get; set; }

		/// <summary>
		///		Alto
		/// </summary>
		public Styles.UnitStyle Height { get; set; }
	}
}
