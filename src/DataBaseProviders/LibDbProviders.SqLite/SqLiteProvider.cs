using System;
using System.Data;
using System.Data.SQLite;

using Bau.Libraries.LibDbProviders.Base;
using Bau.Libraries.LibDbProviders.Base.Parameters;

namespace Bau.Libraries.LibDbProviders.SqLite
{
	/// <summary>
	///		Proveedor para SqLite
	/// </summary>
	public class SqLiteProvider : DbProviderBase
	{
		public SqLiteProvider(IConnectionString connectionString) : base(connectionString) 
		{ 
			SqlParser = new Parser.SqLiteSelectParser();
		}

		/// <summary>
		///		Crea la conexión
		/// </summary>
		protected override IDbConnection GetInstance()
		{
			return new SQLiteConnection(ConnectionString.ConnectionString);
		}

		/// <summary>
		///		Obtiene un comando
		/// </summary>
		protected override IDbCommand GetCommand(string text)
		{
			return new SQLiteCommand(text, Connection as SQLiteConnection, Transaction as SQLiteTransaction);
		}

		/// <summary>
		///		Convierte un parámetro
		/// </summary>
		protected override IDataParameter ConvertParameter(ParameterDb parameter)
		{
			// Convierte el parámetro
			if (parameter.Direction == ParameterDirection.ReturnValue)
				return new SQLiteParameter(parameter.Name, DbType.Int32);
			if (parameter.Value == null)
				return new SQLiteParameter(parameter.Name, null);
			if (parameter.IsText)
				return new SQLiteParameter(parameter.Name, DbType.String);
			if (parameter.Value is bool?)
				return new SQLiteParameter(parameter.Name, DbType.Int64);
			if (parameter.Value is int?)
				return new SQLiteParameter(parameter.Name, DbType.Int64);
			if (parameter.Value is double?)
				return new SQLiteParameter(parameter.Name, DbType.Double);
			if (parameter.Value is string)
				return new SQLiteParameter(parameter.Name, DbType.String, parameter.Length);
			if (parameter.Value is byte[])
				return new SQLiteParameter(parameter.Name, DbType.Binary);
			if (parameter.Value is DateTime?)
				return new SQLiteParameter(parameter.Name, DbType.DateTime);
			if (parameter.Value is Enum)
				return new SQLiteParameter(parameter.Name, DbType.Int64);
			// Si ha llegado hasta aquí, lanza una excepción
			throw new NotSupportedException($"Tipo del parámetro {parameter.Name} desconocido");
		}

		/// <summary>
		///		Obtiene el esquema
		/// </summary>
		public override Base.Schema.SchemaDbModel GetSchema()
		{
			return new Parser.SqLiteSchemaReader().GetSchema(this);
		}
	}
}