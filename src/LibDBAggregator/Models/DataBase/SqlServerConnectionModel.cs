using System;
using System.Collections.Generic;
using System.Data;

using Bau.Libraries.LibDBProvidersBase.Providers.SQLServer;

namespace Bau.Libraries.LibDBAggregator.Models.DataBase
{
	/// <summary>
	///		Conexión a SqlServer
	/// </summary>
	public class SqlServerConnectionModel : AbstractDataBaseConnectionModel
	{
		/// <summary>
		///		Obtiene una tabla de datos
		/// </summary>
		public override DataTable GetDataTable(string command, Dictionary<string, object> parameters)
		{
			using (SQLServerProvider connection = new SQLServerProvider(GetConnectionString()))
			{
				// Abre la conexión
				connection.Open();
				// Lee los datos
				return connection.GetDataTable(command, ConvertParameters(parameters), CommandType.Text);
			}
		}

		/// <summary>
		///		Obtiene la cadena de conexión
		/// </summary>
		private SQLServerConnectionString GetConnectionString()
		{
			return new SQLServerConnectionString(Server, User, Password, DataBase)
							{
								Port = Port,
								UseIntegratedSecurity = UseIntegratedSecurity
							};
		}

		/// <summary>
		///		Servidor
		/// </summary>
		public string Server { get; set; }

		/// <summary>
		///		Puerto
		/// </summary>
		public int Port { get; set; }

		/// <summary>
		///		Base de datos
		/// </summary>
		public string DataBase { get; set; }

		/// <summary>
		///		Usuario
		/// </summary>
		public string User { get; set; }

		/// <summary>
		///		Contraseña
		/// </summary>
		public string Password { get; set; }

		/// <summary>
		///		Indica si debe utilizar seguridad integrada
		/// </summary>
		public bool UseIntegratedSecurity { get; set; }
	}
}
