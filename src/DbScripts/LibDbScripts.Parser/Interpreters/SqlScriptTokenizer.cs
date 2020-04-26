using System;
using System.Collections.Generic;

using Bau.Libraries.LibHelper.Extensors;

namespace Bau.Libraries.LibDbScripts.Parser.Interpreters
{
	/// <summary>
	///		Tokenizador de una cadena SQL
	/// </summary>
	internal class SqlScriptTokenizer
	{
		// Constantes privadas
		private const string FormatNotQuote = "WithoutQuote";
		// Constantes con los separadores de inicio y fin de las variables para SqlCmd
		private const string StartVariableSqlCmd = "$(";
		private const string EndVariableSqlCmd = ")";

		/// <summary>
		///		Estados del tokenizador
		/// </summary>
		private enum Mode
		{
			/// <summary>No estamos en ningún contenido aún (acaba de empezar el proceso)</summary>
			Unknown,
			/// <summary>En un comentario</summary>
			AtComment,
			/// <summary>Al final de un comentario de línea</summary>
			AtEndComment,
			/// <summary>En una consulta SQL</summary>
			AtSql,
			/// <summary>En un cierre de consulta SQL</summary>
			AtGo,
			/// <summary>En un comando de SQLCmd</summary>
			AtSqlCmd
		}

		/// <summary>
		///		Tipo de línea
		/// </summary>
		private enum LineType
		{
			/// <summary>Desconocido</summary>
			Unknown,
			/// <summary>Inicio de comentario multilínea</summary>
			StartCommentMultiline,
			/// <summary>Fin de un comentario multilínea</summary>
			EndCommentMultiline,
			/// <summary>Inicio de comentario de una línea</summary>
			StartCommentLine,
			/// <summary>Comando GO</summary>
			Go,
			/// <summary>Comando de SqlCmd</summary>
			SqlCmd
		}

		/// <summary>
		///		Interpreta un archivo SQL separando por secciones
		/// </summary>
		internal List<SqlSectionModel> ParseByFile(string fileName, Dictionary<string, object> parameters, out string error)
		{
			return Parse(LibHelper.Files.HelperFiles.LoadTextFile(fileName), parameters, out error);
		}
		
		/// <summary>
		///		Interpreta una cadena SQL separando por secciones
		/// </summary>
		internal List<SqlSectionModel> Parse(string sql, Dictionary<string, object> variables, out string error)
		{
			List<SqlSectionModel> sqlParts = Tokenize(sql);

				// Inicializa los argumentos de salida
				error = string.Empty;
				// Reemplaza las variables
				foreach (SqlSectionModel sqlPart in sqlParts)
					if (sqlPart.Type == SqlSectionModel.SectionType.Sql)
						sqlPart.Content	= ReplaceConstants(sqlPart.Content, variables, out error);
				// Devuelve las secciones de la cadena SQL
				return sqlParts;
		}

		/// <summary>
		///		Divide la cadena SQL entre comentarios y comandos
		/// </summary>
		private List<SqlSectionModel> Tokenize(string sql)
		{
			List<SqlSectionModel> sqlParts = new List<SqlSectionModel>();
			Mode mode = Mode.Unknown;
			string content = string.Empty;

				// Si hay algo que interpretar...
				if (!string.IsNullOrWhiteSpace(sql))
				{
					// Separa la cadena por la palabra reservada GO
					foreach (string part in sql.Split('\r', '\n'))
						if (!string.IsNullOrWhiteSpace(part))
						{
							// Actua dependiendo del modo
							switch (mode)
							{
								case Mode.Unknown:
										switch (GetLineType(part))
										{
											case LineType.StartCommentMultiline:
													mode = Mode.AtComment;
												break;
											case LineType.StartCommentLine:
													mode = Mode.AtEndComment;
												break;
											case LineType.SqlCmd:
													mode = Mode.AtSqlCmd;
												break;
											default:
													mode = Mode.AtSql;
												break;
										}
										content += part + Environment.NewLine;
									break;
								case Mode.AtSql:
										switch (GetLineType(part))
										{
											case LineType.Go:
											case LineType.StartCommentLine:
											case LineType.StartCommentMultiline:
											case LineType.SqlCmd:
													mode = Mode.AtGo;
												break;
											default:
													content += part + Environment.NewLine;
												break;
										}
									break;
								case Mode.AtComment:
										// Añade el contenido al comentario (siempre)
										content += part + Environment.NewLine;
										// Calcula el modo dependiendo del tipo de líneas
										switch (GetLineType(part))
										{
											case LineType.EndCommentMultiline:
													mode = Mode.AtEndComment;
												break;
										}
									break;
							}
							// Si estamos en un final de comentario de línea, añadimos el contenido directamente y limpiamos
							// Si estamos en un final de SQL, añadimos el contenido y limpiamos
							if (mode == Mode.AtEndComment)
							{
								sqlParts.Add(new SqlSectionModel(SqlSectionModel.SectionType.Comment, content));
								content = string.Empty;
								mode = Mode.Unknown;
							}
							else if (mode == Mode.AtGo)
							{
								// Añade el comando (si no está vacío)
								if (!string.IsNullOrWhiteSpace(content))
									sqlParts.Add(new SqlSectionModel(SqlSectionModel.SectionType.Sql, content));
								// Prepara para el resto del bucle
								content = string.Empty;
								mode = Mode.Unknown;
							}
							else if (mode == Mode.AtSqlCmd) 
							{
								// Añade el comando a la lista de secciones
								sqlParts.Add(new SqlSectionModel(SqlSectionModel.SectionType.ExternalCommand, content));
								// Prepara para el resto del bucle
								content = string.Empty;
								mode = Mode.Unknown;
							}
						}
					// Añade la última cadena
					if (!string.IsNullOrWhiteSpace(content))
					{
						if (mode == Mode.AtComment)
							sqlParts.Add(new SqlSectionModel(SqlSectionModel.SectionType.Comment, content));
						else
							sqlParts.Add(new SqlSectionModel(SqlSectionModel.SectionType.Sql, content));
					}
				}
				// Devuelve la lista de comandos de salida
				return sqlParts;
		}

		/// <summary>
		///		Obtiene el tipo de línea
		/// </summary>
		private LineType GetLineType(string content)
		{
			// Quita los espacios
			content = content.TrimIgnoreNull();
			// Comprueba los datos
			if (content.StartsWith("/*"))
			{
				if (content.EndsWith("*/"))
					return LineType.StartCommentLine;
				else
					return LineType.StartCommentMultiline;
			}
			if (content.StartsWith("--"))
				return LineType.StartCommentLine;
			if (content.EndsWith("*/"))
				return LineType.EndCommentMultiline;
			if (content.Equals("GO", StringComparison.CurrentCultureIgnoreCase))
				return LineType.Go;
			if (content.StartsWith(":SETVAR ", StringComparison.CurrentCultureIgnoreCase) ||
					content.StartsWith("USE ", StringComparison.CurrentCultureIgnoreCase) ||
					content.StartsWith("PRINT ", StringComparison.CurrentCultureIgnoreCase))
				return LineType.SqlCmd;
			// Si ha llegado hasta aquí es porque no es nada especial
			return LineType.Unknown;
		}

		/// <summary>
		///		Reemplaza las variables que aparezcan entre {{ }} o sean SQLCmd
		/// </summary>
		internal string ReplaceConstants(string sql, Dictionary<string, object> constants, out string error)
		{
			// Reemplaza las variables que estén entre {{ y }}
			foreach (KeyValuePair<string, object> keyValue in constants)
				sql = sql.ReplaceWithStringComparison("{{" + keyValue.Key + "}}", keyValue.Value.ToString());
			// Reemplaza los valores de escape (\{\{ y \}\}
			sql = sql.ReplaceWithStringComparison("\\{\\{", "{{");
			sql = sql.ReplaceWithStringComparison("\\}\\}", "}}");
			// Interpreta las variables de SqlCmd
			sql = ParseSqlCommand(sql, constants, out error);
			// Devuelve la cadena convertida
			return sql;
		}

		/// <summary>
		///		Interpreta las variables de SqlCmd
		/// </summary>
		private string ParseSqlCommand(string command, Dictionary<string, object> variables, out string error)
		{
			string result = string.Empty;

				// Inicializa los argumentos de salida
				error = string.Empty;
				// Mientras quede algo de texto por interpretar ...
				while (!string.IsNullOrWhiteSpace(command) && string.IsNullOrWhiteSpace(error))
				{
					int start = command.IndexOf(StartVariableSqlCmd);

						// Interpreta la variable
						if (start >= 0)
						{
							int end;

								// Añade el inicio al resultado
								result += command.Left(start);
								// Quita el token de inicio de la variable del texto
								command = command.From(start + EndVariableSqlCmd.Length);
								// Obtiene el índice de la parte final de la variable
								end = command.IndexOf(EndVariableSqlCmd);
								// Si se ha encontrado un nombre de variable
								if (end >= 0)
								{
									(string variableName, string format) = GetVariableName(command.Left(end));

										// Si se ha definido la variable
										if (!variables.ContainsKey(variableName))
											error = $"Can't find the variable {variableName}";
										else
										{
											object variableValue = variables[variableName];

												// Quita el nombre de la variable del texto
												command = command.From(end + EndVariableSqlCmd.Length);
												// y añade el contenido de la variable al resultado
												result += ConvertStringValue(variableValue, format);
										}
								}
								else //... no hay cadena de fin de variable se añade al resultado el comienzo de variable
									result += EndVariableSqlCmd;
						}
						else
						{
							result += command;
							command = string.Empty;
						}
				}
				// Devuelve el contenido
				return result;
		}

		/// <summary>
		///		Obtiene el nombre de variable y el formato
		/// </summary>
		private (string variableName, string format) GetVariableName(string text)
		{	
			int indexSeparator = text.IndexOf(':');

				// Separa el nombre de variable del formato si es necesario
				if (indexSeparator >= 0)
					return (text.Substring(0, indexSeparator), text.Substring(indexSeparator + 1));
				else
					return (text, string.Empty);
		}

		/// <summary>
		///		Convierte el valor de una cadena
		/// </summary>
		protected string ConvertStringValue(object value, string format)
		{
			if (value is int || value is long || value is float || value is double || value is decimal)
				return ConvertDouble(value.ToString().GetDouble());
			else if (value is DateTime date)
				return ConvertDateTime(date);
			else if (value is bool boolean)
				return ConvertBoolean(boolean);
			else
				return ConvertString(value?.ToString(), format);
		}

		/// <summary>
		///		Convierte un valor numérico
		/// </summary>
		private string ConvertDouble(double? value)
		{
			if (value == null)
				return "NULL";
			else
				return (value ?? 0).ToString(System.Globalization.CultureInfo.InvariantCulture);
		}

		/// <summary>
		///		Convierte un valor de fecha
		/// </summary>
		private string ConvertDateTime(DateTime? value)
		{
			if (value == null)
				return "NULL";
			else
				return $"'{value:yyyy-MM-dd HH:mm:ss}'";
		}

		/// <summary>
		///		Convierte un valor de cadena
		/// </summary>
		private string ConvertString(string value, string format)
		{
			if (value == null)
				return "NULL";
			else if (format.EqualsIgnoreCase(FormatNotQuote))
				return value.Replace("'", "''");
			else if (string.IsNullOrEmpty(value))
				return "''";
			else
				return $"'{value.Replace("'", "''")}'";
		}

		/// <summary>
		///		Convierte un valor lógico
		/// </summary>
		private string ConvertBoolean(bool value)
		{
			if (value)
				return "0";
			else 
				return "1";
		}
	}
}
