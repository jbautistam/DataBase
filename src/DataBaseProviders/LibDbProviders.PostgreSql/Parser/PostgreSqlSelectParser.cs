using System;

namespace Bau.Libraries.LibPostgreSqlProvider.Parser
{
	/// <summary>
	///		Parser para consultas en SQL de PostgreSql
	/// </summary>
	internal class PostgreSqlSelectParser : LibDbProviders.Base.SqlTools.SqlSelectParserBase
	{
		/// <summary>
		///		Obtiene una cadena SQL con paginación en el servidor
		/// </summary>
		public override string GetSqlPagination(string sql, int pageNumber, int pageSize)
		{
			string offset = "";

				// Calcula la cadena de Offset
				if (pageNumber > 0)
					offset = $" OFFSET {pageNumber * pageSize} ";
				// Devuelve la cadena SQL con la paginación
				return $"{sql} LIMIT {pageSize} {offset}";
		}
	}
}