using System;
using System.Data;
using System.Data.Odbc;

using Bau.Libraries.LibDbProviders.Base;
using Bau.Libraries.LibDbProviders.Base.Parameters;

namespace Bau.Libraries.LibDbProviders.Spark
{
	/// <summary>
	///		Proveedor para Spark
	/// </summary>
	public class SparkProvider : DbProviderBase
	{ 
		public SparkProvider(IConnectionString connectionString) : base(connectionString) 
		{
			SqlHelper = new Parser.SparkSelectParser();
		}

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
		protected override IDbCommand GetCommand(string text, TimeSpan? timeout = null)
		{ 
			return new OdbcCommand(text, Connection as OdbcConnection);
		}

		/// <summary>
		///		Obtiene un parámetro SQLServer a partir de un parámetro genérico
		/// </summary>
		protected override IDataParameter ConvertParameter(ParameterDb parameter)
		{ 
			// Crea un parámetro de retorno entero
			if (parameter.Direction == ParameterDirection.ReturnValue)
				return new OdbcParameter(parameter.Name, OdbcType.Int);
			// Asigna el tipo de parámetro
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
			// Devuelve un parámetro genérico
			return new OdbcParameter(parameter.Name, parameter.Value);
		}

		/// <summary>
		///		Obtiene el esquema
		/// </summary>
		public async override System.Threading.Tasks.Task<Base.Schema.SchemaDbModel> GetSchemaAsync(TimeSpan timeout, System.Threading.CancellationToken cancellationToken)
		{
			return await new Parser.SparkSchemaReader().GetSchemaAsync(this, timeout, cancellationToken);
		}

		/// <summary>
		///		Implementación del sistema de tratamiento de cadenas SQL
		/// </summary>
		public override Base.SqlTools.ISqlHelper SqlHelper { get; }
	}
}