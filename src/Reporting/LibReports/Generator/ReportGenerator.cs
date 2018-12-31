using System;
using System.Collections.Generic;

using Bau.Libraries.Aggregator.Providers.Base;
using Bau.Libraries.LibMarkupLanguage;

namespace Bau.Libraries.LibReports.Generator
{
	/// <summary>
	///		Generador de informes
	/// </summary>
	internal class ReportGenerator
	{
		internal ReportGenerator(ReportManager manager, string pathBase, Dictionary<string, object> parameters)
		{
			Manager = manager;
			PathBase = pathBase;
			Parameters = parameters;
		}

		/// <summary>
		///		Genera un informe a partir de una cadena de texto XML
		/// </summary>
		public void Generate(string xml, string fileOutput)
		{
			MLFile fileML = LoadXml(xml);

				// Rellena el XML de salida con los datos y lo graba
				if (fileML != null)
				{
					MLFile fileTargetML = new ReportFiller(this).Convert(fileML);

						if (Manager.Errors.Count == 0)
							SaveXml(fileTargetML, fileOutput);
				}
		}

		/// <summary>
		///		Carga una cadena XML
		/// </summary>
		private MLFile LoadXml(string xml)
		{
			// Interpreta la cadena XML
			try
			{
				return new LibMarkupLanguage.Services.XML.XMLParser().ParseText(xml);
			}
			catch (Exception exception)
			{
				Manager.Errors.Add($"Error al convertir la cadena XML: {exception.Message}");
			}
			// Si ha llegado hasta aquí es porque ha habido algún error
			return null;
		}

		/// <summary>
		///		Escribe el XML
		/// </summary>
		private void SaveXml(MLFile fileML, string fileName)
		{
			new LibMarkupLanguage.Services.XML.XMLWriter().Save(fileName, fileML, out string error);
			if (!string.IsNullOrEmpty(error))
				Manager.Errors.Add($"Error al grabar el archivo intermedio {fileName}. {error}");
		}

		/// <summary>
		///		Manager de generación de informes
		/// </summary>
		internal ReportManager Manager { get; }

		/// <summary>
		///		Directorio base
		/// </summary>
		internal string PathBase { get; }

		/// <summary>
		///		Parámetros
		/// </summary>
		internal Dictionary<string, object> Parameters { get; }
	}
}
