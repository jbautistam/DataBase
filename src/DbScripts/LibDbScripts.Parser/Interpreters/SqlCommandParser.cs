using System;
using System.Collections.Generic;

using Bau.Libraries.LibHelper.Extensors;

namespace Bau.Libraries.LibDbScripts.Parser.Interpreters
{
	/// <summary>
	///		Intérprete para evaluación de cadenas de comandos sql
	/// </summary>
	internal class SqlCommandParser
	{
		// Constantes privadas
		private const string FormatNotQuote = "WithoutQuote";
		// Constantes con los separadores de inicio y fin de las variables
		protected const string StartVariable = "{{";
		protected const string EndVariable = "}}";

		/// <summary>
		///		Interpreta un comando SQL
		/// </summary>
		internal string ParseCommand(string command, Dictionary<string, object> variables, out string error)
		{
			string result = string.Empty;

				// Inicializa los argumentos de salida
				error = string.Empty;
				// Mientras quede algo de texto por interpretar ...
				while (!string.IsNullOrWhiteSpace(command) && string.IsNullOrWhiteSpace(error))
				{
					int start = command.IndexOf(StartVariable);

						// Interpreta la variable
						if (start >= 0)
						{
							int end;

								// Añade el inicio al resultado
								result += command.Left(start);
								// Quita el token de inicio de la variable del texto
								command = command.From(start + EndVariable.Length);
								// Obtiene el índice de la parte final de la variable
								end = command.IndexOf(EndVariable);
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
												command = command.From(end + EndVariable.Length);
												// y añade el contenido de la variable al resultado
												result += ConvertStringValue(variableValue, format);
										}
								}
								else //... no hay cadena de fin de variable se añade al resultado el comienzo de variable
									result += EndVariable;
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
		private string ConvertStringValue(object value, string format)
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
			else if (string.IsNullOrEmpty(value))
				return "''";
			else
			{
				if (format.EqualsIgnoreCase(FormatNotQuote))
					return value.Replace("'", "''");
				else
					return $"'{value.Replace("'", "''")}'";
			}
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
