using System;

namespace Bau.Libraries.LibDbProviders.Base.SqlTools
{
	/// <summary>
	///		Parser para sentencias SQL
	/// </summary>
	public abstract class SqlSelectParserBase
	{
		/// <summary>
		///		Obtiene una cadena SQL que cuenta los elementos resultantes de una consulta
		/// </summary>
		public virtual string GetSqlCount(string sql)
		{
			return $"SELECT COUNT(*) FROM ({GetSqlWithoutOrderBy(Normalize(sql))}) AS tmpQuery";
		}

		/// <summary>
		///		Obtiene la cadena SQL necesaria para paginar
		/// </summary>
		public abstract string GetSqlPagination(string sql, int pageNumber, int pageSize);

		/// <summary>
		///		Obtiene el límite inferior para la paginación
		/// </summary>
		protected int GetUpperLimit(int pageNumber, int pageSize)
		{
			return (pageNumber + 1) * pageSize;
		}

		/// <summary>
		///		Obtiene el límite superior para la paginación
		/// </summary>
		protected int GetLowerLimit(int pageNumber, int pageSize)
		{
			return pageSize * pageNumber + 1;
		}

		/// <summary>
		///		Elimina la sección ORDER BY de la consulta
		/// </summary>
		protected string GetSqlWithoutOrderBy(string sql)
		{
			return Cut(sql, "ORDER BY", out string last).Trim();
		}

		/// <summary>
		///		Indica si en la cadena SQL se incluye un Distinct
		/// </summary>
		protected bool HasDistinctClause(string sql)
		{
			return sql.StartsWith("SELECT DISTINCT", StringComparison.CurrentCultureIgnoreCase);
		}

		/// <summary>
		///		Obtiene el alias de un campo
		/// </summary>
		protected string GetAlias(string field)
		{
			if (string.IsNullOrWhiteSpace(field))
				return field;
			else
			{
				int indexStartAs = field.IndexOf(" AS ", StringComparison.InvariantCultureIgnoreCase);

					if (indexStartAs < 0)
						return field;
					else
					{
						int indexEndAs = indexStartAs + " AS ".Length;

							if (indexEndAs < field.Length)
								return field.Substring(indexEndAs);
							else
								return field;
					}
			}
		}

		/// <summary>
		///		Elimina caracteres no deseados de una cadena SQL (saltos de línea, espacios dobles, tabuladores)
		/// </summary>
		protected string Normalize(string sql)
		{ 
			// Normaliza una cadena SQL
			sql = sql.Replace("\t", " ");
			sql = sql.Replace("\r\n", " ");
			sql = sql.Replace("\r", " ");
			sql = sql.Replace("\n", " ");
			// Quita los espacios dobles
			while (sql.IndexOf("  ", StringComparison.InvariantCulture) >= 0)
				sql = sql.Replace("  ", " ");
			// Devuelve la cadena
			return sql;
		}

		/// <summary>
		///		Corta una cadena hasta un separador. Devuelve la parte inicial de la cadena antes del separador
		///	y deja en la cadena inicial, a partir del separador
		/// </summary>
		protected string Cut(string source, string separator, out string target)
		{
			string cut = "";

				// Inicializa los valores de salida
				target = "";
				// Si hay algo que cortar ...
				if (!string.IsNullOrWhiteSpace(source))
				{
					int index = source.IndexOf(separator, StringComparison.CurrentCultureIgnoreCase);

						// Corta la cadena
						if (index < 0)
							cut = source;
						else
							cut = source.Substring(0, index);
						// Borra la cadena cortada
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
