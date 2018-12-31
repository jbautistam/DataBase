using System;
using System.Data;
using System.Data.OleDb;

using Bau.Libraries.LibDBProvidersBase.Parameters;

namespace Bau.Libraries.LibDBProvidersBase.Providers.OleDB
{
	/// <summary>
	///		Proveedor para OleDB
	/// </summary>
	public class OleDBProvider : DBProviderBase
	{
		public OleDBProvider(OleDBConnectionString connectionString) : this(new OleDbConnection(connectionString.ConnectionString)) { }

		public OleDBProvider(IDbConnection connection) : base(connection) { }

		/// <summary>
		///		Obtiene un comando
		/// </summary>
		protected override IDbCommand GetCommand(string text)
		{
			return new OleDbCommand(text, Connection as OleDbConnection);
		}

		/// <summary>
		///		Obtiene un dataTable
		/// </summary>
		protected override DataTable FillDataTable(IDbCommand command)
		{
			DataTable table = new DataTable();

				// Rellena la tabla con los datos
				using (OleDbDataAdapter adapter = new OleDbDataAdapter(command as OleDbCommand))
					adapter.Fill(table);
				// Devuelve la tabla
				return table;
		}

		/// <summary>
		///		Ejecuta un INSERT sobre la base de datos y devuelve el ID de IDENTITY
		/// </summary>
		public override int? ExecuteGetIdentity(string text, ParametersDBCollection parametersDB, CommandType commandType)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///		Convierte un parámetro
		/// </summary>
		protected override IDataParameter ConvertParameter(ParameterDB parameter)
		{
			if (parameter.Direction == ParameterDirection.ReturnValue)
				return new OleDbParameter(parameter.Name, OleDbType.Integer);
			else if (parameter.IsText)
				return new OleDbParameter(parameter.Name, OleDbType.VarChar);
			else if (parameter.Value is bool)
				return new OleDbParameter(parameter.Name, OleDbType.Boolean);
			else if (parameter.Value is int)
				return new OleDbParameter(parameter.Name, OleDbType.Integer);
			else if (parameter.Value is double)
				return new OleDbParameter(parameter.Name, OleDbType.Double);
			else if (parameter.Value is string)
				return new OleDbParameter(parameter.Name, OleDbType.VarChar, parameter.Length);
			else if (parameter.Value is byte[])
				return new OleDbParameter(parameter.Name, OleDbType.Binary);
			else if (parameter.Value is DateTime)
				return new OleDbParameter(parameter.Name, OleDbType.Date);
			else
				throw new NotSupportedException($"Tipo del parámetro {parameter.Name} desconocido");
		}
	}
}