using System;
using System.Collections.Generic;
using System.Data;

using Bau.Libraries.LibCommonHelper.Extensors;
using Bau.Libraries.LibReports.Generator.Data;
using Bau.Libraries.LibTokenizer.Variables;

namespace Bau.Libraries.LibReports.Generator.Compiler
{
	/// <summary>
	///		Compilador de texto
	/// </summary>
	internal class TextCompiler
	{ 
		// Constantes privadas
		private const string StartVariable = "{{";
		private const string EndVariable = "}}";
		private const string VariableRowIndex = "RowIndex";
		// Variables privadas
		private VariablesCollection _symbolsGlobal = new VariablesCollection();

		public TextCompiler(Dictionary<string, object> parameters)
		{
			InitSymbolsGlobal(parameters);
		}

		/// <summary>
		///		Inicializa la tabla de símbolos global
		/// </summary>
		private void InitSymbolsGlobal(Dictionary<string, object> parameters)
		{ 
			// Añade las funciones fijas a la tabla de variables global
			_symbolsGlobal.Add("Today", DateTime.Now);
			// Añade los parámetros de entrada
			foreach (KeyValuePair<string, object> parameter in parameters)
				_symbolsGlobal.Add(parameter.Key, parameter.Value?.ToString());
		}

		/// <summary>
		///		Interpreta el texto teniendo en cuenta los datos leídos en el registro y los datos de registros anteriores así como
		///	los formatos y valores
		/// </summary>
		/// <returns>
		///		Por ejemplo podríamos tener un texto como:
		///			En relación con el cliente {{Client}}'
		///	indicando que se sustituyese el valor {{Client}} por alguno de los registros almacenados en la pila y leídos por tanto en secciones padre
		/// </returns>
		internal string ParseText(DataTableReaderRow readerRow, string text)
		{ 
			// Interpreta el texto con los registros de la pila
			if (!text.IsEmpty())
				while (readerRow != null)
				{
					VariablesCollection symbolsLocal = _symbolsGlobal.Clone();

						// Añade el número de fila
						symbolsLocal.Add(VariableRowIndex, readerRow.RowIndex);
						// Añade los datos de las columnas
						foreach (DataColumn dcColumn in readerRow.Row.Table.Columns)
							symbolsLocal.Add(dcColumn.Caption, readerRow.Row[dcColumn]);
						// Añade las variables
						symbolsLocal.AddRange(Interpreter.GetSymbols());
						// Reemplaza el contenido del texto
						text = ReplaceVariables(text, symbolsLocal);
						// Pasa a la fila padre
						readerRow = readerRow.TableReader.Parent?.GetActualRow();
				}
			// Devuelve el texto interpretado
			return text;
		}

		/// <summary>
		///		Obtiene un valor de la tabla de variables o la pila de datos
		/// </summary>
		internal string GetValue(DataTableReaderRow readerRow, string field)
		{
			VariablesCollection variables = GetSymbolsTableLocal(readerRow);

				// Obtiene el valor
				return GetValue(variables, field);
		}

		/// <summary>
		///		Obtiene un valor de una tabla de variables
		/// </summary>
		internal string GetValue(VariablesCollection variables, string field)
		{
			string value = "";

				// Obtiene el valor de la variable
				if (!field.IsEmpty())
				{
					Variable variable = variables.Search(field);

						// Obtiene el contenido de la variable
						if (variable != null && !(variable.Value is ValueNull))
							value = variable.Value.Content;
				}
				// Devuelve el valor de la variable
				return value;
		}

		/// <summary>
		///		Obtiene la tabla de símbolos local
		/// </summary>
		internal VariablesCollection GetSymbolsTableLocal(DataTableReaderRow readerRow)
		{
			VariablesCollection symbolsLocal = _symbolsGlobal.Clone();

				// Añade el número de fila
				if (readerRow != null)
					symbolsLocal.Add(VariableRowIndex, readerRow.RowIndex);
				// Añade los datos de la fila y de las filas de la pila a la tabla de símbolos local
				while (readerRow != null)
				{ 
					// Añade los datos de las columnas
					foreach (DataColumn column in readerRow.Row.Table.Columns)
						symbolsLocal.Add(column.Caption, readerRow.Row[column]);
					// Pasa al elemento padre
					readerRow = readerRow.TableReader.Parent?.GetActualRow();
				}
				// Devuelve la tabla de símbolos local
				return symbolsLocal;
		}

		/// <summary>
		///		Reemplaza el texto de un registro
		/// </summary>
		private string ReplaceVariables(string text, VariablesCollection variables)
		{
			string result = "";

				// Mientras quede algo de texto por interpretar ...
				while (!text.IsEmpty())
				{
					int start = text.IndexOf(StartVariable);

						// Interpreta la variable
						if (start >= 0)
						{
							int end;

								// Añade el inicio al resultado
								result += text.Left(start);
								// Quita el token de inicio de la variable del texto
								text = text.From(start + EndVariable.Length);
								// Obtiene el índice de la parte final de la variable
								end = text.IndexOf(EndVariable);
								// Si se ha encontrado un nombre de variable
								if (end >= 0)
								{
									string variable = text.Left(end);
									string format = GetFormat(variable);
									bool found = false;

										// Quita el nombre de la variable del texto
										text = text.From(end + EndVariable.Length);
										// Si se ha encontrado un formato se quita del nombre de la cadena
										if (!format.IsEmpty())
											variable = variable.Left(variable.Length - format.Length - 1);
										// Interpreta la cadena
										foreach (Variable symbol in variables)
											if (!found && (variable.EqualsIgnoreCase(symbol.Name) ||
														   $"@{variable}".EqualsIgnoreCase(symbol.Name)))
											{ 
												// Añade el valor de la variable
												result += ParseVariable(symbol.Value, format);
												// Indica que se ha encontrado la variable
												found = true;
											}
										// Si se ha encontrado, se añade el mismo valor al resultando
										if (!found)
										{ 
											// Primero la variable
											result += StartVariable + variable;
											// Después el formato
											if (!format.IsEmpty())
												result += ":" + format;
											// ... y por último el fin de variable
											result += EndVariable;
										}
								}
								else //... no hay cadena de fin de variable se añade al resultado el comienzo de variable
									result += StartVariable;
						}
						else
						{
							result += text;
							text = "";
						}
				}
				// Devuelve el contenido
				return result;
		}

		/// <summary>
		///		Obtiene el formato asociado a una variable
		/// </summary>
		private string GetFormat(string variable)
		{
			string format = "";
			int startIndex = variable.IndexOf(":");

				// Obtiene el formato
				if (startIndex >= 0)
					format = variable.From(startIndex + 1).TrimIgnoreNull();
				// Devuelve el formato encontrado
				return format;
		}

		/// <summary>
		///		Interpreta el valor de una variable
		/// </summary>
		private string ParseVariable(ValueBase value, string format)
		{
			if (value == null)
				return " ";
			else if (!format.IsEmpty())
				return string.Format("{0:" + format + "}", value.InnerValue);
			else
				return value.Content;
		}

		/// <summary>
		///		Evalúa un código
		/// </summary>
		internal void Evaluate(DataTableReaderRow readerRow, string code)
		{
			Interpreter.Execute(GetSymbolsTableLocal(readerRow), code);
		}

		/// <summary>
		///		Comprueba si la condición es verdadera
		/// </summary>
		internal bool EvaluateCondition(DataTableReaderRow readerRow, string code)
		{
			return Interpreter.EvaluateCondition(GetSymbolsTableLocal(readerRow), code);
		}

		/// <summary>
		///		Intérprete de ejecución de comandos 
		/// </summary>
		private Execution.Interpreter Interpreter { get; } = new Execution.Interpreter();
	}
}
