using System;
using System.Collections.Generic;

using Bau.Libraries.LibHelper.Extensors;

namespace Bau.Libraries.LibDbScripts.Parser.Interpreters
{
	/// <summary>
	///		Controlador para obtener un script SqlCmd sustituyendo las variables del contexto
	/// </summary>
	internal class ScriptSqlCmdParser
	{
		/// <summary>
		///		Transforma un script SqlCmd desde un archivo
		/// </summary>
		internal string ParseByFile(string fileName, Dictionary<string, object> variables, 
									List<(string variable, string to)> mappings, out string error)
		{
			string content = ReadFile(fileName, out error);

				if (string.IsNullOrWhiteSpace(error))
					return Parse(content, variables, mappings, out error);
				else
					return content;
		}

		/// <summary>
		///		Transforma un script SqlCmd
		/// </summary>
		internal string Parse(string content, Dictionary<string, object> variables, 
							  List<(string variable, string to)> mappings, out string error)
		{
			// Inicializa los argumentos de salida
			error = string.Empty;
			// Elimina las líneas 
			if (!string.IsNullOrEmpty(content))
				content = RemoveSqlCmdLines(content);
			// Cambia las variables de SqlCmd por los valores de las variables y los mapeos
			if (!string.IsNullOrEmpty(content))
			{
				// Añade las variables mapeadas
				foreach ((string variable, string to) in mappings)
					if (!variables.ContainsKey(variable))
						error = $"Cant find the variable '{variable}' for map ({variable} - {to})";
					else
						variables.Add(to, variables[variable]);
				// Sustituye las variables si no hay error
				if (string.IsNullOrWhiteSpace(error))
					content = ReplaceSqlCmdParameters(content, variables);
			}
			// Devuelve el contenido resultante del script
			return content;
		}

		/// <summary>
		///		Lee el contenido del archivo evitando las excepciones
		/// </summary>
		private string ReadFile(string fileName, out string error)
		{
			// Inicializa los argumentos de salida
			error = string.Empty;
			// Lee el contenido del archivo
			try
			{
				return System.IO.File.ReadAllText(fileName, System.Text.Encoding.Default);
			}
			catch (Exception exception)
			{
				error = $"Error when load '{fileName}'. {exception.Message}";
			}
			// Devuelve una cadena vacía
			return string.Empty;
		}

		/// <summary>
		///		Elimina las líneas innecesarias del script SQLCmd: comentarios, GO, SETVAR
		/// </summary>
		private string RemoveSqlCmdLines(string script)
		{
			string[] parts = script.Split('\n');
			string result = string.Empty;

				// Añade las líneas del script al resultado
				foreach (string part in parts)
					if (!string.IsNullOrWhiteSpace(part))
					{
						string line = part.Trim();

							// Añade sólo las líneas que no comiencen por :SETVAR, USE, GO o --
							if (!line.StartsWith(":SETVAR ", StringComparison.CurrentCultureIgnoreCase) && 
									!line.StartsWith("--") &&
									!line.StartsWith("USE ", StringComparison.CurrentCultureIgnoreCase))
									//&&
									//(executeDbccSentences ||
									// (!executeDbccSentences && !line.StartsWith("DBCC"))))
							{
								if (line.Equals("GO", StringComparison.CurrentCultureIgnoreCase))
									result += ";" + Environment.NewLine;
								else
									result += line + Environment.NewLine;
							}
					}
				// Devuelve el resultado
				return result;
		}

		/// <summary>
		///		Reemplaza los valores de las variables del SQLCmd. Del estilo $(Variable)
		/// </summary>
		private string ReplaceSqlCmdParameters(string script, Dictionary<string, object> parameters)
		{
			// Reemplaza las variables por su contenido
			foreach (KeyValuePair<string, object> keyValue in parameters)
				script = script.ReplaceWithStringComparison($"$({keyValue.Key})", ConvertToSqlCmdVariable(keyValue.Value));
			// Devuelve el script
			return script;
		}

		/// <summary>
		///		Convierte un objeto en una cadena para el contenido de una variable de SqlCmd
		/// </summary>
		private string ConvertToSqlCmdVariable(object value)
		{
			if (value == null || value == DBNull.Value)
				return "NULL";
			else
				switch (value)
				{
					case int valueInteger:
						return ConvertIntToSql(valueInteger);
					case short valueInteger:
						return ConvertIntToSql(valueInteger);
					case long valueInteger:
						return ConvertIntToSql(valueInteger);
					case double valueDecimal:
						return ConvertDecimalToSql(valueDecimal);
					case float valueDecimal:
						return ConvertDecimalToSql(valueDecimal);
					case decimal valueDecimal:
						return ConvertDecimalToSql((double) valueDecimal);
					case string valueString:
						return ConvertStringToSql(valueString);
					case DateTime valueDate:
						return ConvertDateToSql(valueDate);
					case bool valueBool:
						return ConvertBooleanToSql(valueBool);
					default:
						return ConvertStringToSql(value.ToString());
				}
		}

		/// <summary>
		///		Convierte un valor lógico a SQL
		/// </summary>
		private string ConvertBooleanToSql(bool value)
		{
			if (value)
				return "1";
			else
				return "0";
		}

		/// <summary>
		///		Convierte una fecha a SQL
		/// </summary>
		private string ConvertDateToSql(DateTime valueDate)
		{
			return $"{valueDate:yyyy-MM-dd HH:mm:ss.ms}";
		}

		/// <summary>
		///		Convierte un valor decimal a Sql
		/// </summary>
		private string ConvertDecimalToSql(double value)
		{
			return value.ToString(System.Globalization.CultureInfo.InvariantCulture);
		}

		/// <summary>
		///		Convierte un entero en una cadena
		/// </summary>
		private string ConvertIntToSql(long value)
		{
			return value.ToString();
		}

		/// <summary>
		///		Convierte una cadena a SQL
		/// </summary>
		private string ConvertStringToSql(string value)
		{
			return value.Replace("'", "''");
		}
	}
}