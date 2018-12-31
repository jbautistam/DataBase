using System;
using System.Data;
using System.Data.SqlClient;

using Bau.Libraries.LibDbProviders.Base;
using Bau.Libraries.LibDbProviders.Base.Schema;
using Bau.Libraries.LibDbProviders.Base.Parameters;

namespace Bau.Libraries.LibDbProviders.SqlServer
{
	/// <summary>
	///		Proveedor para SQL Server
	/// </summary>
	public class SqlServerProvider : DbProviderBase
	{
		public SqlServerProvider(IConnectionString connectionString) : base(connectionString) 
		{ 
			SqlParser = new	SqlServerSelectParser(this);
		}

		/// <summary>
		///		Crea la conexión
		/// </summary>
		protected override IDbConnection GetInstance()
		{
			return new SqlConnection(ConnectionString.ConnectionString);
		}

		/// <summary>
		///		Obtiene un comando
		/// </summary>
		protected override IDbCommand GetCommand(string text)
		{
			return new SqlCommand(text, Connection as SqlConnection, Transaction as SqlTransaction);
		}

		/// <summary>
		///		Convierte un parámetro
		/// </summary>
		protected override IDataParameter ConvertParameter(ParameterDb parameter)
		{
			if (parameter.Direction == ParameterDirection.ReturnValue)
				return new SqlParameter(parameter.Name, SqlDbType.Int);
			if (parameter.Value == null)
				return new SqlParameter(parameter.Name, null);
			if (parameter.IsText)
				return new SqlParameter(parameter.Name, SqlDbType.Text);
			if (parameter.Value is bool?)
				return new SqlParameter(parameter.Name, SqlDbType.Bit);
			if (parameter.Value is int?)
				return new SqlParameter(parameter.Name, SqlDbType.Int);
			if (parameter.Value is double?)
				return new SqlParameter(parameter.Name, SqlDbType.Float);
			if (parameter.Value is string)
				return new SqlParameter(parameter.Name, SqlDbType.VarChar, parameter.Length);
			if (parameter.Value is byte[])
				return new SqlParameter(parameter.Name, SqlDbType.Image);
			if (parameter.Value is DateTime?)
				return new SqlParameter(parameter.Name, SqlDbType.DateTime);
			if (parameter.Value is Enum)
				return new SqlParameter(parameter.Name, SqlDbType.Int);
			throw new NotSupportedException($"Tipo del parámetro {parameter.Name} desconocido");
		}

		/// <summary>
		///		Cambia el timeout de la conexión (tiene que estar cerrada)
		/// </summary>
		public void SetTimeOut(int timeOutSeconds)
		{
			ConnectionString = new SqlServerConnectionString(ConnectionString.ConnectionString, timeOutSeconds);
		}

		/// <summary>
		///		Obtiene el esquema
		/// </summary>
		public override SchemaDbModel GetSchema()
		{
			return new SqlServerSchemaReader().GetSchema(this);
		}
	}
}