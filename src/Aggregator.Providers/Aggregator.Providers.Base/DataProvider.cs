using System;
using System.Data;
using System.Collections.Generic;

namespace Bau.Libraries.Aggregator.Providers.Base
{
	/// <summary>
	///		Base para los proveedores de datos
	/// </summary>
	public abstract class DataProvider : IDataProvider
	{
		protected DataProvider(string key, string type)
		{
			Key = key;
			Type = type;
		}

		/// <summary>
		///		Obtiene un valor de un comando
		/// </summary>
		protected string GetCommandValue(Dictionary<string, string> commandParameters, string key)
		{
			// Busca el comando
			foreach (KeyValuePair<string, string> command in commandParameters)
				if (command.Key.Equals(key, StringComparison.CurrentCultureIgnoreCase))
					return command.Value;
			// Si ha llegado hasta aquí es porque no ha encontrado nada
			return null;
		}

		/// <summary>
		///		Código del proveedor
		/// </summary>
		public string Key { get; }

		/// <summary>
		///		Tipo del proveedor de datos
		/// </summary>
		public string Type { get; }

		/// <summary>
		///		Carga una serie de datos
		/// </summary>
		public abstract IEnumerable<DataTable> LoadData(DataProviderCommand command, int rowsPerPage = 20_000);

		/// <summary>
		///		Carga una serie de datos paginados
		/// </summary>
		public abstract DataTable LoadData(DataProviderCommand command, int pageIndex, int pageSize, out long records);

		/// <summary>
		///		Carga el esquema
		/// </summary>
		public abstract LibDbProviders.Base.Schema.SchemaDbModel LoadSchema();

		/// <summary>
		///		Ejecuta un comando
		/// </summary>
		public abstract object Execute(DataProviderCommand command);

		/// <summary>
		///		Ejecuta una serie de comandos
		/// </summary>
		public abstract void Execute(List<DataProviderCommand> commands);
	}
}
