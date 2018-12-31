using System;

namespace Bau.Libraries.LibDbProviders.Base.Providers.ODBC
{
	/// <summary>
	///		Cadena de conexión de OleDB
	/// </summary>
	public class OdbcConnectionString : DBConnectionStringBase
	{ 
		public OdbcConnectionString(string connectionString, int timeOut = 15) : base(connectionString, timeOut) {}
	}
}