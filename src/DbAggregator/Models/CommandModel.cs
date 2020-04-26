using System;
using System.Collections.Generic;

namespace Bau.Libraries.DbAggregator.Models
{
	/// <summary>
	///		Comando para un proveedor de datos
	/// </summary>
	public class CommandModel
	{
		public CommandModel(string sql)
		{
			Sql = sql;
		}

		/// <summary>
		///		Obtiene la cadena de información del comando
		/// </summary>
		public string Debug()
		{
			if (string.IsNullOrWhiteSpace(Sql))
				return "The command SQL is empty";
			else
				return $"Sql: {Sql}";
		}

		/// <summary>
		///		Comandos
		/// </summary>
		public string Sql { get; }

		/// <summary>
		///		Parámetros
		/// </summary>
		public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
	}
}
