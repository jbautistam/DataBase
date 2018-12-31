using System;

using Bau.Libraries.LibDbProviders.Base;

namespace Bau.Libraries.LibDbProviders.SqLite
{
	/// <summary>
	///		Cadena de conexión de SqLite
	/// </summary>
	public class SqLiteConnectionString : DbConnectionStringBase
	{ 
		// Enumerados públicos
		/// <summary>
		///		Modo de apretura de la base de datos
		/// </summary>
		public enum OpenMode
		{
			/// <summary>Indefinido</summary>
			Unknown,
			/// <summary>Abre la base de datos para lectura y escritura y la crea si no existe</summary>
			ReadWriteCreate,
			/// <summary>Abre la base de datos para lectura / escritura</summary>
			ReadWrite,
			/// <summary>Abre la base de datos en modo de sólo lectura</summary>
			ReadOnly,
			/// <summary>Abre una base de datos en memoria</summary>
			Memory
		}
		// Variables privadas
		private string _connectionString;

		public SqLiteConnectionString() : base(null, 30) { }

		public SqLiteConnectionString(string connectionString, int timeOut = 15) : base(connectionString, timeOut) {}

		public SqLiteConnectionString(string fileName, string password, OpenMode mode = OpenMode.ReadWriteCreate, int timeOut = 15) : base(null, timeOut)
		{
			FileName = fileName;
			Password = password;
			Mode = mode;
		}

		/// <summary>
		///		Nombre de archivo
		/// </summary>
		public string FileName { get; }

		/// <summary>
		///		Contraseña
		/// </summary>
		public string Password { get; }

		/// <summary>
		///		Modo de conexión
		/// </summary>
		public OpenMode Mode { get; }

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
					string connection = $"Data Source={FileName};";

						// Añade la contraseña
						if (!string.IsNullOrEmpty(Password))
							connection += $"Password={Password};";
						// Añade el modo
						if (Mode != OpenMode.Unknown)
							connection += $"Mode={Mode.ToString()};";
						// Devuelve la cadena de conexión
						return connection;
				}
			}
			set { _connectionString = value; }
		}
	}
}