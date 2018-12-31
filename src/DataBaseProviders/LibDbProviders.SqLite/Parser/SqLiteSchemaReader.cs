using System;
using System.Collections.Generic;
using System.Data;

using Bau.Libraries.LibDbProviders.Base;
using Bau.Libraries.LibDbProviders.Base.Schema;

namespace Bau.Libraries.LibDbProviders.SqLite.Parser
{
	/// <summary>
	///		Clase de lectura del esquema de SqLite
	/// </summary>
	internal class SqLiteSchemaReader
	{
		/// <summary>
		///		Carga el esquema
		/// </summary>
		internal SchemaDbModel GetSchema(SqLiteProvider provider)
		{
			var schema = new SchemaDbModel();

				// Abre la conexión
				provider.Open();
				// Carga los datos
				foreach (string table in GetTables(provider))
					if (!string.IsNullOrWhiteSpace(table))
						using (IDataReader rdoData = provider.ExecuteReader($"PRAGMA table_info([{table}])", null, CommandType.Text))
						{ 
							while (rdoData.Read())
								schema.Add(true, "",
										   table,
										   rdoData.IisNull<string>("Name"),
										   GetFieldType(rdoData.IisNull<string>("Type")),
										   rdoData.IisNull<string>("Type"),
										   GetLengthFieldType(rdoData.IisNull<string>("Type")),
										   rdoData.IisNull<long>("pk") == 1,
										   rdoData.IisNull<long>("notnull") == 1);
						}
				// Cierra la conexión
				provider.Close();
				// Devuelve el esquema
				return schema;
		}

		/// <summary>
		///		Obtiene la lista de tablas
		/// </summary>
		private List<string> GetTables(SqLiteProvider provider)
		{
			List<string> tables = new List<string>();

				// Obtiene los nombres de tabla
				using (IDataReader rdoData = provider.ExecuteReader("SELECT Name FROM sqlite_master WHERE type = 'table'", null, CommandType.Text))
				{
					while (rdoData.Read())
						tables.Add(rdoData.IisNull<string>("name"));
				}
				// Devuelve la colección de tablas
				return tables;
		}

		/// <summary>
		///		Obtiene el tipo de campo
		/// </summary>
		private FieldDbModel.Fieldtype GetFieldType(string columnType)
		{
			// Normaliza el nombre de columna
			columnType = (columnType ?? "").ToUpper();
			// Obtiene el tipo de campo
			if (columnType.StartsWith("TEXT(")) // ... en sqLite el tipo es TEXT(2000), por tanto no entraría por el switch
				return FieldDbModel.Fieldtype.String;
			else
				switch (columnType)
				{
					case "INTEGER":
						return FieldDbModel.Fieldtype.Integer;
					case "FLOAT":
						return FieldDbModel.Fieldtype.Decimal;
					case "TEXT":
						return FieldDbModel.Fieldtype.String;
					case "DATETIME":
						return FieldDbModel.Fieldtype.Date;
					//case "BIT":
					//	return FieldDbModel.Fieldtype.Boolean;
					default:
						return FieldDbModel.Fieldtype.Unknown;
			}
		}

		/// <summary>
		///		Obtiene la longitud a partir del tipo de campo. En sqLite, el tipo de campo es VarChar(2000), por ejemplo
		/// </summary>
		private int GetLengthFieldType(string fieldType)
		{
			return 0;
		}
	}
}
