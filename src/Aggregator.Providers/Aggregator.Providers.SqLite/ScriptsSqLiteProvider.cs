using System;
using System.Collections.Generic;
using System.Data;

using Bau.Libraries.LibDbProviders.Base.Parameters;
using Bau.Libraries.Aggregator.Providers.Base;
using Bau.Libraries.LibDbProviders.SqLite;
using Bau.Libraries.LibDbProviders.Base.Schema;

namespace Bau.Libraries.Aggregator.Providers.SqLite
{
	/// <summary>
	///		Proveedor de datos para SqLite
	/// </summary>
	public class ScriptsSqLiteProvider : DataProvider
	{
		public ScriptsSqLiteProvider(string key, string type, string fileName) : base(key, type)
		{
			FileName = fileName;
		}

		/// <summary>
		///		Carga una tabla de datos a partir de una cadena SQL
		/// </summary>
		public override IEnumerable<DataTable> LoadData(DataProviderCommand command, int rowsPerPage = 20_000)
		{
			DataTable result = new DataTable();
			string sql = GetCommandSql(command);

				if (!string.IsNullOrEmpty(sql))
					using (SqLiteProvider provider = new SqLiteProvider(new SqLiteConnectionString(FileName, null)))
					{
						int pageIndex = 0;
						long rowsReaded = 0, records;
						ParametersDbCollection parametersDB = GetParametersDb(command.Parameters);

							// Abre la conexión
							provider.Open();
							// Obtiene el número de registros
							records = provider.GetRecordsCount(sql, parametersDB) ?? 0;
							// Recoge las páginas de datos
							while (rowsReaded < records)
							{
								DataTable table = provider.GetDataTable(sql, parametersDB, CommandType.Text, pageIndex++, rowsPerPage);

									// Incrementa el número de filas leídas
									rowsReaded += table.Rows.Count;
									yield return table;
									// Devuelve la tabla
									// Si no hay nada en la tabla, se termina para evitar bucles infinitos
									if (table.Rows.Count == 0)
										rowsReaded = records + 1;
							}
					}
		}

		/// <summary>
		///		Obtiene el comando SQL
		/// </summary>
		private string GetCommandSql(DataProviderCommand command)
		{
			 return GetCommandValue(command.Sentences, "Sentence");
		}

		/// <summary>
		///		Carga una tabla de datos a partir de una cadena SQL
		/// </summary>
		public override DataTable LoadData(DataProviderCommand command, int pageIndex, int pageSize, out long records)
		{
			string sql = GetCommandSql(command);

				// Inicializa los argumentos de salida
				records = 0;
				// Carga los datos
				if (!string.IsNullOrEmpty(sql))
					using (SqLiteProvider provider = new SqLiteProvider(new SqLiteConnectionString(FileName, null)))
					{
						ParametersDbCollection parametersDB = GetParametersDb(command.Parameters);

							// Abre la conexión
							provider.Open();
							// Obtiene el número de registros
							records = provider.GetRecordsCount(sql, parametersDB) ?? 0;
							// Recoge las páginas de datos
							return provider.GetDataTable(sql, parametersDB, CommandType.Text, pageIndex, pageSize);
					}
				// Devuelve una excepción
				throw new Base.Exceptions.DataProviderException("Not found sql in command (key: sentence)");
		}

		/// <summary>
		///		Ejecuta un comando
		/// </summary>
		public override object Execute(DataProviderCommand command)
		{
			string sql = GetCommandSql(command);
			object result = null;

				// Ejecuta el comando
				if (!string.IsNullOrEmpty(sql))
					using (SqLiteProvider provider = new SqLiteProvider(new SqLiteConnectionString(FileName, null)))
					{
						// Abre la conexión
						provider.Open();
						// Ejecuta el comando
						result = provider.ExecuteScalar(sql, GetParametersDb(command.Parameters), CommandType.Text);
					}
				// Devuelve el número de registros
				return result;
		}

		/// <summary>
		///		Ejecuta una serie de comandos
		/// </summary>
		public override void Execute(List< DataProviderCommand> commands)
		{
			using (SqLiteProvider provider = new SqLiteProvider(new SqLiteConnectionString(FileName, null)))
			{
				// Abre la conexión
				provider.Open();
				// Ejecuta los comandos
				foreach (DataProviderCommand command in commands)
				{
					string sql = GetCommandSql(command);

						if (!string.IsNullOrWhiteSpace(sql))
							provider.Execute(sql, GetParametersDb(command.Parameters), CommandType.Text);
				}
			}
		}

		/// <summary>
		///		Obtiene los parámetros de base de datos
		/// </summary>
		protected ParametersDbCollection GetParametersDb(Dictionary<string, object> parameters)
		{
			ParametersDbCollection parametersDB = new ParametersDbCollection();

				// Asigna los parámetros
				if (parameters != null)
					foreach (KeyValuePair<string, object> filter in parameters)
						parametersDB.Add(filter.Key, filter.Value);
				// Devuelve la colección
				return parametersDB;
		}

		/// <summary>
		///		Carga el esquema de base de datos
		/// </summary>
		public override SchemaDbModel LoadSchema()
		{
			using (SqLiteProvider provider = new SqLiteProvider(new SqLiteConnectionString(FileName, null)))
			{
				return provider.GetSchema();
			}
		}

		/// <summary>
		///		Nombre de archivo de la base de datos
		/// </summary>
		private string FileName { get; }
	}
}
