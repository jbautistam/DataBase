using System;
using System.Data;

using Bau.Libraries.LibDBProvidersBase.Parameters;

namespace Bau.Libraries.LibDBProvidersBase
{
	/// <summary>
	///		Interface para los proveedores de datos
	/// </summary>
	public interface IDBProvider : IDisposable
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
		void Execute(string sql, ParametersDBCollection parameters, CommandType commandType);

		/// <summary>
		///		Obtiene un DataReader
		/// </summary>
		IDataReader ExecuteReader(string sql, ParametersDBCollection parametersDB, CommandType commandType);

		/// <summary>
		///		Ejecuta una sentencia o procedimiento sobre la base de datos y devuelve un escalar
		/// </summary>
		object ExecuteScalar(string sql, ParametersDBCollection parameters, CommandType commandType);

		/// <summary>
		///		Ejecuta un INSERT sobre la base de datos y obtiene el valor de identidad
		/// </summary>
		int? ExecuteGetIdentity(string text, ParametersDBCollection parametersDB, CommandType commandType);

		/// <summary>
		///		Obtiene un dataTable a partir de un nombre de una sentencia o procedimiento y sus parámetros
		/// </summary>
		DataTable GetDataTable(string sql, ParametersDBCollection parameters, CommandType commandType);

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
	}
}
