using System;

namespace Bau.Libraries.LibDBProvidersBase.RepositoryData.Sql
{
	/// <summary>
	///		Intérprete de consultas SQL
	/// </summary>
	public class SqlParserBase
	{
		/// <summary>
		///		Parte de la SQL original para obtener una nueva SQL transformada para que permita paginación.
		/// </summary>
		public string GetSQLPagination(string sql, int pageNumber, int pageSize)
		{
			string sqlPagination;

				// Elimina caracteres no deseados
				sql = NormalizeSql(sql);
				// Obtiene la parte Row_Number y lo incluye como campo de la SQL original
				sqlPagination = GetRowNumberSection(sql) + ", " + GetInitialSql(sql);
				// Rodea la consulta con SELECT * FROM (sql) añadiéndole un alias y adjunta la sección de paginación del tipo: WHERE RowNumber BETWEEN x AND y
				sqlPagination = $" SELECT * FROM ({sqlPagination}) AS ResultsQuery WHERE RowNumber BETWEEN {(pageSize * pageNumber) + 1} AND {(pageNumber + 1) * pageSize}";
				// Devuelve la cadena SQL paginada
				return sqlPagination;
		}

		/// <summary>
		///		Añade a la consulta original, la parte correspondiente al SELECT COUNT(*)
		/// </summary>
		public string GetSQLCount(string sql)
		{
			return $"SELECT COUNT(*) FROM ({RemoveOrderBy(sql)}) AS tmpQuery";
		}

		/// <summary>
		///		Devuelve una sentencia SQL preparada para tratarla con una consulta de paginación
		/// </summary>
		private string GetInitialSql(string sqlOriginal)
		{
			string sql = RemoveOrderBy(sqlOriginal);

				// Si la SQL tiene un distinct, se quita
				if (HasDistinctClause(sqlOriginal))
					sql = $" * FROM ({sql}) AS tmp1";
				else // ... si no hay DISTINCT eliminamos la palabra SELECT de la consulta
					sql = sql.Remove(0, "SELECT".Length);
				// Devuelve la cadena SQL
				return sql.Trim();
		}

		/// <summary>
		///		Elimina la sección ORDER BY de la consulta.
		/// </summary>
		private string RemoveOrderBy(string sql)
		{
			return Cut(sql, "ORDER BY", out string last).Trim();
		}

		/// <summary>
		///		Indica si en la cadena SQL se incluye un Distinct
		/// </summary>
		private bool HasDistinctClause(string sql)
		{
			return sql.StartsWith("SELECT DISTINCT", StringComparison.CurrentCultureIgnoreCase);
		}

		/// <summary>
		///		Elimina caracteres no deseados
		/// </summary>
		private string NormalizeSql(string sql)
		{ 
			// Quita tabuladores y saltos de línea
			sql = sql.Replace("\t", " ");
			sql = sql.Replace("\r\n", " ");
			sql = sql.Replace("\r", " ");
			sql = sql.Replace("\n", " ");
			// Quita los espacios dobles
			while (sql.IndexOf("  ") >= 0)
				sql = sql.Replace("  ", " ");
			// Devuelve la cadena
			return sql;
		}

		/// <summary>
		///		A partir de la cadena SQL, devuelve la sección correspondiente al ROW_NUMBER:
		/// (SELECT ROW_NUMBER() OVER (ORDER BY [strOrderBy])
		/// </summary>
		private string GetRowNumberSection(string sql)
		{
			string orderBySection = string.Empty;

				// Quita el ORDER By de la cadena SQL
				Cut(sql, "ORDER BY", out orderBySection);
				// Si no hay cláusula ORDER BY, incluimos en el ORDER BY necesario en la sección ROW_NUMBER el primer
				// campo de la SELECT.
				if (string.IsNullOrWhiteSpace(orderBySection))
					orderBySection = GetSqlFirstField(sql);
				// Cuando la consulta consta de un DISTINCT, se quitan las referencias a tablas del ORDER BY      
				if (HasDistinctClause(sql))
					orderBySection = RemoveTableReferencesToOrderBySection(orderBySection);
				// Devuelve la cadena SQL
				return $"SELECT ROW_NUMBER() OVER (ORDER BY {orderBySection}) AS RowNumber";
		}

		/// <summary>
		///		Devuelve el primer campo de la consulta
		/// </summary>
		private string GetSqlFirstField(string sql)
		{
			string firstField = string.Empty;
			string sqlLast = string.Empty;

				// Obtiene la primera parte de la consulta, por ejemplo: SELECT tabla1.campo1
				firstField = Cut(sql, ",", out sqlLast);
				// Si tiene un DISTINCT se ha de quitar también.
				if (HasDistinctClause(firstField))
					firstField = firstField.Remove(0, "SELECT DISTINCT".Length);
				else
					firstField = firstField.Remove(0, "SELECT".Length);
				// Quita la referencia a tablas.
				firstField = RemoveTableReferencesToElement(firstField);
				// Si tiene un alias, obtiene el alias
				firstField = GetAlias(firstField);
				// Devuelve la cadena
				return firstField;
		}

		/// <summary>
		///		Obtiene el alias de un campo
		/// </summary>
		private string GetAlias(string firstField)
		{
			if (string.IsNullOrEmpty(firstField))
				return firstField;
			else
			{
				int indexStartAs = firstField.IndexOf(" AS ", StringComparison.CurrentCultureIgnoreCase);

					if (indexStartAs < 0)
						return firstField;
					else
					{
						int indexEndAs = indexStartAs + " AS ".Length;

							if (indexEndAs < firstField.Length)
								return firstField.Substring(indexEndAs);
							else
								return firstField;
					}
			}
		}

		/// <summary>
		///		Elimina las referencias a las tablas de los elementos del ORDER BY
		/// </summary>
		private string RemoveTableReferencesToOrderBySection(string orderBySection)
		{
			string orderByWithoutTables = string.Empty;

				// Obtiene los elementos de la sección ORDER BY sin las referencias de tablas
				if (!string.IsNullOrWhiteSpace(orderBySection))
				{
					string [] fields = orderBySection.Trim().Split(',');

						// Si el array no contiene nada es porque solo incluye uno, con lo que se devuelve sólo ese elemento
						if (fields != null && fields.Length > 0)
						{
							// Recorre los campos rellenando la lista de campos
							foreach (string field in fields)
							{
								string fieldWithoutTable = RemoveTableReferencesToElement(field.Trim());

									// Añade el nombre del campo a la lista
									if (!string.IsNullOrWhiteSpace(fieldWithoutTable))
									{
										// Añade el separador si es necesario
										if (!string.IsNullOrWhiteSpace(orderByWithoutTables))
											orderByWithoutTables += ", ";
										// Añade el campo
										orderByWithoutTables += fieldWithoutTable;
									}
							}
						}
						else
							orderByWithoutTables = orderBySection;
				}
				// Devuelve los campos de la cláusula ORDER BY sin las tablas
				return orderByWithoutTables;
		}

		/// <summary>
		///		Devuelve el elemento sin referencias a tablas
		/// </summary>
		private string RemoveTableReferencesToElement(string field)
		{
			string cutResult = Cut(field, ".", out string fieldWithoutTable);

				// Si no existen referencias a tablas, o el campo está precedido por dbo se devuelve el mismo elemento enviado como parámetro
				if (string.IsNullOrWhiteSpace(fieldWithoutTable) || cutResult.Equals("dbo", StringComparison.InvariantCultureIgnoreCase))
					return field;
				else
					return fieldWithoutTable;
		}

		/// <summary>
		///		Corta una cadena hasta un separador. Devuelve la parte inicial de la cadena antes del separador
		///	y deja en la cadena inicial, a partir del separador
		/// </summary>
		private string Cut(string source, string separator, out string target)
		{
			int index;
			string cut = "";

				// Inicializa los valores de salida
				target = "";
				// Si hay algo que cortar ...
				if (!string.IsNullOrWhiteSpace(source))
				{ 
					// Obtiene el índice donde se encuentra el separador
					index = source.IndexOf(separator, StringComparison.CurrentCultureIgnoreCase);
					// Corta la cadena
					if (index < 0)
						cut = source;
					else
						cut = source.Substring(0, index);
					// Borra al cadena cortada
					if ((cut + separator).Length - 1 < source.Length)
						target = source.Substring((cut + separator).Length);
					else
						target = "";
				}
				// Devuelve la primera parte de la cadena
				return cut;
		}
	}
}
