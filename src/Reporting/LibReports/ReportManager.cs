using System;
using System.Collections.Generic;

using Bau.Libraries.LibReports.Generator;
using Bau.Libraries.Aggregator.Providers.Base;

namespace Bau.Libraries.LibReports
{
	/// <summary>
	///		Generador de informes
	/// </summary>
	public class ReportManager
	{
		public ReportManager(DataProviderCollection dataProviders)
		{
			DataProviders = dataProviders;
		}

		/// <summary>
		///		Genera un informe
		/// </summary>
		public bool GenerateByFile(string fileName, string fileTarget, Dictionary<string, object> parameters)
		{
			return GenerateByXml(System.IO.Path.GetDirectoryName(fileName),
								 LibCommonHelper.Files.HelperFiles.LoadTextFile(fileName), fileTarget, parameters);
		}

		/// <summary>
		///		Genera un informe a partir de una cadena de texto XML
		/// </summary>
		public bool GenerateByXml(string pathBase, string xml, string fileTarget, Dictionary<string, object> parameters)
		{
			ReportGenerator generator = new ReportGenerator(this, pathBase, parameters);

				// Genera el archivo XML con los datos leidos de los proveedores
				generator.Generate(xml, fileTarget);
				// Devuelve el valor que indica si se ha generado
				return Errors.Count == 0;
		}

		/// <summary>
		///		Render de archivos
		/// </summary>
		public bool Render(string fileSource, string fileTarget, Renderer.IReportRenderer renderer)
		{
			// Representa el archivo
			if (renderer != null)
				renderer.Render(fileTarget, fileSource);
			else
				LibCommonHelper.Files.HelperFiles.CopyFile(fileSource, fileTarget);
			// Devuelve el valor que indica si los datos son correctos
			return true;
		}

		/// <summary>
		///		Proveedores de datos
		/// </summary>
		public DataProviderCollection DataProviders { get; }

		/// <summary>
		///		Errores
		/// </summary>
		public List<string> Errors { get; } = new List<string>();
	}
}
