using System;
using System.Collections.Generic;
using System.Data;

using Bau.Libraries.Aggregator.Providers.Base;
using Bau.Libraries.Aggregator.Providers.Base.Builders;
using Bau.Libraries.LibCommonHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibReports.Generator.Data;

namespace Bau.Libraries.LibReports.Generator
{
	/// <summary>
	///		Clase para lectura y relleno de datos de un informe
	/// </summary>
	internal class ReportFiller
	{   
		// Constantes privadas
		private const string TagImport = "Import";
		private const string TagFileName = "FileName";
		private const string TagLoadData = "LoadData";
		private const string TagProvider = "Provider";
		private const string TagCommand = "Command";
		private const string TagParameter = "Parameter";
		private const string TagType = "Type";
		private const string TagName = "Name";
		private const string TagField = "Field";
		private const string TagDefault = "Default";
		private const string TagErrorData = "ErrorData";
		private const string TagEmptyData = "EmptyData";
		private const string TagWithData = "WithData";
		private const string TagForEach = "ForEach";
		private const string TagCode = "Code";
		private const string TagIf = "If";
		private const string TagCondition = "Condition";
		private const string TagThen = "Then";
		private const string TagElse = "Else";
		// Variables privadas
		private DataTableStack _tablesStack = null;

		internal ReportFiller(ReportGenerator generator)
		{
			Generator = generator;
			_tablesStack = new DataTableStack(generator.Parameters);
			Compiler = new Compiler.TextCompiler(generator.Parameters);
		}

		/// <summary>
		///		Transforma la definición de un informe rellenándola con los datos de los proveedores
		/// </summary>
		internal MLFile Convert(MLFile fileSourceML)
		{
			MLFile fileTargetML = new MLFile();

				// Convierte los nodos
				fileTargetML.Nodes.AddRange(ConvertNodes(fileSourceML.Nodes));
				// Devuelve el informe
				return fileTargetML;
		}

		/// <summary>
		///		Convierte una serie de nodos
		/// </summary>
		private MLNodesCollection ConvertNodes(MLNodesCollection nodesML)
		{
			MLNodesCollection targetML = new MLNodesCollection();

				// Convierte una serie de nodos
				foreach (MLNode nodeML in nodesML)
					if (Generator.Manager.Errors.Count == 0)
						targetML.AddRange(ConvertNode(nodeML));
				// Devuelve la colección de nodos
				return targetML;
		}

		/// <summary>
		///		Convierte un nodo
		/// </summary>
		private MLNodesCollection ConvertNode(MLNode sourceML)
		{
			switch (sourceML.Name)
			{
				case TagImport:
					return ConvertImportNode(sourceML);
				case TagLoadData:
					return ConvertLoadDataNode(sourceML);
				case TagForEach:
					return ConvertForEachNode(sourceML);
				case TagCode:
					return ConvertCodeNode(sourceML);
				case TagIf:
					return ConvertIfNode(sourceML);
				default:
					return new MLNodesCollection 
									{ 
										ConvertDefaultNode(sourceML) 
									};
			}
		}

		/// <summary>
		///		Convierte un nodo sin procesamiento
		/// </summary>
		private MLNode ConvertDefaultNode(MLNode sourceML)
		{
			MLNode targetML = new MLNode(sourceML.Name);

				// Clona los atributos
				targetML.Attributes.AddRange(ConvertAttributes(sourceML.Attributes));
				// Convierte el contenido
				ConvertContentNode(targetML, sourceML);
				// Devuelve el nodo
				return targetML;
		}

		/// <summary>
		///		Convierte el contenido de un nodo (colección de hijos o valor de texto)
		/// </summary>
		private void ConvertContentNode(MLNode targetML, MLNode sourceML)
		{
			if (sourceML.Nodes.Count == 0)
				targetML.Value = ConvertText(sourceML.Value);
			else
				targetML.Nodes.AddRange(ConvertNodes(sourceML.Nodes));
		}

		/// <summary>
		///		Convierte una colección de atributos
		/// </summary>
		private MLAttributesCollection ConvertAttributes(MLAttributesCollection attributesML)
		{
			MLAttributesCollection targetML = new MLAttributesCollection();

				// Clona los atributos
				foreach (MLAttribute attributeML in attributesML)
					targetML.Add(ConvertAttribute(attributeML));
				// Devuelve la colección resultante
				return targetML;
		}

		/// <summary>
		///		Convierte un atributo
		/// </summary>
		private MLAttribute ConvertAttribute(MLAttribute attributeML)
		{
			return new MLAttribute
							{
								Name = attributeML.Name,
								Prefix = attributeML.Prefix,
								Value = ConvertText(attributeML.Value)
							};
		}

		/// <summary>
		///		Convierte un nodo de importación
		/// </summary>
		private MLNodesCollection ConvertImportNode(MLNode sourceML)
		{
			MLNodesCollection targetML = new MLNodesCollection();
			string fileName = sourceML.Attributes[TagFileName].Value;

				// Importa los nodos
				if (fileName.IsEmpty())
					Generator.Manager.Errors.Add($"El nodo {sourceML.Name} no contiene el atributo {TagFileName}");
				else
				{
					MLFile fileML = LoadXmlFile(System.IO.Path.Combine(Generator.PathBase, fileName));

						if (fileML != null)
							targetML = ConvertNodes(fileML.Nodes);
				}
				// Devuelve la colección de nodos
				return targetML;
		}

		/// <summary>
		///		Carga un archivo XML
		/// </summary>
		private MLFile LoadXmlFile(string fileName)
		{
			MLFile fileML = null;

				// Carga el archivo
				if (!System.IO.File.Exists(fileName))
					Generator.Manager.Errors.Add($"No se encuentra el archivo {fileName}");
				else 
				{
					fileML = new LibMarkupLanguage.Services.XML.XMLParser().Load(fileName, out string error);
					if (!string.IsNullOrEmpty(error))
						Generator.Manager.Errors.Add($"Error al cargar el archivo {fileName}.{Environment.NewLine}{error}");
				}
				// Devuelve el archivo
				return fileML;
		}

		/// <summary>
		///		Convierte un texto
		/// </summary>
		private string ConvertText(string text)
		{
			return Compiler.ParseText(_tablesStack.GetActualRow(), text);
		}

		/// <summary>
		///		Convierte una llamada a proveedor en una serie de datos
		/// </summary>
		private MLNodesCollection ConvertLoadDataNode(MLNode sourceML)
		{
			MLNodesCollection targetML = new MLNodesCollection();
			string providerKey = sourceML.Attributes[TagProvider].Value;

				// Carga los datos y procesa
				if (providerKey.IsEmpty())
					Generator.Manager.Errors.Add($"No se encuentra el valor del atributo {TagProvider} en la etiqueta {sourceML.Name}");
				else
				{
					IDataProvider provider = Generator.Manager.DataProviders[providerKey];

						if (provider == null)
							Generator.Manager.Errors.Add($"No se ha definido ningún proveedor con la clave {providerKey}");
						else
						{
							long rows = 0;
							string error = string.Empty;

								try
								{
									IEnumerator<DataTable> enumerator = provider.LoadData(GetCommand(sourceML.Nodes[TagCommand])).GetEnumerator();

										while (enumerator.MoveNext())
										{
											DataTable table = enumerator.Current;

												// Añade el número de filas
												if (table != null)
												{
													// Incrementa el número de filas leidas
													rows += table.Rows.Count;
													// Añade la tabla de datos a la pila
													_tablesStack.Add(table);
													// Convierte los nodos
													targetML.AddRange(ConvertNodes(sourceML.Nodes[TagWithData].Nodes));
													// Elimina la tabla de datos de la pila (porque puede que no tengamos forEach)
													_tablesStack.Remove(table);
												}
										}
								}
								catch (Exception exception)
								{
									error = $"Error al cargar los datos del proveedor {providerKey}.{Environment.NewLine}.{exception.Message}";
								}
								// Si no se ha leido nada, añade los nodos de error o de datos vacíos
								if (!string.IsNullOrEmpty(error))
								{
									if (!sourceML.Attributes[TagErrorData].Value.IsEmpty())
										Generator.Manager.Errors.Add(sourceML.Attributes[TagErrorData].Value);
									else
										Generator.Manager.Errors.Add(error);
								}
								else if (rows == 0)
									targetML.AddRange(ConvertNodes(sourceML.Nodes[TagEmptyData].Nodes));
						}
				}
				// Devuelve la colección de datos
				return targetML;
		}

		/// <summary>
		///		Obtiene la tabla de datos
		/// </summary>
		private DataProviderCommand GetCommand(MLNode commandML)
		{
			DataProviderCommandBuilder builder = new DataProviderCommandBuilder();

				// Asigna los parámetros iniciales al comando
				foreach (KeyValuePair<string, object> parameter in Generator.Parameters)
					builder.WithParameter(parameter.Key, parameter.Value);
				// Interpreta los parámetros del proveedor
				foreach (MLNode nodeML in commandML.Nodes)
					switch (nodeML.Name)
					{
						case TagParameter:
								if (!string.IsNullOrWhiteSpace(nodeML.Attributes[TagField].Value))
									builder.WithParameter(nodeML.Attributes[TagName].Value, 
														  _tablesStack.GetRowValue(nodeML.Attributes[TagField].Value));
								else
									builder.WithParameter(nodeML.Attributes[TagName].Value,
														  nodeML.Attributes[TagType].Value.GetEnum(DataProviderCommandBuilder.ParameterType.String),
														  nodeML.Attributes[TagField].Value,
														  nodeML.Attributes[TagDefault].Value);
							break;
						default:
								builder.WithSentence(nodeML.Name, nodeML.Value);
							break;
					}
				// Devuelve el comando
				return builder.Build();
		}

		/// <summary>
		///		Convierte un nodo foreach con los datos de la tabla actual
		/// </summary>
		private MLNodesCollection ConvertForEachNode(MLNode sourceML)
		{
			MLNodesCollection nodesML = new MLNodesCollection();

				// Carga los nodos
				if (_tablesStack.IsEmpty())
					Generator.Manager.Errors.Add($"No hay ningún origen de datos para ejecutar el bucle {sourceML.Name}");
				else // ... recorre las filas de la última tabla en la pila
					foreach (DataTableReaderRow row in _tablesStack.GetRows())
						foreach (MLNode loopItemML in sourceML.Nodes)
							nodesML.AddRange(ConvertNode(loopItemML));
				// Devuelve la colección de nodos
				return nodesML;
		}

		/// <summary>
		///		Convierte un nodo de código
		/// </summary>
		/// <remarks>
		///		El código del nodo simplemente modifica el estado interno del informe pero no devuelve nodos
		/// </remarks>
		private MLNodesCollection ConvertCodeNode(MLNode sourceML)
		{
			// Evalúa el código del nodo
			if (sourceML.Nodes.Count != 0)
				Generator.Manager.Errors.Add("Los nodos de código sólo pueden contener instrucciones");
			else if (!sourceML.Value.IsEmpty())
				Compiler.Evaluate(_tablesStack.GetActualRow(), sourceML.Value);
			// Devuelve una colección vacía de nodos, simplemente cambia el estado interno del informe
			return new MLNodesCollection();
		}

		/// <summary>
		///		Convierte un nodo condicional
		/// </summary>
		private MLNodesCollection ConvertIfNode(MLNode sourceML)
		{
			MLNodesCollection targetML = new MLNodesCollection();

				// Evalúa la condición y obtiene los nodos
				if (sourceML.Attributes[TagCondition].Value.IsEmpty())
					Generator.Manager.Errors.Add($"No se encuentre el atributo {TagCondition} en el nodo {sourceML.Name}");
				else if (Compiler.EvaluateCondition(_tablesStack.GetActualRow(), sourceML.Attributes[TagCondition].Value))
				{
					if (sourceML.Nodes[TagThen].Nodes.Count == 0)
						Generator.Manager.Errors.Add($"No se encuentra el nodo {TagThen} en el nodo {sourceML.Name}");
					else
						targetML = ConvertNodes(sourceML.Nodes[TagThen].Nodes);
				}
				else if (sourceML.Nodes[TagElse].Nodes.Count > 0)
					targetML = ConvertNodes(sourceML.Nodes[TagElse].Nodes);
				// Devuelve los nodos
				return targetML;
		}

		/// <summary>
		///		Compilador de datos
		/// </summary>
		private Compiler.TextCompiler Compiler { get; }

		/// <summary>
		///		Generador utilizado en la conversión
		/// </summary>
		private ReportGenerator Generator { get; }
	}
}
