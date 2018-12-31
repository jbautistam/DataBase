using System;

namespace Bau.Libraries.LibDbProviders.Base 
{
	/// <summary>
	///		Clase que implementa la base de una cadena de conexión
	/// </summary>
	public abstract class DbConnectionStringBase : IConnectionString
	{
		protected DbConnectionStringBase(string connectionString, int timeOut = 15)
		{ 
			ConnectionString = connectionString;
			TimeOut = timeOut;
		}

		/// <summary>
		///		Cadena de conexión
		/// </summary>
		public virtual string ConnectionString { get; set; }

		/// <summary>
		///		Tiempo de espera
		/// </summary>
		public int TimeOut { get; set; }
	}
}
