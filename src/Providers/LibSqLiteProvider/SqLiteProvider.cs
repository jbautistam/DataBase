using System;
using System.Data;
using System.Data.SQLite;

using Bau.Libraries.LibDBProvidersBase.Providers;
using Bau.Libraries.LibDBProvidersBase.Parameters;

namespace Bau.Libraries.LibSqLiteProvider
{
	/// <summary>
	///		Proveedor para SqLite
	/// </summary>
	public class SqLiteProvider : DBProviderBase
	{
		public SqLiteProvider(SqLiteConnectionString connectionString) : this(new SQLiteConnection(connectionString.ConnectionString))
		{ 
			ConnectionString = connectionString; 
		}

		public SqLiteProvider(IDbConnection connection) : base(connection) { }

		/// <summary>
		///		Obtiene un comando
		/// </summary>
		protected override IDbCommand GetCommand(string text)
		{
			return new SQLiteCommand(text, Connection as SQLiteConnection, Transaction as SQLiteTransaction);
		}

		/// <summary>
		///		Obtiene un dataTable
		/// </summary>
		protected override DataTable FillDataTable(IDbCommand command)
		{
			DataTable table = new DataTable();

				// Rellena la tabla con los datos
				using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command as SQLiteCommand))
				{
					adapter.FillLoadOption = LoadOption.OverwriteChanges;
					adapter.Fill(table);
				}
				// Devuelve la tabla
				return table;
		}

		/// <summary>
		///		Convierte un parámetro
		/// </summary>
		protected override IDataParameter ConvertParameter(ParameterDB parameter)
		{
			// Convierte el parámetro
			if (parameter.Direction == ParameterDirection.ReturnValue)
				return new SQLiteParameter(parameter.Name, DbType.Int32);
			if (parameter.Value == null)
				return new SQLiteParameter(parameter.Name, null);
			if (parameter.IsText)
				return new SQLiteParameter(parameter.Name, DbType.String);
			if (parameter.Value is bool?)
				return new SQLiteParameter(parameter.Name, DbType.Boolean);
			if (parameter.Value is int?)
				return new SQLiteParameter(parameter.Name, DbType.Int32);
			if (parameter.Value is double?)
				return new SQLiteParameter(parameter.Name, DbType.Double);
			if (parameter.Value is string)
				return new SQLiteParameter(parameter.Name, DbType.String, parameter.Length);
			if (parameter.Value is byte[])
				return new SQLiteParameter(parameter.Name, DbType.Byte);
			if (parameter.Value is DateTime?)
				return new SQLiteParameter(parameter.Name, DbType.DateTime);
			if (parameter.Value is Enum)
				return new SQLiteParameter(parameter.Name, DbType.Int32);
			// Si ha llegado hasta aquí, lanza una excepción
			throw new NotSupportedException($"Tipo del parámetro {parameter.Name} desconocido");
		}

		public override int? ExecuteGetIdentity(string text, ParametersDBCollection parametersDB, CommandType commandType)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///		Datos de conexión
		/// </summary>
		public SqLiteConnectionString ConnectionString { get; }
	}
}