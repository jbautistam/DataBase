using System;

using Bau.Libraries.LibDbProviders.Base.Providers;

namespace Bau.Libraries.LibMySqlProvider
{
	/// <summary>
	///		Cadena de conexión de MySql
	/// </summary>
	public class MySqlConnectionString : DBConnectionStringBase
	{ 
		// Variables privadas
		private string _connectionString;

		public MySqlConnectionString() : base(null, 30) { }

		public MySqlConnectionString(string connectionString) : base(connectionString, 30) {}

		public MySqlConnectionString(string server, string dataBase, int port, string user, string password = null, int timeOut = 15) : base(null, timeOut)
		{
			Server = server;
			DataBase = dataBase;
			Port = port;
			User = user;
			Password = password;
		}

		/// <summary>
		///		Servidor
		/// </summary>
		public string Server { get; set; }

		/// <summary>
		///		Base de datos
		/// </summary>
		public string DataBase { get; set; }

		/// <summary>
		///		Puerto
		/// </summary>
		public int Port { get; set; } = 3306;

		/// <summary>
		///		Usuario
		/// </summary>
		public string User { get; set; }

		/// <summary>
		///		Contraseña
		/// </summary>
		public string Password { get; set; }

		/// <summary>
		///		Cadena de conexión
		/// </summary>
		public override string ConnectionString 
		{
			get 
			{
				if (!string.IsNullOrEmpty(_connectionString))
					return _connectionString;
				else 
				{ 
					string connection = $"server={Server};database={DataBase};port={Port};user={User};";

						// Añade la contraseña
						if (!string.IsNullOrEmpty(Password))
							connection += $"password={Password};";
						// Devuelve la cadena de conexión
						return connection;
				}
			}
			set { _connectionString = value; }
		}
	}
}