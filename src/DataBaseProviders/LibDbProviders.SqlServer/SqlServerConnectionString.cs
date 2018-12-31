using System;

using Bau.Libraries.LibDbProviders.Base;

namespace Bau.Libraries.LibDbProviders.SqlServer
{
	/// <summary>
	///		Cadena de conexión de SQL Server
	/// </summary>
	public class SqlServerConnectionString : DbConnectionStringBase
	{ 
		// Enumerados públicos
		/// <summary>
		///		Tipo de conexión
		/// </summary>
		public enum ConnectionType
		{
			/// <summary>A un servidor</summary>
			Normal,
			/// <summary>Un archivo de base de datos</summary>
			File
		}
		// Variables privadas
		private string _connectionString;

		public SqlServerConnectionString() : base(null, 30) { }

		public SqlServerConnectionString(string connectionString, int timeOut = 30) : base(connectionString, timeOut) { }

		public SqlServerConnectionString(string server, string user, string password, string dataBase, bool integratedSecurity, int timeOut = 15) 
						: this(server, 0, user, password, dataBase, integratedSecurity, timeOut) {}

		public SqlServerConnectionString(string server, int port, string user, string password, string dataBase, bool integratedSecurity, int timeOut = 15) : base(null, timeOut)
		{
			Type = ConnectionType.Normal;
			Server = server;
			Port = port;
			User = user;
			Password = password;
			DataBase = dataBase;
			UseIntegratedSecurity = integratedSecurity;
		}

		public SqlServerConnectionString(string server, string dataBaseFile, int timeOut = 15) : base(null, timeOut)
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
		public int Port { get; set; }

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
		///		Cadena de conexión
		/// </summary>
		public override string ConnectionString 
		{
			get 
			{
				string GetServerAndPort(string server, int port)
				{
					if (port < 1)
						return server;
					else
						return $"{server},{port}";
				}

				if (!string.IsNullOrEmpty(_connectionString))
					return _connectionString;
				else
					switch (Type)
					{
						case ConnectionType.File:
							return $"Data Source={Server};AttachDbFilename=\"{DataBaseFile}\";Connect Timeout={TimeOut};User Instance=True;Integrated Security={UseIntegratedSecurity};";
						case ConnectionType.Normal:
							string connectionString = $"Data Source={GetServerAndPort(Server, Port)};Initial Catalog={DataBase};";

								// Añade datos de usuario
								if (UseIntegratedSecurity)
									connectionString += "Integrated Security=True";
								else
									connectionString += $"Persist Security Info=True;User ID={User};Password={Password}";
								// Devuelve la cadena de conexión
								return connectionString;
						default:
							throw new Base.DBExceptions.DbException("Tipo de conexión desconocida");
					}
			}
			set { _connectionString = value; }
		}
	}
}