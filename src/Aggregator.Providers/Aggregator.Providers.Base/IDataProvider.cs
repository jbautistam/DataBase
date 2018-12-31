using System;
using System.Data;
using System.Collections.Generic;

namespace Bau.Libraries.Aggregator.Providers.Base
{
	/// <summary>
	///		Interface para los proveedores de datos 
	/// </summary>
	public interface IDataProvider
	{
		/// <summary>
		///		Clave del proveedor de datos
		/// </summary>
		string Key { get; }

		/// <summary>
		///		Tipo de datos
		/// </summary>
		string Type { get; }

		/// <summary>
		///		Carga los datos
		/// </summary>
		IEnumerable<DataTable> LoadData(DataProviderCommand command, int rowsPerPage = 20_000);

		/// <summary>
		///		Carga una página de datos
		/// </summary>
		DataTable LoadData(DataProviderCommand command, int pageIndex, int pageSize, out long records);

		/// <summary>
		///		Carga el esquema
		/// </summary>
		LibDbProviders.Base.Schema.SchemaDbModel LoadSchema();

		/// <summary>
		///		Ejecuta un comando
		/// </summary>
		object Execute(DataProviderCommand command);

		/// <summary>
		///		Ejecuta una serie de comandos
		/// </summary>
		void Execute(List<DataProviderCommand> commands);
	}
}
