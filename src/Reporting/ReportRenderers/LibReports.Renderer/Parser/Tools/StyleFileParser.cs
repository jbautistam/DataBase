using System;

using Bau.Libraries.LibCommonHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibMarkupLanguage.Services.XML;

namespace Bau.Libraries.LibReports.Renderer.Parser.Tools
{
	/// <summary>
	///		Parser de un archivo de estilos
	/// </summary>
	internal class StyleFileParser
	{
		/// <summary>
		///		Interpreta un archivo de estilos y los añade / modifica los estilos del informe
		/// </summary>
		internal void Parse(ReportParser reportParser, Models.Report report, string fileName)
		{
			if (System.IO.File.Exists(fileName))
			{
				StyleParser parser = new StyleParser(reportParser);
				MLFile fileML = new XMLParser().ParseText(LibCommonHelper.Files.HelperFiles.LoadTextFile(fileName));

					// Recorre los nodos añadiendo / modificando los estilos del informe
					foreach (MLNode nodeML in fileML.Nodes)
						if (nodeML.Name == "Styles")
							foreach (MLNode childML in nodeML.Nodes)
								if (childML.Name == "Style")
								{
									Models.Styles.StyleReport style = parser.ParseNode(null, childML);

									if (!style.ID.IsEmpty())
										report.Styles[style.ID] = style;
								}
			}
		}
	}
}
