using System;
using System.Data;
using System.Data.SqlServerCe;

using Bau.Libraries.LibDBProvidersBase.Providers;
using Bau.Libraries.LibDBProvidersBase.Parameters;

namespace Bau.Libraries.LibDBProviderSQLServerCE
{
	/// <summary>
	///		Proveedor para SQL Server Compact
	/// </summary>
	public class SQLServerCompactProvider : DBProviderBase
	{ 
		public SQLServerCompactProvider(string strDataBaseFile) : this(new SQLServerCompactConnectionString(strDataBaseFile)) {}

		public SQLServerCompactProvider(SQLServerCompactConnectionString objConnectionString) : base(new SqlCeConnection(objConnectionString.ConnectionString)) {}

		/// <summary>
		///		Obtiene un comando
		/// </summary>
		protected override IDbCommand GetCommand(string strText)
		{ return new SqlCeCommand(strText, Connection as SqlCeConnection);
		}

		/// <summary>
		///		Ejecuta un INSERT sobre la base de datos y obtiene el valor de identidad
		/// </summary>
		public override int? ExecuteGetIdentity(string strText, ParametersDBCollection objColParametersDB, CommandType intCommandType = CommandType.Text)
		{ int? intIdentity = null;

				// Abre la conexión
					Open();
				// Ejecuta sobre la conexión
					Execute(strText, objColParametersDB, intCommandType);
					intIdentity = (int?) ((decimal?) ExecuteScalar("SELECT @@IDENTITY", null, CommandType.Text));
				// Cierra la conexión
					Close();
				// Devuelve el valor identidad
					return intIdentity;
		}

		/// <summary>
		///		Obtiene un dataTable
		/// </summary>
		protected override DataTable FillDataTable(IDbCommand objCommand)
		{	DataTable dtTable = new DataTable();
		
				// Rellena la tabla con los datos
					using (SqlCeDataAdapter objAdapter = new SqlCeDataAdapter(objCommand as SqlCeCommand))
						objAdapter.Fill(dtTable);
				// Devuelve la tabla
					return dtTable;
		}

		/// <summary>
		///		Convierte un parámetro
		/// </summary>
		protected override IDataParameter ConvertParameter(ParameterDB objParameter)
		{	if (objParameter.Direction == ParameterDirection.ReturnValue)
				return new SqlCeParameter(objParameter.Name, SqlDbType.Int);
			else if (objParameter.Value == null)
				return new SqlCeParameter(objParameter.Name,	SqlDbType.Int);
			else if (objParameter.IsText)
				return new SqlCeParameter(objParameter.Name, SqlDbType.Text);
			else if (objParameter.Value is bool?)
				return new SqlCeParameter(objParameter.Name, SqlDbType.Bit);
			else if (objParameter.Value is int?)
				return new SqlCeParameter(objParameter.Name, SqlDbType.Int);
			else if (objParameter.Value is long?)
				return new SqlCeParameter(objParameter.Name, SqlDbType.BigInt);
			else if (objParameter.Value is double?)
				return new SqlCeParameter(objParameter.Name, SqlDbType.Float);
			else if (objParameter.Value is string)
				return new SqlCeParameter(objParameter.Name, SqlDbType.NVarChar, objParameter.Length);
			else if (objParameter.Value is byte [])
				return new SqlCeParameter(objParameter.Name, SqlDbType.Image);
			else if (objParameter.Value is DateTime)
				return new SqlCeParameter(objParameter.Name, SqlDbType.DateTime);
			else
				throw new NotSupportedException("Tipo del parámetro " + objParameter.Name + "desconocido");
		}
	}
}