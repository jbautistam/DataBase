using System;

namespace Bau.Libraries.LibMySqlProvider.Parser
{
	/// <summary>
	///		Parser para consultas en SQL de MySql
	/// </summary>
	internal class MySqlSelectParser : LibDbProviders.Base.SqlTools.SqlSelectParserBase
	{
		/// <summary>
		///		Obtiene una cadena SQL con paginación en el servidor
		/// </summary>
		public override string GetSqlPagination(string sql, int pageNumber, int pageSize)
		{
			string offset = "";

				// Calcula la cadena de Offset
				if (pageNumber > 0)
					offset = $"{pageNumber * pageSize}, ";
				// Devuelve la cadena SQL con la paginación
				return $"{sql} LIMIT {offset} {pageSize}";
		}
	}
}