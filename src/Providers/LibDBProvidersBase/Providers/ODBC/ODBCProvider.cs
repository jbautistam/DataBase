using System;
using System.Data;
using System.Data.Odbc;

using Bau.Libraries.LibDBProvidersBase.Parameters;

namespace Bau.Libraries.LibDBProvidersBase.Providers.ODBC
{
	/// <summary>
	///		Proveedor para ODBC
	/// </summary>
	public class ODBCProvider : DBProviderBase
	{
		public ODBCProvider(ODBCConnectionString connectionString) : this(new OdbcConnection(connectionString.ConnectionString)) { }

		public ODBCProvider(IDbConnection connection) : base(connection) { }

		/// <summary>
		///		Obtiene un comando
		/// </summary>
		protected override IDbCommand GetCommand(string text)
		{
			return new OdbcCommand(text, Connection as OdbcConnection);
		}

		/// <summary>
		///		Obtiene un dataTable
		/// </summary>
		protected override DataTable FillDataTable(IDbCommand command)
		{
			DataTable table = new DataTable();

				// Rellena la tabla con los datos
				using (OdbcDataAdapter adapter = new OdbcDataAdapter(command as OdbcCommand))
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
		///		Obtiene un parámetro SQLServer a partir de un parámetro genérico
		/// </summary>
		protected override IDataParameter ConvertParameter(ParameterDB parameter)
		{
			if (parameter.Direction == ParameterDirection.ReturnValue)
				return new OdbcParameter(parameter.Name, OdbcType.Int);
			else if (parameter.IsText)
				return new OdbcParameter(parameter.Name, OdbcType.VarChar);
			else if (parameter.Value is bool)
				return new OdbcParameter(parameter.Name, OdbcType.Bit);
			else if (parameter.Value is int)
				return new OdbcParameter(parameter.Name, OdbcType.Int);
			else if (parameter.Value is double)
				return new OdbcParameter(parameter.Name, OdbcType.Double);
			else if (parameter.Value is string)
				return new OdbcParameter(parameter.Name, OdbcType.VarChar, parameter.Length);
			else if (parameter.Value is byte[])
				return new OdbcParameter(parameter.Name, OdbcType.Binary);
			else if (parameter.Value is DateTime)
				return new OdbcParameter(parameter.Name, OdbcType.Date);
			else
				throw new NotSupportedException($"Tipo del parámetro {parameter.Name} desconocido");
		}
	}
}