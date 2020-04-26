using System;

using Bau.Libraries.LibCommonHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibDbScripts.Manager.Processor.Sentences;
using Bau.Libraries.LibDbScripts.Manager.Processor.Sentences.Parameters;

namespace Bau.Libraries.LibDbScripts.Manager.Processor.Repository
{
	/// <summary>
	///		Clase de lectura de los scripts
	/// </summary>
	internal class DbScriptRepository
	{
		// Constantes privadas
		private const string TagRoot = "DbScript";
		private const string TagImport = "Import";
		private const string TagFileName = "FileName";
		private const string TagSentenceExecute = "Execute";
		private const string TagSentenceException = "Exception";
		private const string TagProvider = "Provider";
		private const string TagSentenceBlock = "Block";
		private const string TagProviderCommand = "Command";
		private const string TagArgument = "Argument";
		private const string TagType = "Type";
		private const string TagName = "Name";
		private const string TagValue = "Value";
		private const string TagParameterName = "ParameterName";
		private const string TagDefault = "Default";
		private const string TagEmptyData = "EmptyData";
		private const string TagWithData = "WithData";
		private const string TagSentenceForEach = "ForEach";
		private const string TagSentenceIf = "If";
		private const string TagCondition = "Condition";
		private const string TagThen = "Then";
		private const string TagElse = "Else";
		private const string TagSentenceString = "String";
		private const string TagSentenceNumeric = "Numeric";
		private const string TagSentenceBoolean = "Boolean";
		private const string TagSentenceDate = "Date";
		private const string TagDateNow = "Now";
		private const string TagSentenceLet = "Let";
		private const string TagVariable = "Variable";
		private const string TagSentenceFor = "For";
		private const string TagStart = "Start";
		private const string TagEnd = "End";
		private const string TagStep = "Step";
		private const string TagSentencePrint = "Print";
		private const string TagSentenceIfExists = "IfExists";
		private const string TagSentenceBeginTransaction = "BeginTransaction";
		private const string TagSentenceCommitTransaction = "CommitTransaction";
		private const string TagSentenceRollbackTransaction = "RollbackTransaction";

		/// <summary>
		///		Carga el programa de un archivo
		/// </summary>
		internal ProgramModel LoadByFile(string fileName)
		{
			return Load(new LibMarkupLanguage.Services.XML.XMLParser().Load(fileName));
		}

		/// <summary>
		///		Carga el programa de un texto
		/// </summary>
		internal ProgramModel LoadByText(string xml)
		{
			return Load(new LibMarkupLanguage.Services.XML.XMLParser().ParseText(xml));
		}

		/// <summary>
		///		Carga el programa
		/// </summary>
		private ProgramModel Load(MLFile fileML)
		{
			ProgramModel program = new ProgramModel();

				// Carga las sentencias del programa
				if (fileML != null)
					foreach (MLNode rootML in fileML.Nodes)
						if (rootML.Name == TagRoot)
							program.Sentences.AddRange(LoadSentences(rootML.Nodes));
				// Devuelve el programa cargado
				return program;
		}

		/// <summary>
		///		Carga las instrucciones de una serie de nodos
		/// </summary>
		private SentenceCollection LoadSentences(MLNodesCollection nodesML)
		{
			SentenceCollection sentences = new SentenceCollection();

				// Lee las instrucciones
				foreach (MLNode rootML in nodesML)
					switch (rootML.Name)
					{
						case TagImport:
								sentences.AddRange(LoadByFile(rootML.Attributes[TagFileName].Value).Sentences);
							break;
						case TagSentenceBlock:
								sentences.Add(LoadSentenceBlock(rootML));
							break;
						case TagSentenceExecute:
								sentences.Add(LoadSentenceExecute(rootML));
							break;
						case TagSentenceException:
								sentences.Add(LoadSentenceException(rootML));
							break;
						case TagSentenceForEach:
								sentences.Add(LoadSentenceForEach(rootML));
							break;
						case TagSentenceIfExists:
								sentences.Add(LoadSentenceIfExists(rootML));
							break;
						case TagSentenceIf:
								sentences.Add(LoadSentenceIf(rootML));
							break;
						case TagSentenceString:
								sentences.Add(LoadSentenceDeclare(rootML, SentenceDeclare.VariableType.String));
							break;
						case TagSentenceNumeric:
								sentences.Add(LoadSentenceDeclare(rootML, SentenceDeclare.VariableType.Numeric));
							break;
						case TagSentenceBoolean:
								sentences.Add(LoadSentenceDeclare(rootML, SentenceDeclare.VariableType.Boolean));
							break;
						case TagSentenceDate:
								sentences.Add(LoadSentenceDeclare(rootML, SentenceDeclare.VariableType.Date));
							break;
						case TagSentenceLet:
								sentences.Add(LoadSentenceLet(rootML));
							break;
						case TagSentenceFor:
								sentences.Add(LoadSentenceFor(rootML));
							break;
						case TagSentencePrint:
								sentences.Add(LoadSentencePrint(rootML));
							break;
						case TagSentenceBeginTransaction:
								sentences.Add(LoadSentenceBatch(rootML, SentenceDataBatch.BatchCommand.BeginTransaction));
							break;
						case TagSentenceCommitTransaction:
								sentences.Add(LoadSentenceBatch(rootML, SentenceDataBatch.BatchCommand.CommitTransaction));
							break;
						case TagSentenceRollbackTransaction:
								sentences.Add(LoadSentenceBatch(rootML, SentenceDataBatch.BatchCommand.RollbackTransaction));
							break;
					}
				// Devuelve la colección
				return sentences;
		}

		/// <summary>
		///		Carga una sentencia de bloque
		/// </summary>
		private SentenceBase LoadSentenceBlock(MLNode rootML)
		{
			SentenceBlock sentence = new SentenceBlock();

				// Carga los datos de la sentencia
				sentence.Name = rootML.Attributes[TagName].Value.TrimIgnoreNull();
				// Carga las sentencias hija
				sentence.Sentences.AddRange(LoadSentences(rootML.Nodes));
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Carga una sentencia de inicio o fin de lote
		/// </summary>
		private SentenceBase LoadSentenceBatch(MLNode rootML, SentenceDataBatch.BatchCommand type)
		{
			return new SentenceDataBatch
							{
								ProviderKey = rootML.Attributes[TagProvider].Value,
								Type = type
							};
		}

		/// <summary>
		///		Carga una sentencia de impresión
		/// </summary>
		private SentenceBase LoadSentencePrint(MLNode rootML)
		{
			SentencePrint sentence = new SentencePrint();

				// Carga el contenido
				sentence.Message = rootML.Value;
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Carga una sentencia de declaración de variables
		/// </summary>
		private SentenceBase LoadSentenceDeclare(MLNode rootML, SentenceDeclare.VariableType type)
		{
			SentenceDeclare sentence = new SentenceDeclare();

				// Asigna las propiedades
				sentence.Type = type;
				sentence.Name = rootML.Attributes[TagName].Value;
				sentence.Value = ConvertStringValue(type, rootML.Attributes[TagValue].Value);
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Carga una sentencia de asignación
		/// </summary>
		private SentenceBase LoadSentenceLet(MLNode rootML)
		{
			SentenceLet sentence = new SentenceLet();

				// Asigna las propiedades
				sentence.Type = rootML.Attributes[TagType].Value.GetEnum(SentenceDeclare.VariableType.Unknown);
				sentence.Variable = rootML.Attributes[TagName].Value;
				sentence.Expression = rootML.Value;
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Carga una sentencia for
		/// </summary>
		private SentenceBase LoadSentenceFor(MLNode rootML)
		{
			SentenceFor sentence = new SentenceFor();

				// Asigna las propiedades
				sentence.Variable = rootML.Attributes[TagVariable].Value;
				sentence.Start = rootML.Attributes[TagStart].Value.GetInt(0);
				sentence.End = rootML.Attributes[TagEnd].Value.GetInt(0);
				sentence.Step = rootML.Attributes[TagStep].Value.GetInt(1);
				// Carga las sentencias
				sentence.Sentences.AddRange(LoadSentences(rootML.Nodes));
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Carga los datos de una sentencia <see cref="SentenceForEach"/>
		/// </summary>
		private SentenceBaseProvider LoadSentenceForEach(MLNode rootML)
		{
			SentenceForEach sentence = new SentenceForEach();
			
				// Carga la sentencia
				LoadSentenceBaseProvider(rootML, sentence);
				// Carga las instrucciones a ejecutar cuando hay o no hay datos
				foreach (MLNode nodeML in rootML.Nodes)
					switch (nodeML.Name)
					{
						case TagEmptyData:
								sentence.SentencesEmptyData.AddRange(LoadSentences(nodeML.Nodes));
							break;
						case TagWithData:
								sentence.SentencesWithData.AddRange(LoadSentences(nodeML.Nodes));
							break;
					}
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Sentencia de ejecución sobre el proveedor
		/// </summary>
		private SentenceBaseProvider LoadSentenceExecute(MLNode rootML)
		{
			return LoadSentenceBaseProvider(rootML, new SentenceExecuteDataCommand());
		}

		/// <summary>
		///		Carga la sentencia que comprueba si existe un valor
		/// </summary>
		private SentenceBase LoadSentenceIfExists(MLNode rootML)
		{
			SentenceIfExists sentence = new SentenceIfExists();

				// Carga los parámetros de la sentencia
				LoadSentenceBaseProvider(rootML, sentence);
				// Carga las instrucciones a ejecutar cuando existe o no existe
				foreach (MLNode nodeML in rootML.Nodes)
					switch (nodeML.Name)
					{
						case TagThen:
								sentence.SentencesThen.AddRange(LoadSentences(nodeML.Nodes));
							break;
						case TagElse:
								sentence.SentencesElse.AddRange(LoadSentences(nodeML.Nodes));
							break;
					}
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Carga los datos básicos de una sentencia para ejecución sobre el proveedor
		/// </summary>
		private SentenceBaseProvider LoadSentenceBaseProvider(MLNode rootML, SentenceBaseProvider sentence)
		{
			// Carga los datos básicos
			sentence.ProviderKey = rootML.Attributes[TagProvider].Value;
			// Carga las propiedades de la sentencia
			foreach (MLNode nodeML in rootML.Nodes)
				switch (nodeML.Name)
				{
					case TagProviderCommand:
							LoadProviderCommand(nodeML, sentence);
						break;
				}
			// Devuelve la sentencia leida
			return sentence;
		}

		/// <summary>
		///		Carga una sentencia que se envía a un proveedor
		/// </summary>
		private void LoadProviderCommand(MLNode rootML, SentenceBaseProvider sentence)
		{
			foreach (MLNode nodeML in rootML.Nodes)
				switch (nodeML.Name)
				{
					case TagArgument:
							sentence.Command.Filters.Add(LoadFilter(nodeML));
						break;
					default:
							sentence.Command.Commands.Add(new ProviderCommandModel
																{
																	Name = nodeML.Name,
																	Value = nodeML.Value
																}
															);
						break;
				}
		}

		/// <summary>
		///		Carga los datos de un filtro
		/// </summary>
		private FilterModel LoadFilter(MLNode rootML)
		{
			FilterModel filter = new FilterModel();

				// Añade los datos del filtro
				filter.ColumnType = rootML.Attributes[TagType].Value.GetEnum(SentenceDeclare.VariableType.Unknown);
				filter.VariableName = rootML.Attributes[TagVariable].Value;
				filter.Parameter = rootML.Attributes[TagParameterName].Value;
				if (string.IsNullOrEmpty(filter.VariableName) && !string.IsNullOrEmpty(filter.Parameter))
					filter.VariableName = filter.Parameter.Replace("@", "");
				// Obtiene el valor por defecto
				filter.Default = ConvertStringValue(filter.ColumnType, rootML.Attributes[TagDefault].Value);
				// Devuelve los datos del filtro
				return filter;
		}

		/// <summary>
		///		Convierte una cadena con un valor
		/// </summary>
		private object ConvertStringValue(SentenceDeclare.VariableType type, string value)
		{
			if (string.IsNullOrEmpty(value))
				return null;
			else
				switch (type)
				{ 
					case SentenceDeclare.VariableType.Boolean:
						return value.GetBool();
					case SentenceDeclare.VariableType.Date:
						if (value.EqualsIgnoreCase(TagDateNow))
							return DateTime.Now;
						else
							return value.GetDateTime();
					case SentenceDeclare.VariableType.Numeric:
						return value.GetDouble();
					default:
						return value;
				}
		}

		/// <summary>
		///		Carga los datos de una sentencia de excepción
		/// </summary>
		private SentenceBase LoadSentenceException(MLNode rootML)
		{
			return new SentenceException
							{
								Message = rootML.Value
							};
		}

		/// <summary>
		///		Carga una sentencia If
		/// </summary>
		private SentenceBase LoadSentenceIf(MLNode rootML)
		{
			SentenceIf sentence = new SentenceIf();

				// Carga la condición
				sentence.Condition = rootML.Attributes[TagCondition].Value;
				// Carga las sentencias de la parte then y else
				foreach (MLNode nodeML in rootML.Nodes)
					switch (nodeML.Name)
					{
						case TagThen:
								sentence.SentencesThen.AddRange(LoadSentences(nodeML.Nodes));
							break;
						case TagElse:
								sentence.SentencesElse.AddRange(LoadSentences(nodeML.Nodes));
							break;
					}
				// Devuelve la sentencia
				return sentence;
		}
	}
}
