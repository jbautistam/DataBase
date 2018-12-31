using System;
using System.Collections.Generic;
using System.Data;

using Bau.Libraries.Aggregator.Providers.Base;
using Bau.Libraries.Compiler.LibInterpreter.Context;
using Bau.Libraries.Compiler.LibInterpreter.Context.Variables;
using Bau.Libraries.LibDbScripts.Generator.Processor.Context;
using Bau.Libraries.LibDbScripts.Generator.Processor.Sentences;
using Bau.Libraries.LibDbScripts.Generator.Processor.Sentences.Parameters;

namespace Bau.Libraries.LibDbScripts.Generator.Processor
{
	/// <summary>
	///		Clase para lectura y relleno de datos de un informe
	/// </summary>
	internal class DbScriptProcessor
	{   
		internal DbScriptProcessor(DbScriptManager generator, ProgramModel program)
		{
			Generator = generator;
			Program = program;
		}

		/// <summary>
		///		Procesa el programa
		/// </summary>
		internal void Process()
		{
			// Inicializa el contexto y las transacciones
			Context.Clear();
			Context.Add();
			Transactions.Clear();
			// Añade las variables iniciales
			foreach (KeyValuePair<string, object> keyValue in Generator.Parameters)
				Context.Actual.Variables.Add(keyValue.Key, keyValue.Value);
			Context.Actual.Variables.Add("Today", DateTime.Now);
			// Ejecuta el programa
			Execute(Program.Sentences);
		}

		/// <summary>
		///		Ejecuta una serie de sentencias
		/// </summary>
		private void Execute(SentenceCollection sentences)
		{
			foreach (SentenceBase abstractSentence in sentences)
				if (!Stopped)
					switch (abstractSentence)
					{
						case SentenceDeclare sentence:
								ExecuteDeclare(sentence);
							break;
						case SentenceLet sentence:
								ExecuteLet(sentence);
							break;
						case SentenceException sentence:
								ExecuteException(sentence);
							break;
						case SentenceFor sentence:
								ExecuteFor(sentence);
							break;
						case SentenceIf sentence:
								ExecuteIf(sentence);
							break;
						case SentenceForEach sentence:
								ExecuteForEach(sentence);
							break;
						case SentenceIfExists sentence:
								ExecuteIfExists(sentence);
							break;
						case SentenceExecuteDataCommand sentence:
								ExecuteDataCommand(sentence);
							break;
						case SentencePrint sentence:
								ExecutePrint(sentence);
							break;
						case SentenceDataBatch sentence:
								ExecuteDataBatch(sentence);
							break;
					}
		}

		/// <summary>
		///		Ejecuta una serie de sentencias creando un contexto nuevo
		/// </summary>
		private void ExecuteWithContext(SentenceCollection sentences)
		{
			// Crea el contexto
			Context.Add();
			// Ejecuta las sentencias
			Execute(sentences);
			// Elimina el contexto
			Context.Pop();
		}

		/// <summary>
		///		Ejecuta una sentencia de declaración
		/// </summary>
		private void ExecuteDeclare(SentenceDeclare sentence)
		{
			// Ejecuta la sentencia
			Context.Actual.Variables.Add(sentence.Name, sentence.Value);
			// Debug
			AddDebug($"Declare {sentence.Name} =", sentence.Value);
		}

		/// <summary>
		///		Ejecuta una sentencia de asignación
		/// </summary>
		private void ExecuteLet(SentenceLet sentence)
		{
			if (string.IsNullOrWhiteSpace(sentence.Variable))
				AddError("Falta el nombre de la variable");
			else
			{
				VariableModel variable = Context.Actual.Variables.Get(sentence.Variable);

					// Depuración
					AddDebug($"Let {sentence.Variable} = {sentence.Expression}");
					// Declara la variable si no existía
					if (variable == null && sentence.Type != SentenceDeclare.VariableType.Unknown)
					{
						// Añade una variable con el valor predeterminado
						Context.Actual.Variables.Add(sentence.Variable, sentence.Type);
						// Obtiene la variable
						variable = Context.Actual.Variables.Get(sentence.Variable);
					}
					// Ejecuta la sentencia
					if (variable == null)
						AddError($"Variable {sentence.Variable} not declared");
					else
					{
						VariableModel result = new Compiler.Interpreter().EvaluateExpression(Context, sentence.Expression, out string error);

							if (!string.IsNullOrWhiteSpace(error))
								AddError($"Error when evaluate expression {sentence.Expression}. {error}");
							else
								variable.Value = result.Value;
					}
			}
		}

		/// <summary>
		///		Ejecuta una sentencia for
		/// </summary>
		private void ExecuteFor(SentenceFor sentence)
		{
			// Normaliza el inicio, fin e incremento de la sentencia
			sentence.Normalize();
			// Ejecuta el bucle for
			for (int index = sentence.Start; 
					 (index < sentence.End && sentence.Step > 0) || (index > sentence.End && sentence.Step < 0); 
					 index += sentence.Step)
			{
				// Abre un nuevo contexto
				Context.Add();
				// Añade la variable al contexto
				Context.Actual.Variables.Add(sentence.Variable, index);
				// Ejecuta las sentencias
				Execute(sentence.Sentences);
				// Elimina el contexto
				Context.Pop();
			}
		}

		/// <summary>
		///		Ejecuta una sentencia de excepción
		/// </summary>
		private void ExecuteException(SentenceException sentence)
		{
			AddError(sentence.Message);
		}

		/// <summary>
		///		Ejecuta la sentencia de impresión
		/// </summary>
		private void ExecutePrint(SentencePrint sentence)
		{
			string text = new Compiler.Interpreter().EvaluateText(Context.Actual, sentence.Message, out string error);

				// Log
				AddDebug($"Print: {sentence.Message}");
				// Añade el resultado de la sentencia
				if (!string.IsNullOrWhiteSpace(error))
					AddError($"Error when execute print: {error}");
				else
					AddInfo(text);
		}

		/// <summary>
		///		Ejecuta una sentencia condicional
		/// </summary>
		private void ExecuteIf(SentenceIf sentence)
		{
			if (string.IsNullOrWhiteSpace(sentence.Condition))
				AddError("No se encuentra ninguna condición");
			else 
			{
				bool result = new Compiler.Interpreter().EvaluateCondition(Context, sentence.Condition, out string error);

					if (!string.IsNullOrEmpty(error))
						AddError(error);
					else if (result && sentence.SentencesThen.Count > 0)
						ExecuteWithContext(sentence.SentencesThen);
					else if (!result && sentence.SentencesElse.Count > 0)
						ExecuteWithContext(sentence.SentencesElse);
			}
		}

		/// <summary>
		///		Ejecuta una sentencia foreach
		/// </summary>
		private void ExecuteForEach(SentenceForEach sentence)
		{
			IDataProvider provider = Generator.DataProviders[sentence.ProviderKey];

				if (provider == null)
					AddError($"No se encuentra el proveedor {sentence.ProviderKey}");
				else
				{
					DataProviderCommand command = ConvertProviderCommand(sentence.Command, out string error);

						if (!string.IsNullOrWhiteSpace(error))
							AddError($"Error when convert command. {error}");
						else
						{
							int startRow = 0;

								// Ejecuta la consulta sobre el proveedor
								try
								{
									foreach (DataTable table in GetDataTable(provider, command))
										if (table.Rows.Count > 0)
										{
											// Ejecuta las instrucciones
											foreach (DataRow row in table.Rows)
											{
												// Crea un contexto
												Context.Add();
												// Añade el índice de fila a la lista de variables
												Context.Actual.Variables.Add("RowIndex", startRow + table.Rows.IndexOf(row));
												// Añade las columnas
												for (int index = 0; index < table.Columns.Count; index++)
													if (row.IsNull(index) || row[index] == null)
														Context.Actual.Variables.Add(table.Columns[index].ColumnName, null);
													else
														Context.Actual.Variables.Add(table.Columns[index].ColumnName, row[index]);
												// Ejecuta las sentencias
												Execute(sentence.SentencesWithData);
												// Limpia el contexto
												Context.Pop();
											}
											// Añade el total de filas
											startRow += table.Rows.Count;
										}
								}
								catch (Exception exception)
								{
									AddError($"Error al cargar los datos. {exception.Message}");
								}
								// Si no se han encontrado datos, ejecuta las sentencias adecuadas
								if (startRow == 0)
									Execute(sentence.SentencesEmptyData);
						}
				}
		}

		/// <summary>
		///		Ejecuta la sentencia que comprueba si existe un valor en la tabla de datos
		/// </summary>
		private void ExecuteIfExists(SentenceIfExists sentence)
		{
			IDataProvider provider = Generator.DataProviders[sentence.ProviderKey];

				if (provider == null)
					AddError($"No se encuentra el proveedor {sentence.ProviderKey}");
				else
				{
					DataProviderCommand command = ConvertProviderCommand(sentence.Command, out string error);

						// Log
						AddDebug("ExecuteIfExists", sentence.Command);
						// Ejecuta el comando
						if (!string.IsNullOrWhiteSpace(error))
							AddError($"Error when convert command. {error}");
						else 
						{	
							IEnumerator<DataTable> tableEnumerator = GetDataTable(provider, command).GetEnumerator();

								if (tableEnumerator.MoveNext())
									try
									{
										DataTable table = tableEnumerator.Current;

											if (table.Rows.Count > 0 && sentence.SentencesThen.Count > 0)
												ExecuteWithContext(sentence.SentencesThen);
											else if (table.Rows.Count == 0 && sentence.SentencesElse.Count > 0)
												ExecuteWithContext(sentence.SentencesElse);
									}
									catch (Exception exception)
									{
										AddError($"Error al cargar los datos. {exception.Message}");
									}
								else if (sentence.SentencesElse.Count > 0) // ... no había ningún recordset para la tabla
									ExecuteWithContext(sentence.SentencesElse);
						}
				}
		}

		/// <summary>
		///		Obtiene la tabla de datos
		/// </summary>
		private IEnumerable<DataTable> GetDataTable(IDataProvider provider, DataProviderCommand command)
		{
			int pageIndex = 0;

				// Carga los datos
				foreach (DataTable table in provider.LoadData(command))
				{
					// Log
					AddDebug($"Reading page {++pageIndex}", command);
					// Devuelve la tabla
					yield return table;
				}
		}

		/// <summary>
		///		Ejecuta una sentencia de ejecución de comando de datos sobre el proveedor
		/// </summary>
		private void ExecuteDataCommand(SentenceExecuteDataCommand sentence)
		{
			IDataProvider provider = Generator.DataProviders[sentence.ProviderKey];

				if (provider == null)
					AddError($"Can't find the provider {sentence.ProviderKey}");
				else
				{
					string error = string.Empty;
					DataProviderCommand command = ConvertProviderCommand(sentence.Command, out error);

						// Log
						AddDebug("Execute", sentence.Command);
						// Ejecuta el comando
						if (!string.IsNullOrEmpty(error))
							AddError(error);
						else if (Transactions.Exists(provider))
						{
							AddDebug("Add command to batch", command);
							Transactions.Add(provider, command);
						}
						else
						{
							AddDebug("Execute (converted)", command);
							try
							{
								provider.Execute(command);
							}
							catch (Exception exception)
							{
								AddError($"Error when execute command. {exception.Message}");
							}
						}
				}
		}

		/// <summary>
		///		Convierte el comando del proveedor
		/// </summary>
		private DataProviderCommand ConvertProviderCommand(ProviderSentenceModel sentence, out string error)
		{
			DataProviderCommand command = new DataProviderCommand();

				// Inicializa los argumentos de salida
				error = string.Empty;
				// Añade los comandos
				foreach (ProviderCommandModel sentenceCommand in sentence.Commands)
					if (string.IsNullOrWhiteSpace(error))
						command.Sentences.Add(sentenceCommand.Name, 
											  new Compiler.Interpreter().EvaluateCommand(Context.Actual, sentenceCommand.Value, out error));
				// Añade los filtros
				if (string.IsNullOrWhiteSpace(error))
					foreach (FilterModel filter in sentence.Filters)
						command.Parameters.Add(filter.Parameter, Context.Actual.Variables.Get(filter.VariableName).Value ?? filter.Default);
				// Devuelve el comando del proveedor
				return command;
		}

		/// <summary>
		///		Ejecuta una sentencia de lote
		/// </summary>
		private void ExecuteDataBatch(SentenceDataBatch sentence)
		{
			switch (sentence.Type)
			{
				case SentenceDataBatch.BatchCommand.BeginTransaction:
						ExecuteBeginTransaction(sentence.ProviderKey);
					break;
				case SentenceDataBatch.BatchCommand.CommitTransaction:
						ExecuteCommitTransaction(sentence.ProviderKey);
					break;
				case SentenceDataBatch.BatchCommand.RollbackTransaction:
						ExecuteRollbackTransaction(sentence.ProviderKey);
					break;
			}
		}

		/// <summary>
		///		Abre una transacción
		/// </summary>
		private void ExecuteBeginTransaction(string providerKey)
		{
			IDataProvider provider = Generator.DataProviders[providerKey];

				// Abre las transacciones para el proveedor
				if (provider == null)
					AddError($"Can't find the provider {providerKey}");
				else
				{
					// Log
					AddDebug($"Open transaction for provider {providerKey}");
					// Añade la transacción
					Transactions.Add(provider);
				}
		}

		/// <summary>
		///		Ejecuta el commit de una transacción
		/// </summary>
		private void ExecuteCommitTransaction(string providerKey)
		{
			BatchTransactionModel transaction = Transactions.Get(providerKey);

				if (transaction == null)
					AddError($"Can't find an open transaction for provider {providerKey}");
				else
				{
					// Log
					AddDebug($"Start batch execution for provider {providerKey}");
					// Ejecuta los comandos
					try
					{
						transaction.Provider.Execute(transaction.Commands);
					}
					catch (Exception exception)
					{
						AddError($"Error when execute commands batch. {exception.Message}");
					}
					// Borra la transacción
					Transactions.Remove(providerKey);
				}
		}

		/// <summary>
		///		Ejecuta el rollback de una transacción
		/// </summary>
		private void ExecuteRollbackTransaction(string providerKey)
		{
			BatchTransactionModel transaction = Transactions.Get(providerKey);

				if (transaction == null)
					AddError($"Can't find an open transaction for provider {providerKey}");
				else
				{
					// Log
					AddDebug($"Rollback transaction for provider {providerKey}");
					// Elimina la transacción
					Transactions.Remove(providerKey);
				}

		}

		/// <summary>
		///		Añade un mensaje de depuración
		/// </summary>
		private void AddDebug(string message)
		{
			Generator.RaiseMessage(message, EventArguments.MessageEventArgs.MessageType.Debug);
		}

		/// <summary>
		///		Añade el mensaje de depuración de una sentencia de comando sobre el proveedor
		/// </summary>
		private void AddDebug(string header, DataProviderCommand command)
		{
			AddDebug(header);
			foreach (KeyValuePair<string, string> sentence in command.Sentences)
				AddDebug($"{sentence.Key}: {sentence.Value}");
		}

		/// <summary>
		///		Añade la depuración de una sentencia
		/// </summary>
		private void AddDebug(string header, ProviderSentenceModel command)
		{
			AddDebug(header);
			foreach (ProviderCommandModel sentenceCommand in command.Commands)
				AddDebug($"Name {sentenceCommand.Name}. Value: {sentenceCommand.Value}");
		}

		/// <summary>
		///		Añade un valor a la depuración
		/// </summary>
		private void AddDebug(string header, object value)
		{
			AddDebug($"{header} {ConvertObjectValue(value)}");
		}

		/// <summary>
		///		Convierte el valor de un objeto a una cadena para depuración
		/// </summary>
		private string ConvertObjectValue(object value)
		{
			if (value == null)
				return "null";
			else
				return value.ToString();
		}

		/// <summary>
		///		Añade un mensaje informativo
		/// </summary>
		private void AddInfo(string message)
		{
			Generator.RaiseMessage(message, EventArguments.MessageEventArgs.MessageType.Info);
		}

		/// <summary>
		///		Añade un error y detiene la compilación si es necesario
		/// </summary>
		private void AddError(string error)
		{
			// Añade el mensaje de error
			Generator.RaiseMessage(error, EventArguments.MessageEventArgs.MessageType.Error);
			// Detiene la compilación
			Stopped = true;
		}

		/// <summary>
		///		Generador utilizado en la conversión
		/// </summary>
		private DbScriptManager Generator { get; }

		/// <summary>
		///		Programa a ejecutar
		/// </summary>
		private ProgramModel Program { get; }

		/// <summary>
		///		Contexto de ejecución
		/// </summary>
		private ContextStackModel Context { get; } = new ContextStackModel();

		/// <summary>
		///		Transacciones activas en la ejecución del script
		/// </summary>
		private BatchTransactionModelCollection Transactions { get; } = new BatchTransactionModelCollection();

		/// <summary>
		///		Indica si se ha detenido el programa por una excepción
		/// </summary>
		private bool Stopped { get; set; }
	}
}
