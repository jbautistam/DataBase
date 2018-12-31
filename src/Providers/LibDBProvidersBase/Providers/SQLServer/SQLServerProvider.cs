using System;
using System.Data;
using System.Data.SqlClient;

using Bau.Libraries.LibDBProvidersBase.Parameters;

namespace Bau.Libraries.LibDBProvidersBase.Providers.SQLServer
{
	/// <summary>
	///		Proveedor para SQL Server
	/// </summary>
	public class SQLServerProvider : DBProviderBase
	{
		public SQLServerProvider(SQLServerConnectionString connectionString) : this(new SqlConnection(connectionString.ConnectionString)) { }

		public SQLServerProvider(IDbConnection connection) : base(connection) { }

		/// <summary>
		///		Obtiene un comando
		/// </summary>
		protected override IDbCommand GetCommand(string text)
		{
			return new SqlCommand(text, Connection as SqlConnection);
		}

		/// <summary>
		///		Ejecuta un INSERT sobre la base de datos y obtiene el valor de identidad
		/// </summary>
		public override int? ExecuteGetIdentity(string text, ParametersDBCollection parametersDB, CommandType commandType = CommandType.Text)
		{
			int? identity = null;

				// Abre la conexión
				Open();
				// Ejecuta sobre la conexión
				switch (commandType)
				{
					case CommandType.Text:
							identity = (int?) ((decimal?) ExecuteScalar(NormalizeSqlInsert(text), parametersDB, commandType));
						break;
					case CommandType.StoredProcedure:
							Execute(text, parametersDB, commandType);
							identity = (int?) parametersDB["@return_code"].Value;
						break;
					default:
							Execute(text, parametersDB, commandType);
							identity = null;
						break;
				}
				// Cierra la conexión
				Close();
				// Devuelve el valor identidad
				return identity;
		}

		/// <summary>
		///		Normaliza una cadena SQL de inserción de datos
		/// </summary>
		private string NormalizeSqlInsert(string sqlInsert)
		{ 
			// Añade a la cadena de inserción una consulta para obtener el SCOPE_IDENTITY
			if (sqlInsert.IndexOf("Scope_Identity()", StringComparison.CurrentCultureIgnoreCase) < 0)
				sqlInsert += "; SELECT SCOPE_IDENTITY()";
			// Devuelve la cadena de inserción
			return sqlInsert;
		}

		/// <summary>
		///		Obtiene un dataTable
		/// </summary>
		protected override DataTable FillDataTable(IDbCommand command)
		{
			DataTable table = new DataTable();

				// Rellena la tabla con los datos
				using (System.Data.Common.DbDataAdapter adapter = new SqlDataAdapter(command as SqlCommand))
					adapter.Fill(table);
				// Devuelve la tabla
				return table;
		}

		/// <summary>
		///		Convierte un parámetro
		/// </summary>
		protected override IDataParameter ConvertParameter(ParameterDB parameter)
		{
			if (parameter.Direction == ParameterDirection.ReturnValue)
				return new SqlParameter(parameter.Name, SqlDbType.Int);
			else if (parameter.Value == null)
				return new SqlParameter(parameter.Name, null);
			else if (parameter.IsText)
				return new SqlParameter(parameter.Name, SqlDbType.Text);
			else if (parameter.Value is bool?)
				return new SqlParameter(parameter.Name, SqlDbType.Bit);
			else if (parameter.Value is int?)
				return new SqlParameter(parameter.Name, SqlDbType.Int);
			else if (parameter.Value is double?)
				return new SqlParameter(parameter.Name, SqlDbType.Float);
			else if (parameter.Value is string)
				return new SqlParameter(parameter.Name, SqlDbType.VarChar, parameter.Length);
			else if (parameter.Value is byte[])
				return new SqlParameter(parameter.Name, SqlDbType.Image);
			else if (parameter.Value is DateTime?)
				return new SqlParameter(parameter.Name, SqlDbType.DateTime);
			else
				throw new NotSupportedException($"Tipo del parámetro {parameter.Name} desconocido");
		}
	}
}