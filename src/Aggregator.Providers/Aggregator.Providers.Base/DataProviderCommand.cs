using System;
using System.Collections.Generic;

namespace Bau.Libraries.Aggregator.Providers.Base
{
	/// <summary>
	///		Comando para un proveedor de datos
	/// </summary>
	public class DataProviderCommand
	{
		/// <summary>
		///		Comandos
		/// </summary>
		public Dictionary<string, string> Sentences { get; } = new Dictionary<string, string>();

		/// <summary>
		///		Parámetros
		/// </summary>
		public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
	}
}
