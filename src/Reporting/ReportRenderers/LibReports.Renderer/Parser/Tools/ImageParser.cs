using System;

using Bau.Libraries.LibCommonHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibReports.Renderer.Models.Contents;

namespace Bau.Libraries.LibReports.Renderer.Parser.Tools
{
	/// <summary>
	///		Intérprete del XML de <see cref="ImageReport"/>
	/// </summary>
	internal class ImageParser : ParserBase<ImageReport>
	{
		internal ImageParser(ReportParser reportParser) : base(reportParser) { }

		/// <summary>
		///		Interpreta una imagen
		/// </summary>
		protected override ImageReport ParseInner(MLNode nodeML)
		{
			ImageReport image = new ImageReport(Parent);

				// Asigna los datos de la imagen
				if (!nodeML.Attributes["IdImage"].Value.IsEmpty())
					image.Key = nodeML.Attributes["IdImage"].Value;
				else
					image.FileName = nodeML.Attributes["FileName"].Value;
				// Asigna el ancho y alto
				image.Top = ParserHelper.GetUnit(nodeML.Attributes["Top"].Value);
				image.Left = ParserHelper.GetUnit(nodeML.Attributes["Left"].Value);
				image.Width = ParserHelper.GetUnit(nodeML.Attributes["Width"].Value);
				image.Height = ParserHelper.GetUnit(nodeML.Attributes["Height"].Value);
				// Devuelve la imagen
				return image;
		}
	}
}
