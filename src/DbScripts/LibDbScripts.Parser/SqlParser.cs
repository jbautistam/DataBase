using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibDbScripts.Parser
{
	/// <summary>
	///		Parser de scripts SQL
	/// </summary>
	public class SqlParser
	{
		/// <summary>
		///		Interpreta un comando
		/// </summary>
		public string ParseCommand(string command, Dictionary<string, object> parameters, out string error)
		{
			return new Interpreters.SqlScriptTokenizer().ReplaceConstants(command, parameters, out error);
		}

		/// <summary>
		///		Obtiene las secciones de un archivo SQL
		/// </summary>
		public List<SqlSectionModel> TokenizeByFile(string fileName, Dictionary<string, object> parameters, out string error)
		{
			return new Interpreters.SqlScriptTokenizer().ParseByFile(fileName, parameters, out error);
		}

		/// <summary>
		///		Obtiene las secciones de una cadena SQL
		/// </summary>
		public List<SqlSectionModel> Tokenize(string sql, Dictionary<string, object> parameters, out string error)
		{
			return new Interpreters.SqlScriptTokenizer().Parse(sql, parameters, out error);
		}
	}
}
