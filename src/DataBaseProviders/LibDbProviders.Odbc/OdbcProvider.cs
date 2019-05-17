using System;
using System.Data;
using System.Data.Odbc;

using Bau.Libraries.LibDbProviders.Base;
using Bau.Libraries.LibDbProviders.Base.Parameters;
using Bau.Libraries.LibDbProviders.Base.Schema;

namespace Bau.Libraries.LibDbProviders.Odbc
{
	/// <summary>
	///		Proveedor para ODBC
	/// </summary>
	public class OdbcProvider : DbProviderBase
	{ 
		public OdbcProvider(IConnectionString connectionString) : base(connectionString) {}

		/// <summary>
		///		Crea la conexión
		/// </summary>
		protected override IDbConnection GetInstance()
		{ 
			return new OdbcConnection(ConnectionString.ConnectionString);
		}

		/// <summary>
		///		Obtiene un comando
		/// </summary>
		protected override IDbCommand GetCommand(string text)
		{ 
			return new OdbcCommand(text, Connection as OdbcConnection);
		}

		/// <summary>
		///		Obtiene un parámetro SQLServer a partir de un parámetro genérico
		/// </summary>
		protected override IDataParameter ConvertParameter(ParameterDb parameter)
		{ 
			if (parameter.Direction == ParameterDirection.ReturnValue)
				return new OdbcParameter(parameter.Name, OdbcType.Int);
			if (parameter.IsText)
				return new OdbcParameter(parameter.Name, OdbcType.VarChar);
			if (parameter.Value is bool)
				return new OdbcParameter(parameter.Name, OdbcType.Bit);
			if (parameter.Value is int)
				return new OdbcParameter(parameter.Name, OdbcType.Int);
			if (parameter.Value is double)
				return new OdbcParameter(parameter.Name, OdbcType.Double);
			if (parameter.Value is string)
				return new OdbcParameter(parameter.Name, OdbcType.VarChar, parameter.Length);
			if (parameter.Value is byte [])
				return new OdbcParameter(parameter.Name, OdbcType.Binary);
			if (parameter.Value is DateTime)
				return new OdbcParameter(parameter.Name, OdbcType.Date);
			if (parameter.Value is Enum)
				return new OdbcParameter(parameter.Name, OdbcType.Int);
			throw new NotSupportedException($"Tipo del parámetro {parameter.Name} desconocido");
		}

		/// <summary>
		///		Obtiene el esquema
		/// </summary>
		public override SchemaDbModel GetSchema()
		{
			return new OdbcSchemaReader().GetSchema(this);
		}
	}
}