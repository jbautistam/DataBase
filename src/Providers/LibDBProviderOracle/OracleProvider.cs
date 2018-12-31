using System;
using System.Data;
using System.Data.OracleClient;

using Bau.Libraries.LibDBProvidersBase;
using Bau.Libraries.LibDBProvidersBase.Providers;
using Bau.Libraries.LibDBProvidersBase.Parameters;

namespace Bau.Libraries.LibDBProviderOracle
{
	/// <summary>
	///		Proveedor para SQL Server Compact
	/// </summary>
	public class OracleProvider : DBProviderBase
	{ 
		public OracleProvider(IConnectionString objConnectionString) : base(objConnectionString) {}

		/// <summary>
		///		Crea la conexión
		/// </summary>
		protected override IDbConnection GetInstance()
		{ return new OracleConnection(ConnectionString.ConnectionString);
		}

		/// <summary>
		///		Obtiene un comando
		/// </summary>
		protected override IDbCommand GetCommand(string strText)
		{ return new OracleCommand(strText, Connection as OracleConnection);
		}

		/// <summary>
		///		Obtiene un dataTable
		/// </summary>
		protected override DataTable FillDataTable(IDbCommand objCommand)
		{	DataTable dtTable = new DataTable();
		
				// Rellena la tabla con los datos
					using (OracleDataAdapter objAdapter = new OracleDataAdapter(objCommand as OracleCommand))
						objAdapter.Fill(dtTable);
				// Devuelve la tabla
					return dtTable;
		}

		/// <summary>
		///		Convierte un parámetro
		/// </summary>
		protected override IDataParameter ConvertParameter(ParameterDB objParameter)
		{	if (objParameter.Direction == ParameterDirection.ReturnValue)
				return new OracleParameter(objParameter.Name, OracleType.Int32);
			else if (objParameter.Value == null)
				return new OracleParameter(objParameter.Name,	OracleType.Int32);
			else if (objParameter.IsText)
				return new OracleParameter(objParameter.Name, OracleType.VarChar);
			else if (objParameter.Value is bool?)
				return new OracleParameter(objParameter.Name, OracleType.Byte);
			else if (objParameter.Value is int?)
				return new OracleParameter(objParameter.Name, OracleType.Int32);
			else if (objParameter.Value is long?)
				return new OracleParameter(objParameter.Name, OracleType.Int32);
			else if (objParameter.Value is double?)
				return new OracleParameter(objParameter.Name, OracleType.Float);
			else if (objParameter.Value is string)
				return new OracleParameter(objParameter.Name, OracleType.VarChar, objParameter.Length);
			else if (objParameter.Value is byte [])
				return new OracleParameter(objParameter.Name, OracleType.Blob);
			else if (objParameter.Value is DateTime)
				return new OracleParameter(objParameter.Name, OracleType.DateTime);			
			else
				throw new NotSupportedException("Tipo del parámetro " + objParameter.Name + "desconocido");
		}
	}
}