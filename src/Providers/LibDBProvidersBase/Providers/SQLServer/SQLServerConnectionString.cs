using System;

namespace Bau.Libraries.LibDBProvidersBase.Providers.SQLServer
{
	/// <summary>
	///		Cadena de conexión de SQL Server
	/// </summary>
	public class SQLServerConnectionString : IConnectionString
	{ 
		// Enumerados públicos
		/// <summary>
		///		Tipo de conexión
		/// </summary>
		public enum ConnectionType
		{
			/// <summary>Normal</summary>
			Normal,
			/// <summary>Archivo</summary>
			File
		}
		// Variables privadas
		private string connectionString = null;

		public SQLServerConnectionString()
		{
			TimeOut = 30;
		}

		public SQLServerConnectionString(string connectionString)
		{
			ConnectionString = connectionString;
		}

		public SQLServerConnectionString(string server, string user, string password, string dataBase) : this()
		{
			Type = ConnectionType.Normal;
			Server = server;
			User = user;
			Password = password;
			DataBase = dataBase;
		}

		public SQLServerConnectionString(string server, string dataBaseFile)
		{
			Type = ConnectionType.File;
			Server = server;
			DataBaseFile = dataBaseFile;
		}

		/// <summary>
		///		Tipo de conexión
		/// </summary>
		public ConnectionType Type { get; set; }

		/// <summary>
		///		Servidor
		/// </summary>
		public string Server { get; set; }

		/// <summary>
		///		Puerto
		/// </summary>
		public int Port { get; set; } = 1433;

		/// <summary>
		///		Indica si se debe utilizar seguridad integrada
		/// </summary>
		public bool UseIntegratedSecurity { get; set; }

		/// <summary>
		///		Usuario
		/// </summary>
		public string User { get; set; }

		/// <summary>
		///		Contraseña
		/// </summary>
		public string Password { get; set; }

		/// <summary>
		///		Base de datos
		/// </summary>
		public string DataBase { get; set; }

		/// <summary>
		///		Archivo de base de datos
		/// </summary>
		public string DataBaseFile { get; set; }

		/// <summary>
		///		Tiempo de espera para la conexión
		/// </summary>
		public int TimeOut { get; set; }

		/// <summary>
		///		Cadena de conexión
		/// </summary>
		public string ConnectionString
		{
			get
			{
				if (!string.IsNullOrEmpty(connectionString))
					return connectionString;
				else
				{
					switch (Type)
					{
						case ConnectionType.File:
							return $"Data Source={Server};AttachDbFilename=\"{DataBaseFile}\";Connect Timeout={TimeOut};Integrated Security={UseIntegratedSecurity};";
						case ConnectionType.Normal:
							string serverCompound = Server;

								// Añade el puerto a la cadena
								if (Port != 1433 && Port > 0)
									serverCompound += "," + Port;
								// Añade los datos de servidor y base de datos
								// serverCompound = $"Server={serverCompound};DataBase={DataBase};Integrated Security={UseIntegratedSecurity};";
								serverCompound = $"Data Source={serverCompound};Initial Catalog={DataBase};Integrated Security={UseIntegratedSecurity};";
								// Añade usuario y contraseña
								if (!UseIntegratedSecurity)
									serverCompound += $"Uid={User};Pwd={Password};";
								// Devuelve la cadena de conexión
								return serverCompound;
						default:
							throw new DBExceptions.DBException("Tipo de conexión desconocida");
					}
				}
			}
			set { connectionString = value; }
		}
	}
}