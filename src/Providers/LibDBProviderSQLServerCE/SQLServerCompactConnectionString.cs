using System;

using Bau.Libraries.LibDBProvidersBase;

namespace Bau.Libraries.LibDBProviderSQLServerCE
{
	/// <summary>
	///		Cadena de conexión de SQL Server Compact
	/// </summary>
	public class SQLServerCompactConnectionString : IConnectionString
	{ // Variables privadas
			private string strConnectionString = null;

		public SQLServerCompactConnectionString(string strFileName)
		{ FileName = strFileName;
		}

		/// <summary>
		///		Nombre de archivo
		/// </summary>
		public string FileName { get; set; }

		/// <summary>
		///		Cadena de conexión
		/// </summary>
		public string ConnectionString 
		{ get
				{ if (!string.IsNullOrEmpty(strConnectionString))
						return strConnectionString;
					else
						return "Data Source=" + FileName + ";Default Lock Timeout=20000";
				}
			set { strConnectionString = value; }
		}
	}
}