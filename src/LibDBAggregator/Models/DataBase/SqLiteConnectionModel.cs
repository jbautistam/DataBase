using System;
using System.Collections.Generic;
using System.Data;

using Bau.Libraries.LibSqLiteProvider;

namespace Bau.Libraries.LibDBAggregator.Models.DataBase
{
	/// <summary>
	///		Conexión a SqLite
	/// </summary>
	public class SqLiteConnectionModel : AbstractDataBaseConnectionModel
	{
		/// <summary>
		///		Obtiene una tabla de datos
		/// </summary>
		public override DataTable GetDataTable(string command, Dictionary<string, object> parameters)
		{
			using (SqLiteProvider connection = new SqLiteProvider(GetConnectionString()))
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
		private SqLiteConnectionString GetConnectionString()
		{
			return new SqLiteConnectionString(FileName, Password);
		}

		/// <summary>
		///		Nombre de archivo
		/// </summary>
		public string FileName { get; set; }

		/// <summary>
		///		Contraseña
		/// </summary>
		public string Password { get; set; }
	}
}
