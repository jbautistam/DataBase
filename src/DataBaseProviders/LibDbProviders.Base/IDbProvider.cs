using System;
using System.Data;

using Bau.Libraries.LibDbProviders.Base.Parameters;

namespace Bau.Libraries.LibDbProviders.Base
{
	/// <summary>
	///		Interface para los proveedores de datos
	/// </summary>
	public interface IDbProvider : IDisposable
	{
		/// <summary>
		///		Abre la conexión
		/// </summary>
		void Open();

		/// <summary>
		///		Cierra la conexión
		/// </summary>
		void Close();

		/// <summary>
		///		Ejecuta una sentencia o un procedimiento sobre la base de datos
		/// </summary>
		/// <returns>
		/// Devuelve el número de registros procesados o -1 si se ha ejecutado un Rollback o la cadena SQL es una
		///	instrucción DML
		///	</returns>
		int Execute(string sql, ParametersDbCollection parameters, CommandType commandType);

		/// <summary>
		///		Obtiene un DataReader
		/// </summary>
		IDataReader ExecuteReader(string sql, ParametersDbCollection parametersDB, CommandType commandType);

		/// <summary>
		///		Obtiene un IDataReader a partir de un nombre de una sentencia o procedimiento y sus parámetros paginando
		///	en el servidor
		/// </summary>
		IDataReader ExecuteReader(string sql, ParametersDbCollection parameters, CommandType commandType, int pageNumber, int pageSize);

		/// <summary>
		///		Ejecuta una sentencia o procedimiento sobre la base de datos y devuelve un escalar
		/// </summary>
		object ExecuteScalar(string sql, ParametersDbCollection parameters, CommandType commandType);

		/// <summary>
		///		Obtiene un dataTable a partir de un nombre de una sentencia o procedimiento y sus parámetros
		/// </summary>
		DataTable GetDataTable(string sql, ParametersDbCollection parameters, CommandType commandType);

		/// <summary>
		///		Obtiene un dataTable a partir de un nombre de una sentencia o procedimiento y sus parámetros
		/// </summary>
		DataTable GetDataTable(string sql, ParametersDbCollection parameters, CommandType commandType, int pageNumber, int pageSize);

		/// <summary>
		///		Obtiene el número de registros resultantes de una consulta SQL
		/// </summary>
		long? GetRecordsCount(string sql, ParametersDbCollection parametersDB);

		/// <summary>
		///		Inicia una transacción
		/// </summary>
		void BeginTransaction();

		/// <summary>
		///		Confirma una transacción
		/// </summary>
		void Commit();

		/// <summary>
		///		Deshace una transacción
		/// </summary>
		void RollBack();

		/// <summary>
		///		Obtiene el esquema de la base de datos
		/// </summary>
		Schema.SchemaDbModel GetSchema();

		/// <summary>
		///		Parámetros de conexión
		/// </summary>
		IConnectionString ConnectionString { get; set; }

		/// <summary>
		///		Transacción actual
		/// </summary>
		IDbTransaction Transaction { get; }
	}
}