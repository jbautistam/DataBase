using System;

using Bau.Libraries.LibDBProvidersBase;

namespace Bau.Libraries.LibDBProviderOracle
{
	/// <summary>
	///		Cadena de conexión de SQL Server Compact
	/// </summary>
	public class OracleConnectionString : IConnectionString
	{ 
		public OracleConnectionString(string strConnectionString)
		{ ConnectionString = strConnectionString;
		}

		/// <summary>
		///		Cadena de conexión
		/// </summary>
		public string ConnectionString { get; set; }
	}
}