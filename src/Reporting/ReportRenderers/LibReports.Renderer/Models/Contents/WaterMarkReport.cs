using System;

using Bau.Libraries.LibReports.Renderer.Models.Base;

namespace Bau.Libraries.LibReports.Renderer.Models.Contents
{
	/// <summary>
	///		Marca de agua para la página
	/// </summary>
	public class WaterMarkReport : ContentReportBase
	{ 
		// Enumerados públicos
		/// <summary>
		///		Modo de la marca de agua
		/// </summary>
		public enum ModeWaterMark
		{
			/// <summary>Inicio de la marca de agua</summary>
			Start,
			/// <summary>Fin de la marca de agua</summary>
			End
		}

		public WaterMarkReport(ContentReportBase parent) : base(parent) { }

		/// <summary>
		///		Clona la definición de la sección
		/// </summary>
		public WaterMarkReport CloneDefinition(ContentReportBase parent)
		{
			WaterMarkReport waterMark = new WaterMarkReport(parent);

				// Clona los datos básicos
				base.CloneToTarget(waterMark);
				// Clona los datos de la marca de agua
				waterMark.BackgroundPage = BackgroundPage;
				waterMark.Mode = Mode;
				waterMark.ImagesMustFitPage = ImagesMustFitPage;
				// Clona las imágenes hija
				foreach (ContentReportBase content in Images)
					if (content is ImageReport)
						waterMark.Images.Add((content as ImageReport).Clone(parent));
					else
						throw new NotImplementedException("No se reconoce el tipo de contenido para una marca de agua");
				// Devuelve la sección clonada
				return waterMark;
		}

		/// <summary>
		///		Color de fondo de la página
		/// </summary>
		public System.Drawing.Color? BackgroundPage { get; set; }

		/// <summary>
		///		Modo de la marca de página
		/// </summary>
		public ModeWaterMark Mode { get; set; }

		/// <summary>
		///		Imágenes de la marca de agua
		/// </summary>
		public ClassBaseCollection<ImageReport> Images { get; } = new ClassBaseCollection<ImageReport>();

		/// <summary>
		///		Indica si las imágenes deben ajustarse a toda la página
		/// </summary>
		public bool ImagesMustFitPage { get; set; }
	}
}
