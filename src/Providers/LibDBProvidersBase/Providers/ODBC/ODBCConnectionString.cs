using System;

namespace Bau.Libraries.LibDBProvidersBase.Providers.ODBC
{
	/// <summary>
	///		Cadena de conexión de OleDB
	/// </summary>
	public class ODBCConnectionString : IConnectionString
	{
		public ODBCConnectionString(string connectionString)
		{
			ConnectionString = connectionString;
		}

		/// <summary>
		///		Cadena de conexión
		/// </summary>
		public string ConnectionString { get; set; }
	}
}