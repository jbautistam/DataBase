using System;

using Bau.Libraries.LibDbProviders.Base.Providers;

namespace Bau.Libraries.LibPostgreSqlProvider
{
	/// <summary>
	///		Cadena de conexión de PostgreSql
	/// </summary>
	public class PostgreSqlConnectionString : DBConnectionStringBase
	{ 
		// Variables privadas
		private string connectionString;

		public PostgreSqlConnectionString() : base(null, 30) { }

		public PostgreSqlConnectionString(string connectionString) : base(connectionString, 30) {}

		public PostgreSqlConnectionString(string server, string dataBase, int port, bool integratedSecurity, 
										  string user, string password = null, int timeOut = 15) : base(null, timeOut)
		{
			Server = server;
			DataBase = dataBase;
			Port = port;
			IntegratedSecurity = integratedSecurity;
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
		public int Port { get; set; } = 5432;

		/// <summary>
		///		Indica si se debe utilizar seguridad integrada
		/// </summary>
		public bool IntegratedSecurity { get; set; }

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
		/// <remarks>
		///		Ejemplo: Server=127.0.0.1;Port=5432;Database=myDataBase;User Id=myUsername;Password=myPassword;
		///		Ejemplo: Server=127.0.0.1;Port=5432;Database=myDataBase;Integrated Security=true;
		/// </remarks>
		public override string ConnectionString 
		{
			get 
			{
				if (!string.IsNullOrEmpty(connectionString))
					return connectionString;
				else 
				{ 
					string connection = $"Server={Server};Database={DataBase};";

						// Añade el puerto
						if (Port > 0)
							connection += $"port={Port};";
						// Añade las opciones de seguridad
						if (IntegratedSecurity)
							connection += "Integrated Security=true;";
						else
						{
							connection += $"User Id={User};";
							if (!string.IsNullOrEmpty(Password))
								connection += $"Password={Password};";
						}
						// Devuelve la cadena de conexión
						return connection;
				}
			}
			set { connectionString = value; }
		}
	}
}