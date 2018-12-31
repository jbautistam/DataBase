using System;
using System.Collections.Generic;

using Bau.Libraries.LibCommonHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibReports.Renderer.Models.Base;
using Bau.Libraries.LibReports.Renderer.Parser.Tools;

namespace Bau.Libraries.LibReports.Renderer.Parser
{
	/// <summary>
	///		Servicio de localización de parsers
	/// </summary>
	internal class ParserLocator
	{
		internal ParserLocator(ReportParser reportParser)
		{ ReportParser = reportParser;
		}

		/// <summary>
		///		Añade un intérprete de un elemento
		/// </summary>
		internal void Add(string tag, IReportItemParser parser)
		{ // Normaliza la clave
				tag = Normalize(tag);
			// Añade el intérprete
				if (!ParsersItem.ContainsKey(tag))
					ParsersItem.Add(tag, parser);
		}

		/// <summary>
		///		Normaliza una clave
		/// </summary>
		private string Normalize(string key)
		{ // Normaliza los elementos
				if (key.EqualsIgnoreCase("Body") || key.EqualsIgnoreCase("Section"))
					key = "Section";
				else if (key.EqualsIgnoreCase("HeaderSelector") || key.EqualsIgnoreCase("FootSelector"))
					key = "HeaderSelector";
				else if (key.EqualsIgnoreCase("Header") || key.EqualsIgnoreCase("Foot"))
					key = "Header";
			// Pasa la clave a mayúsculas
				key = key.ToUpper();
			// Devuelve la clave
				return key;
		}

		/// <summary>
		///		Obtiene el intérprete de una etiqueta
		/// </summary>
		internal IReportItemParser GetParserForTag(string tag)
		{ IReportItemParser parser;

				// Normaliza la clave
					tag = Normalize(tag);
				// Si no existe el intérprete, lo añade antes de hacer la búsqueda
					if (!ParsersItem.ContainsKey(tag))
						switch (tag)
							{ case "PARAGRAPH":
										Add(tag, new ParagraphParser(ReportParser));
									break;
								case "IMAGE":
										Add(tag, new ImageParser(ReportParser));
									break;
								case "SECTION":
										Add(tag, new SectionParser(ReportParser));
									break;
								case "TABLE":
										Add(tag, new TableParser(ReportParser));
									break;
								case "IF":
										Add(tag, new ConditionalSectionParser(ReportParser));
									break;
								case "LIST":
										Add(tag, new ListParser(ReportParser));
									break;
								case "HORIZONTALTABLE":
										Add(tag, new TableHorizontalParser(ReportParser));
									break;
								case "WATERMARK":
										Add(tag, new WaterMarkParser(ReportParser));
									break;
								case "HEADERSELECTOR":
										Add(tag, new HeaderFootSelectorParser(ReportParser));
									break;
								case "STYLE":
										Add(tag, new StyleParser(ReportParser));
									break;
								case "HEADER":
										Add(tag, new HeaderFootParser(ReportParser));
									break;
								case "CODE":
										Add(tag, new CodeParser(ReportParser));
									break;
								case "CHART":
										Add(tag, new ChartParser(ReportParser));
									break;
								case "LINK":
										Add(tag, new LinkParser(ReportParser));
									break;
							}
				// Obtiene el intérprete	
					if (ParsersItem.TryGetValue(Normalize(tag), out parser))
						return parser;
					else 
						throw new NotImplementedException("No se localiza ningún parser para la etiqueta " + tag);
		}

		/// <summary>
		///		Interpreta un elemento
		/// </summary>
		internal TypeData Parse<TypeData>(ContentReportBase parent, MLNode nodeML)
									where TypeData : ClassBase
		{ return GetParserForTag(nodeML.Name).Parse(parent, nodeML) as TypeData;
		}

		/// <summary>
		///		Intérprete del informe
		/// </summary>
		internal ReportParser ReportParser { get; private set; }

		/// <summary>
		///		Intérpretes de objetos definidos
		/// </summary>
		private Dictionary<string, IReportItemParser> ParsersItem { get; set; } = new Dictionary<string, IReportItemParser>();
	}
}
