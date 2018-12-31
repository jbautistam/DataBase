using System;

using Bau.Libraries.LibCommonHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibReports.Renderer.Models.Contents;

namespace Bau.Libraries.LibReports.Renderer.Parser.Tools
{
	/// <summary>
	///		Intérprete para los datos de una marca de agua a partir de XML
	/// </summary>
	internal class WaterMarkParser : ParserBase<WaterMarkReport>
	{
		internal WaterMarkParser(ReportParser reportParser) : base(reportParser) { }

		/// <summary>
		///		Interpreta los datos de una marca de agua
		/// </summary>
		protected override WaterMarkReport ParseInner(MLNode nodeML)
		{
			WaterMarkReport waterMark = new WaterMarkReport(Parent);

				// Interpreta los datos particulares de la marca de agua
				waterMark.BackgroundPage = ParserHelper.GetColor(nodeML.Attributes["Backcolor"].Value);
				waterMark.Mode = nodeML.Attributes["Mode"].Value.GetEnum(WaterMarkReport.ModeWaterMark.End);
				waterMark.ImagesMustFitPage = nodeML.Attributes["MustFitPage"].Value.GetBool(false);
				// Interpreta las imágenes hija	
				foreach (MLNode childML in nodeML.Nodes)
					switch (childML.Name)
					{
						case "Image":
								waterMark.Images.Add(new ImageParser(ReportParser).Parse(waterMark, childML));
							break;
						default:
							throw new NotImplementedException("Nodo desconocido - Nodo: " + childML.Name);
					}
				// Devuelve la marca de agua
				return waterMark;
		}
	}
}
