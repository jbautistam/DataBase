using System;

namespace Bau.Libraries.LibDbProviders.Odbc
{
	/// <summary>
	///		Cadena de conexión de OleDB
	/// </summary>
	public class OdbcConnectionString : Base.DbConnectionStringBase
	{ 
		public OdbcConnectionString(string connectionString, int timeOut = 15) : base(connectionString, timeOut) {}
	}
}