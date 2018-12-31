using System;
using System.Collections.Generic;
using System.Data;

namespace Bau.Libraries.Aggregator.Providers.Base
{
	/// <summary>
	///		Manager para los proveedores de datos
	/// </summary>
    public class DataProviderManager
    {
		/// <summary>
		///		Añade un proveedor
		/// </summary>
		public void Add(IDataProvider provider)
		{
			Providers.Add(provider);
		}

		/// <summary>
		///		Carga los datos
		/// </summary>
		public IEnumerable<DataTable> LoadData(string key, DataProviderCommand command, int rowsPerPage = 20_000)
		{
			foreach (DataTable table in GetProvider(key).LoadData(command, rowsPerPage))
				yield return table;
		}

		/// <summary>
		///		Ejecuta un comando
		/// </summary>
		public object Execute(string key, DataProviderCommand command)
		{
			return GetProvider(key).Execute(command);
		}

		/// <summary>
		///		Ejecuta una serie de comandos
		/// </summary>
		public void Execute(string key, List<DataProviderCommand> commands)
		{
			GetProvider(key).Execute(commands);
		}

		/// <summary>
		///		Carga el esquema
		/// </summary>
		public LibDbProviders.Base.Schema.SchemaDbModel LoadSchema(string key)
		{
			return GetProvider(key).LoadSchema();
		}

		/// <summary>
		///		Obtiene el proveedor
		/// </summary>
		private IDataProvider GetProvider(string key)
		{
			IDataProvider provider = Providers[key];

				if (provider == null)
					throw new Exceptions.DataProviderException($"Can't find the provider {key}");
				else
					return provider;
		}

		/// <summary>
		///		Colección de proveedores
		/// </summary>
		private DataProviderCollection Providers { get; } = new DataProviderCollection();
    }
}
