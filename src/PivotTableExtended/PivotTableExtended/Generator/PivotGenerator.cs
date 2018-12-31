using System;
using System.Data;

using Bau.Libraries.LibCommonHelper.Extensors;
using Bau.Libraries.PivotTableExtended.Definitions;
using Bau.Libraries.PivotTableExtended.Results;

namespace Bau.Libraries.PivotTableExtended.Generator
{
	/// <summary>
	///		Generador de la tabla pivot
	/// </summary>
	internal class PivotGenerator
	{
		internal PivotGenerator(GroupModelCollection objColGroups)
		{ Groups = objColGroups;
			GroupsRows = Groups.GetGroups(GroupModel.GroupType.Row);
			GroupsColumns = Groups.GetGroups(GroupModel.GroupType.Column);
			GroupsCells = Groups.GetGroups(GroupModel.GroupType.Cell);
		}

		/// <summary>
		///		Genera la tabla pivot a partir de una tabla de datos
		/// </summary>
		internal RawPivotTable Compute(DataTable dtTable)
		{	RawPivotTable objTable = new RawPivotTable(Groups);

				// Genera los árboles de etiquetas de la tabla
					foreach (DataRow drRow in dtTable.Rows)
						{ AddTreeLabels(objTable, true, GroupsRows, drRow);
							AddTreeLabels(objTable, false, GroupsColumns, drRow);
						}
				// Añade las etiquetas de celda
					AddLabelsGroupCell(objTable);
				// Calcula las celdas
					ComputeCells(objTable, dtTable);
				// Ordena las etiquetas (antes de hacer todas las sumas para que los totales aparezcan al final)
					objTable.RowsLabels.SortByTitle();
					objTable.ColumnsLabels.SortByTitle();
				// Calcula los totales
					ComputeTotalsColumn(objTable);
					//ComputeTotalsRow(objTable);
				// Ordena las etiquetas
					objTable.RowsLabels.SortByTitle();
					objTable.ColumnsLabels.SortByTitle();
				// Devuelve la tabla
					return objTable;
		}

		/// <summary>
		///		Añade las etiquetas de una fila por orden
		/// </summary>
		private void AddTreeLabels(RawPivotTable objTable, bool blnRows, GroupModelCollection objColGroups, DataRow drRow)
		{ LabelModel objActualLabel = null;
			LabelModel objTotalLabel = null;
			LabelModelCollection objColLabels = null;

				// Crea la colección de etiquetas
					for (int intIndex = 0; intIndex < objColGroups.Count; intIndex++)
						{ // Si estamos en el primer grupo
								if (objColLabels == null)
									{ if (blnRows)
											{ if (objTable.RowsLabels == null)
													objTable.RowsLabels = new LabelModelCollection(objColGroups[intIndex]);
												objColLabels = objTable.RowsLabels;
											}
										else
											{ if (objTable.ColumnsLabels == null)
													objTable.ColumnsLabels = new LabelModelCollection(objColGroups[intIndex]);
												objColLabels = objTable.ColumnsLabels;
											}
									}
							// Busca la colección de etiquetas de ese grupo
								foreach (DataColumn dcColumn in drRow.Table.Columns)
									if (dcColumn.ColumnName.EqualsIgnoreCase(objColGroups[intIndex].Field))
										{ string strTitle = "";

												// Obtiene el valor de la columna
													if (drRow[dcColumn] != null)
														strTitle = drRow[dcColumn].ToString();
												// Añadimos la etiqueta
													objActualLabel = objColLabels.Add(objActualLabel, strTitle.TrimIgnoreNull());
												// Añadimos la etiqueta de total
													objTotalLabel = CreateTotalLabel(objColLabels, objActualLabel, objTotalLabel);
										}
								// Crea la colección de hijos de la etiqueta actual
									if (objActualLabel == null)
										throw new NotImplementedException("No se encuentra ninguna etiqueta adecuada para el grupo " + objColGroups[intIndex].Title);
									else if (intIndex < objColGroups.Count - 1)
										{ // Añade el siguiente grupo a la colección de etiqueta hija
												if (objActualLabel.Childs == null)
													objActualLabel.Childs = new LabelModelCollection(objColGroups[intIndex + 1]);
											// Apunta a la colección de etiquetas hija
												objColLabels = objActualLabel.Childs;
										}
						}
		}

		/// <summary>
		///		Crea una etiqueta de total
		/// </summary>
		private LabelModel CreateTotalLabel(LabelModelCollection objColLabels, LabelModel objSource, LabelModel objTotalParent)
		{ LabelModel objTotal = objColLabels.SearchTotal();

				// Si estamos en la primera categoría y aún no hemos creado el total ...
					if (objTotal == null && objTotalParent == null) 
						{ // Crea la etiqueta de total de la cabecera (sin indicar que es una fila de total)
								objTotalParent = objColLabels.AddTotal(new GroupModel(GroupModel.GroupType.Total, "Total", null, true, -1), null, "Total");
							// Le añade una colección de etiquetas
								objTotalParent.Childs = new LabelModelCollection(new GroupModel(GroupModel.GroupType.Total, "Total", null, false, -1));
							// Asigna la colección de etiquetas a los hijos de la etiqueta que se acaba de crear
								objColLabels = objTotalParent.Childs;
						}
					else if (objTotal != null && objTotalParent == null) // ... está en la fila de cabecera, en realidad el total es uno de sus hijos
						{ objColLabels = objTotal.Childs;
							objTotal = objColLabels.SearchTotal();
						}
				// Si no se ha encontrado se añade
					if (objTotal == null)
						{ LabelModel objActualTotalParent = null;
							string strTitle = "Total";

								// Añade el nombre del grupo a la cabecera
									if (objColLabels.Group.Order != -1)
										strTitle += " - " + objColLabels.Group.Title;
								// Obtiene el padre de la etiqueta total
									if (objSource != null)
										objActualTotalParent = objSource.Parent;
								// Crea la etiqueta
									objTotal = new LabelModel(objColLabels, objActualTotalParent, strTitle, true);
								// Añade las etiquetas de las que tiene que sumar
									objTotal.LabelsTotal = new LabelModelCollection(objColLabels.Group);
								// Añade la etiqueta de total a la colección
									objColLabels.Add(objTotal);
						}
				// Añade la etiqueta a la colección de sumas
					if (!objTotal.LabelsTotal.Exists(objSource.GlobalID))
						objTotal.LabelsTotal.Add(objSource);
				// Devuelve la etiqueta creada
					return objTotal;
		}

		/// <summary>
		///		Añade las etiquetas del grupo de celdas y de total de grupo de celdas
		/// </summary>
		private void AddLabelsGroupCell(RawPivotTable objTable)
		{ LabelModelCollection objColEndLabels = GetLabelsEnd(objTable.ColumnsLabels, false, true);

				// Añade las etiquetas de grupo
					foreach (LabelModel objLabel in objColEndLabels)
						{ LabelModel objLabelTotal = new LabelModel(objColEndLabels, objLabel, "Total - " + objLabel.Title, true);

								// Inicializa la colección de etiquetas a sumar del total
									objLabelTotal.LabelsTotal = new LabelModelCollection(objColEndLabels.Group);
								// Crea una colección de etiquetas hijas para la celda
									objLabel.Childs = new LabelModelCollection(new GroupModel(GroupModel.GroupType.Cell, "Final", null, true, 
																																						objColEndLabels.Group.Order + 1, GroupModel.ComputeType.Sum));
								// Crea las etiquetas de grupo
									foreach (GroupModel objGroup in GroupsCells)
										{ LabelModel objLabelGroup = objLabel.Childs.Add(objLabel, objGroup.Title);
			
												// Asigna el grupo por el que se va a realizar el cálculo
													objLabelGroup.Group = objGroup;
												// Añade la etiqueta actual a la colección para la etiqueta de Total de las celdas de la fila
													objLabelTotal.LabelsTotal.Add(objLabelGroup);
										}
								// Añade la etiqueta de total
									objLabel.Childs.Add(objLabelTotal);
						}
			// Añade la totalización de las celdas de la fila
				foreach (LabelModel objLabel in objColEndLabels)
					if (objLabel.IsTotal)
						foreach (LabelModel objLabelGroup in objLabel.Childs)
							if (objLabelGroup.Group != null)
								objLabelGroup.LabelsTotal = GetGroupLabelsTotal(objLabelGroup);
		}

		/// <summary>
		///		Obtiene las etiquetas que se van a totalizar de un grupo
		/// </summary>
		private LabelModelCollection GetGroupLabelsTotal(LabelModel objLabel)
		{ LabelModelCollection objColLabelsTotal = new LabelModelCollection(objLabel.Group);

					System.Diagnostics.Debug.WriteLine("Añadiendo a: " + objLabel.GetFullTitle());
				// Añade las etiquetas que se deben sumar
					if (objLabel.Parent != null)
						foreach (LabelModel objTotal in objLabel.Parent.LabelsTotal)
							if (objTotal.Childs != null)
								foreach (LabelModel objLabelGroup in objTotal.Childs)
									if (objLabelGroup.Title.EqualsIgnoreCase(objLabel.Title) && !objColLabelsTotal.Exists(objLabelGroup.GlobalID))
										{ objColLabelsTotal.Add(objLabelGroup);
											System.Diagnostics.Debug.WriteLine("\tAñadiendo la etiqueta de total: " + objLabelGroup.GetFullTitle());
										}
				// Devuelve la colección de etiquetas
					return objColLabelsTotal;
		}

		/// <summary>
		///		Calcula los totales de las columnas
		/// </summary>
		private void ComputeTotalsColumn(RawPivotTable objTable)
		{ LabelModelCollection objColLabelTotalColumns = GetLabelsEnd(objTable.ColumnsLabels, true, false);
			LabelModelCollection objColLabelRows = GetLabelsEnd(objTable.RowsLabels, false, false);

				// Recorre las filas de totales calculando los totales
					foreach (LabelModel objLabelColumnTotal in objColLabelTotalColumns)
						{ // Para todas las filas ...
								foreach (LabelModel objLabelRow in objColLabelRows)
								//LabelModel objLabelRow = objColLabelRows[0];
									{ double dblValue = 0;

											// Depuración
												System.Diagnostics.Debug.WriteLine("Calculando total de la columna: " + objLabelColumnTotal.GetFullTitle() + " para la fila " + objLabelRow.GetFullTitle());
											// Recorre los elementos a totalizar
												if (objLabelColumnTotal.LabelsTotal != null)
													foreach (LabelModel objLabel in objLabelColumnTotal.LabelsTotal)
														{ string strKey = objTable.Cells.GetCellKey(objLabelRow, objLabel);
															CellModel objCell;

																System.Diagnostics.Debug.WriteLine("\tSuma el valor de ..." + objLabel.GetFullTitle());
																// Obtiene el valor de la celda y lo acumula
																	if (objTable.Cells.TryGetValue(strKey, out objCell))
																		if (objCell != null)
																			dblValue += objCell.Value;
														}
												else
													System.Diagnostics.Debug.WriteLine("\tNo se han definido etiquetas para sumar");
											// Añade la celda a la colección
												objTable.Cells.Add(objLabelColumnTotal.Group, objLabelRow, objLabelColumnTotal, dblValue);
									}
						}
		}

		/// <summary>
		///		Calcula los totales de las filas
		/// </summary>
		private void ComputeTotalsRow(RawPivotTable objTable)
		{ LabelModelCollection objColLabelTotalRows = GetLabelsEnd(objTable.RowsLabels, true, false);
			LabelModelCollection objColLabelColumns = GetLabelsEnd(objTable.ColumnsLabels, false, true);

				System.Diagnostics.Debug.WriteLine("Filas de totales ---------------------------------------------");
				System.Diagnostics.Debug.WriteLine(objColLabelTotalRows.GetDebugStructure());
				System.Diagnostics.Debug.WriteLine("Columnas ---------------------------------------------");
				System.Diagnostics.Debug.WriteLine(objColLabelColumns.GetDebugStructure());
				// Recorre las filas de totales calculando los totales
					foreach (LabelModel objLabelTotal in objColLabelTotalRows)
						{ // Para todas las filas ...
								foreach (LabelModel objLabelColumn in objColLabelColumns)
									{ double dblValue = 0;

											System.Diagnostics.Debug.WriteLine("Columna: " + objLabelColumn.GetFullTitle() + " Fila: " + objLabelTotal.GetFullTitle());

											// Recorre los elementos a totalizar
												foreach (LabelModel objLabel in objLabelTotal.LabelsTotal)
													{ string strKey = objTable.Cells.GetCellKey(objLabel, objLabelColumn);
														CellModel objCell;

															System.Diagnostics.Debug.WriteLine("\tSumar de ..." + objLabel.GetFullTitle());
															// Obtiene el valor de la celda y lo acumula
																if (objTable.Cells.TryGetValue(strKey, out objCell))
																	if (objCell != null)
																		dblValue += objCell.Value;
													}
											// Añade la celda a la colección
												objTable.Cells.Add(objLabelTotal.Group, objLabelTotal, objLabelColumn, dblValue);
									}
						}
		}

		/// <summary>
		///		Obtiene las etiquetas de totales
		/// </summary>
		private LabelModelCollection GetLabelsEnd(LabelModelCollection objColLabels, bool blnTotal, bool blnAll)
		{ LabelModelCollection objColTotals = new LabelModelCollection(new GroupModel(GroupModel.GroupType.Cell, "Total", "", false, 
																																									objColLabels.Group.Order + 1));

				// Obtiene las etiquetas finales de total
					foreach (LabelModel objLabel in objColLabels)
						if (objLabel.Childs == null && (blnAll || objLabel.IsTotal == blnTotal ||
																						(blnTotal && objLabel.Parent.IsTotal)))
							objColTotals.Add(objLabel);
						else if (objLabel.Childs != null)
							objColTotals.AddRange(GetLabelsEnd(objLabel.Childs, blnTotal, blnAll));
				// Devuelve la colección de etiquetas
					return objColTotals;
		}

		/// <summary>
		///		Calcula las celdas
		/// </summary>
		private void ComputeCells(RawPivotTable objTable, DataTable dtTable)
		{ LabelModelCollection objColLabelColumns = GetLabelsEnd(objTable.ColumnsLabels, false, false);
			LabelModelCollection objColLabelRows = GetLabelsEnd(objTable.RowsLabels, false, false);

				// Recorre las filas
					foreach (DataRow drRow in dtTable.Rows)
						{ LabelModel objLabelRow = GetLabelTitle(drRow, objTable.RowsLabels);
							LabelModel objLabelColumn = GetLabelTitle(drRow, objTable.ColumnsLabels);
							double dblTotal = 0;

								if (objLabelColumn.Childs != null && objLabelColumn.Childs.Group.Type == GroupModel.GroupType.Cell)
									{ // Calcula los valores de las etiquetas
											foreach (LabelModel objLabel in objLabelColumn.Childs)
												if (!objLabel.IsTotal)
													{ double dblValue = GetValue(drRow, objLabel);

															// Añade la celda a la colección
																objTable.Cells.Add(objLabelColumn.Childs.Group, objLabelRow, objLabel, dblValue);
															// Añade el total
																dblTotal += dblValue;
													}
									}
						}
		}

		/// <summary>
		///		Obtiene el título de fila
		/// </summary>
		private LabelModel GetLabelTitle(DataRow drRow, LabelModelCollection objColLabels)
		{ LabelModel objLabel = null;
			LabelModelCollection objColActualLabels = objColLabels;
			bool blnEnd = false;

				// Recorre los datos de la fila buscando la etiqueta
					do
						{	string strValue = "";

								// Obtiene el valor de la fila que está asociado al grupo que buscamos
									foreach (DataColumn dcColumn in drRow.Table.Columns)
										if (dcColumn.ColumnName.EqualsIgnoreCase(objColActualLabels.Group.Field) &&
												drRow[dcColumn] != null)
											strValue = drRow[dcColumn].ToString();
									strValue = strValue.TrimIgnoreNull();
								// Busca el valor entre las etiquetas
									foreach (LabelModel objActualLabel in objColActualLabels)
										if (objActualLabel.Title.EqualsIgnoreCase(strValue))
											objLabel = objActualLabel;
								// Pasa al siguiente grupo
									if (objLabel.Childs == null) // ... ya no podemos continuar
										blnEnd = true;
									else
										{ // Obtiene el siguiente grupo de etiquetas
												objColActualLabels = objLabel.Childs;
											// ... los grupos de celdas son grupos especiales y son por los que se realizq
											// el cálculo, por tanto no llegamos hasta ellos
												if (objColActualLabels.Group.Type == GroupModel.GroupType.Cell)
													blnEnd = true;
										}
						}
					while (!blnEnd);
				// Devuelve la etiqueta encontrada
					return objLabel;
		}

		/// <summary>
		///		Obtiene el valor de una fila para una etiqueta
		/// </summary>
		private double GetValue(DataRow drRow, LabelModel objLabel)
		{ // Busca el campo de la etiqueta
				foreach (DataColumn dcColumn in drRow.Table.Columns)
					if (dcColumn.ColumnName.EqualsIgnoreCase(objLabel.Group.Field))
						{ if (drRow[dcColumn] == null)
								return 0;
							else
								return Convert.ToDouble(drRow[dcColumn]);
						}
			// Si ha llegado hasta aquí es que o ha encontrado nada
				return 0;
		}

		/// <summary>
		///		Grupos definidos
		/// </summary>
		private GroupModelCollection Groups { get; set; }

		/// <summary>
		///		Definiciones de grupos de filas
		/// </summary>
		private GroupModelCollection GroupsRows { get; set; }

		/// <summary>
		///		Definiciones de grupos de columnas
		/// </summary>
		private GroupModelCollection GroupsColumns { get; set; }

		/// <summary>
		///		Definiciones de grupos de celdas
		/// </summary>
		private GroupModelCollection GroupsCells { get; set; }
	}
}
