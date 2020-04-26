﻿using System;
using System.Data;

using MySql.Data.MySqlClient;
using Bau.Libraries.LibDbProviders.Base;
using Bau.Libraries.LibDbProviders.Base.Parameters;

namespace Bau.Libraries.LibDbProviders.MySql
{
	/// <summary>
	///		Proveedor para MySql
	/// </summary>
	public class MySqlProvider : DbProviderBase
	{
		public MySqlProvider(IConnectionString connectionString) : base(connectionString) 
		{ 
			SqlHelper = new Parser.MySqlSelectParser();
		}

		/// <summary>
		///		Crea la conexión
		/// </summary>
		protected override IDbConnection GetInstance()
		{
			return new MySqlConnection(ConnectionString.ConnectionString);
		}

		/// <summary>
		///		Obtiene un comando
		/// </summary>
		protected override IDbCommand GetCommand(string text, TimeSpan? timeout = null)
		{
			return new MySqlCommand(text, Connection as MySqlConnection, Transaction as MySqlTransaction);
		}

		/// <summary>
		///		Convierte un parámetro
		/// </summary>
		protected override IDataParameter ConvertParameter(ParameterDb parameter)
		{
			// Convierte el parámetro
			if (parameter.Direction == ParameterDirection.ReturnValue)
				return new MySqlParameter(parameter.Name, MySqlDbType.Int32);
			if (parameter.Value == null)
				return new MySqlParameter(parameter.Name, null);
			if (parameter.IsText)
				return new MySqlParameter(parameter.Name, MySqlDbType.String);
			if (parameter.Value is bool?)
				return new MySqlParameter(parameter.Name, MySqlDbType.Bit);
			if (parameter.Value is int?)
				return new MySqlParameter(parameter.Name, MySqlDbType.Int64);
			if (parameter.Value is double?)
				return new MySqlParameter(parameter.Name, MySqlDbType.Double);
			if (parameter.Value is string)
				return new MySqlParameter(parameter.Name, MySqlDbType.VarString, parameter.Length);
			if (parameter.Value is byte[])
				return new MySqlParameter(parameter.Name, MySqlDbType.Byte);
			if (parameter.Value is DateTime?)
				return new MySqlParameter(parameter.Name, MySqlDbType.DateTime);
			if (parameter.Value is Enum)
				return new MySqlParameter(parameter.Name, MySqlDbType.Int16);
			// Si ha llegado hasta aquí, lanza una excepción
			throw new NotSupportedException($"Tipo del parámetro {parameter.Name} desconocido");
		}

		/// <summary>
		///		Obtiene el esquema
		/// </summary>
		public async override System.Threading.Tasks.Task<Base.Schema.SchemaDbModel> GetSchemaAsync(TimeSpan timeout, System.Threading.CancellationToken cancellationToken)
		{
			return await new Parser.MySqlSchemaReader().GetSchemaAsync(this, timeout, cancellationToken);
		}

		/// <summary>
		///		Implementación del sistema de tratamiento de cadenas SQL
		/// </summary>
		public override Base.SqlTools.ISqlHelper SqlHelper { get; }
	}
}