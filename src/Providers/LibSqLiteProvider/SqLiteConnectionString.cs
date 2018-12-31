using System;

using Bau.Libraries.LibDBProvidersBase;

namespace Bau.Libraries.LibSqLiteProvider
{
	/// <summary>
	///		Cadena de conexión de SqLite
	/// </summary>
	public class SqLiteConnectionString : IConnectionString
	{ 
		// Variables privadas
		private string connectionString;

		public SqLiteConnectionString(string fileName, string password = null, int timeOut = 15)
		{
			FileName = fileName;
			Password = password;
		}

		/// <summary>
		///		Nombre de archivo
		/// </summary>
		public string FileName { get; set; }

		/// <summary>
		///		Contraseña
		/// </summary>
		public string Password { get; set; }

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
						string connection = $"Data Source={FileName};Version=3;Pooling=True;Max Pool Size=100;";

							// Añade la contraseña
							if (!string.IsNullOrEmpty(Password))
								connection += $"Password={Password};";
							// Devuelve la cadena de conexión
							return connection;
					}
			}
			set { connectionString = value; }
		}
	}
}