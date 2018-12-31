using System;

using Bau.Libraries.LibCommonHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibMarkupLanguage.Services.XML;
using Bau.Libraries.LibReports.Renderer.Models;
using Bau.Libraries.LibReports.Renderer.Models.Contents;

namespace Bau.Libraries.LibReports.Renderer.Parser
{
	/// <summary>
	///		Intérprete de un archivo
	/// </summary>
	public class ReportParser
	{
		public ReportParser(string pathBase)
		{
			PathBase = pathBase;
		}

		/// <summary>
		///		Interpreta la definición de un archivo leyendo de un archivo de texto
		/// </summary>
		public Report ParseFile(string fileName)
		{
			return Parse(new XMLParser().ParseText(LibCommonHelper.Files.HelperFiles.LoadTextFile(fileName)));
		}

		/// <summary>
		///		Interpreta la definición de un archivo a partir de una cadena XML
		/// </summary>
		public Report ParseXML(string strXML)
		{
			return Parse(new XMLParser().ParseText(strXML));
		}

		/// <summary>
		///		Interpreta los nodos XML de un archivo
		/// </summary>
		private Report Parse(MLFile fileML)
		{ // Genera un nuevo informe
			Report = new Report();
			ParserLocator = new ParserLocator(this);
			// Recorre los datos
			foreach (MLNode nodeML in fileML.Nodes)
				if (nodeML.Name == "Report")
				{ // Interpreta el tipo de página
					Report.PageType = ParsePageType(nodeML.Attributes["PageType"].Value);
					// Interpreta la orientación
					Report.Landscape = nodeML.Attributes["Landscape"].Value.GetBool();
					// Interpreta los márgenes
					Report.Margin.Top = nodeML.Attributes["MarginTop"].Value.GetDouble() ?? Report.Margin.Top;
					Report.Margin.Right = nodeML.Attributes["MarginRight"].Value.GetDouble() ?? Report.Margin.Right;
					Report.Margin.Bottom = nodeML.Attributes["MarginBottom"].Value.GetDouble() ?? Report.Margin.Bottom;
					Report.Margin.Left = nodeML.Attributes["MarginLeft"].Value.GetDouble() ?? Report.Margin.Left;
					// Carga los nodos
					foreach (MLNode childML in nodeML.Nodes)
						switch (childML.Name)
						{
							case "Style":
								Report.Styles.Add(ParserLocator.Parse<Models.Styles.StyleReport>(null, childML));
								break;
							case "Header":
								Report.Header = ParserLocator.Parse<HeaderFootReport>(null, childML);
								Report.HeadersAndFoots.Add(ParserLocator.Parse<HeaderFootReport>(null, childML));
								break;
							case "Body":
								Report.Body = ParserLocator.Parse<SectionReport>(null, childML);
								break;
							case "Foot":
								Report.Foot = ParserLocator.Parse<HeaderFootReport>(null, childML);
								Report.HeadersAndFoots.Add(ParserLocator.Parse<HeaderFootReport>(null, childML));
								break;
						}
				}
			// Devuelve la definición del informe
			return Report;
		}

		/// <summary>
		///		Interpreta el tipo de página
		/// </summary>
		private Report.ReportPageType ParsePageType(string strPageType)
		{ // Interpreta el tipo de página a partir del valor del XML
			foreach (string typeName in Enum.GetNames(typeof(Report.ReportPageType)))
				if (strPageType.EqualsIgnoreCase(typeName))
					return (Report.ReportPageType) Enum.Parse(typeof(Report.ReportPageType), typeName);
			// Si no ha encontrado nada, devuelve el tipo predeterminado
			return Report.ReportPageType.A4;
		}

		/// <summary>
		///		Convierte un nombre de archivo. Si no existe, lo considera un directorio relativo al origen del informe
		/// </summary>
		private string ConvertFileName(string fileName)
		{
			string fileTarget = fileName;

			// Si no existe el nombre de archivo, puede que sea un directorio relativo
			if (!fileName.IsEmpty() && !PathBase.IsEmpty() && !System.IO.File.Exists(fileName))
			{ // Obtiene el nombre de archivo combinado con el directorio del informe
				fileTarget = System.IO.Path.Combine(PathBase, fileName);
				// Si tampoco existe el nombre de archivo combinado, recupera el original
				if (!System.IO.File.Exists(fileTarget))
					fileTarget = fileName;
			}
			// Devuelve el nombre de archivo
			return fileTarget;
		}

		/// <summary>
		///		Directorio base
		/// </summary>
		internal string PathBase { get; private set; }

		/// <summary>
		///		Informe que se está generando
		/// </summary>
		internal Report Report { get; private set; }

		/// <summary>
		///		Localizador de intérpretes de secciones de un informe
		/// </summary>
		internal ParserLocator ParserLocator { get; private set; }
	}
}