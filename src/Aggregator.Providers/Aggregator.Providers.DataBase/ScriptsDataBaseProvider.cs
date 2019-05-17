﻿using System;
using System.Collections.Generic;
using System.Data;

using Bau.Libraries.LibDbProviders.Base.Parameters;
using Bau.Libraries.Aggregator.Providers.Base;
using Bau.Libraries.LibDbProviders.Base;
using Bau.Libraries.LibDbProviders.Base.Schema;

namespace Bau.Libraries.Aggregator.Providers.DataBase
{
	/// <summary>
	///		Proveedor de datos para bases de datos
	/// </summary>
	public class ScriptsDataBaseProvider : DataProvider
	{
		public ScriptsDataBaseProvider(string id, string type, IDbProvider provider) 
						: base(id, type)
		{
			Provider = provider;
		}

		/// <summary>
		///		Carga una tabla de datos a partir de una cadena SQL
		/// </summary>
		public override IEnumerable<DataTable> LoadData(DataProviderCommand command, int rowsPerPage = 20_000)
		{
			DataTable result = new DataTable();
			string sql = GetCommandSql(command);

				if (!string.IsNullOrEmpty(sql))
				{
					int pageIndex = 0;
					long rowsReaded = 0, records;
					ParametersDbCollection parametersDB = GetParametersDb(command.Parameters);

						// Abre la conexión
						Provider.Open();
						// Obtiene el número de registros
						records = Provider.GetRecordsCount(sql, parametersDB) ?? 0;
						// Recoge las páginas de datos
						while (rowsReaded < records)
						{
							DataTable table = Provider.GetDataTable(sql, parametersDB, CommandType.Text, pageIndex++, rowsPerPage);

								// Incrementa el número de filas leídas
								rowsReaded += table.Rows.Count;
								// Devuelve la tabla
								yield return table;
								// Si no hay nada en la tabla, se termina para evitar bucles infinitos
								if (table.Rows.Count == 0)
									rowsReaded = records + 1;
						}
						// Cierra la conexión
						Provider.Close();
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
			DataTable result = new DataTable();
			string sql = GetCommandSql(command);

				// Inicializa los argumentos de salida
				records = 0;
				// Carga los datos
				if (!string.IsNullOrEmpty(sql))
				{
					ParametersDbCollection parametersDB = GetParametersDb(command.Parameters);
					DataTable table;

						// Abre la conexión
						Provider.Open();
						// Obtiene el número de registros
						records = Provider.GetRecordsCount(sql, parametersDB) ?? 0;
						// Recoge las páginas de datos
						table = Provider.GetDataTable(sql, parametersDB, CommandType.Text, pageIndex, pageSize);
						// Cierra la conexión
						Provider.Close();
						// Devuelve la tabla
						return table;
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
					{
						// Abre la conexión
						Provider.Open();
						// Ejecuta el comando
						result = Provider.ExecuteScalar(sql, GetParametersDb(command.Parameters), CommandType.Text);
						// Cierra la conexión
						Provider.Close();
					}
				// Devuelve el número de registros
				return result;
		}

		/// <summary>
		///		Ejecuta una serie de comandos
		/// </summary>
		public override void Execute(List<DataProviderCommand> commands)
		{
			// Abre la conexión
			Provider.Open();
			// Ejecuta los comandos
			foreach (DataProviderCommand command in commands)
			{
				string sql = GetCommandSql(command);

					if (!string.IsNullOrWhiteSpace(sql))
						Provider.Execute(sql, GetParametersDb(command.Parameters), CommandType.Text);
			}
			// Cierra la conexión
			Provider.Close();
		}

		/// <summary>
		///		Obtiene los parámetros de base de datos
		/// </summary>
		private ParametersDbCollection GetParametersDb(Dictionary<string, object> parameters)
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
			return Provider.GetSchema();
		}

		/// <summary>
		///		Proveedor de base de datos
		/// </summary>
		private IDbProvider Provider { get; }
	}
}
