using System;

using Bau.Libraries.PivotTableExtended.Definitions;

namespace Bau.Libraries.PivotTableExtended.Results
{
	/// <summary>
	///		Datos de una tabla compilada (útil para generar por ejemplo una tabla de HTML)
	/// </summary>
	public class CompiledPivotTable
	{
		/// <summary>
		///		Compila una tabla
		/// </summary>
		public void Compile(RawPivotTable objPivot)
		{ GroupModelCollection objColGroupRows = objPivot.Groups.GetGroups(GroupModel.GroupType.Row);
			GroupModelCollection objColGroupsColumn = objPivot.Groups.GetGroups(GroupModel.GroupType.Column);
			GroupModelCollection objColGroupsCells = objPivot.Groups.GetGroups(GroupModel.GroupType.Cell);

				// Inicializa las variables necesarias en el dibujo
					RowStartCells = objColGroupRows.Count + 2;
					ColumnStartCell = objColGroupsColumn.Count + 1;
				// Limpia la tabla
					Cells.Clear();
				// Crea las cabeceras de columnas
					CreateRowColumnHeaders(objPivot.ColumnsLabels, false, 1, ColumnStartCell);
				// Crea las cabeceras de filas
					CreateRowColumnHeaders(objPivot.RowsLabels, true, RowStartCells, 1);
				// Crea las celdas
					CreateCells(objPivot);
				// Ordena la tabla
					Cells.SortByRows();
		}

		/// <summary>
		///		Crea las cabeceras
		/// </summary>
		private void CreateRowColumnHeaders(LabelModelCollection objColLabels, bool blnByRows, int intRow, int intColumn)
		{	foreach (LabelModel objLabel in objColLabels)
				{ int intSpan = objLabel.ComputeSpan();

						// Crea la celda
							Cells.AddHeader(objColLabels.Group, objLabel, intRow, intColumn,
															blnByRows ? intSpan : 1, blnByRows ? 1 : intSpan, blnByRows, false);
						// Pasa a la siguiente fila o columna
							if (objLabel.Childs != null)
								CreateRowColumnHeaders(objLabel.Childs, blnByRows, 
																			 blnByRows ? intRow: intRow + 1, blnByRows ? intColumn + 1 : intColumn);
						// Incrementa la fila o columna
							if (blnByRows)
								intRow += intSpan;
							else
								intColumn += intSpan;
				}
		}

		/// <summary>
		///		Crea las celdas
		/// </summary>
		private void CreateCells(RawPivotTable objPivot)
		{ foreach (System.Collections.Generic.KeyValuePair<string, CellModel> objKeyCell in objPivot.Cells)
				{ int intRow = Cells.SearchByHeaderLabel(objKeyCell.Value.Row).Row;
					int intColumn = Cells.SearchByHeaderLabel(objKeyCell.Value.Column).Column;

						// Añade la celda
							Cells.AddCell(intRow, intColumn, objKeyCell.Value, false);
				}
		}

		/// <summary>
		///		Obtiene la información de depuración
		/// </summary>
		public string GetDebugInfo()
		{ return Cells.GetDebugInfo();
		}

		/// <summary>
		///		Obtiene el HTML de la tabla
		/// </summary>
		public string GetHtml()
		{ int intMaxRows = Cells.GetNextRow();
			int intMaxColumns = Cells.GetNextColumn();
			string strHtml = "<table border = '1'>" + Environment.NewLine;

				for (int intRow = 1; intRow < intMaxRows; intRow++)
					{ int intColumn = 1;

							// Crea la fila
								strHtml += "<tr>" + Environment.NewLine;
							// Crea las columnas
								while (intColumn < intMaxColumns)
									{ CompiledCell objCell = Cells[intRow, intColumn];

											// Si no se encuentra la celda, se añade una celda en blanco o bien el contenido de la celda
												if (objCell == null)
													{ if ((intRow >= RowStartCells && intColumn >= ColumnStartCell) ||
																(intRow <= RowStartCells && intColumn <= ColumnStartCell))
															strHtml += "<td>&nbsp;</td>" + Environment.NewLine;
														intColumn++;
													}
												else
													{ // Crea la cadena de la celda
															if (objCell.Type == CompiledCell.CellType.Header)
																strHtml += string.Format("<th rowspan = '{0}' colspan= '{1}' title='{2}'>{3}</th>", 
																												 objCell.RowSpan, objCell.ColSpan, 
																												 (objCell.LabelColumn == null ? objCell.LabelRow.GetFullTitle() : objCell.LabelColumn.GetFullTitle()) +
																														objCell.Group.Type + " - " + objCell.Group.Title, 
																												 objCell.Title) + Environment.NewLine;
															else
																strHtml += "<td title = '" + objCell.Title + "'>" + objCell.Value + "</td>" + Environment.NewLine;
														// Incrementa el contador de columna
															intColumn += objCell.ColSpan;
													}
									}
							// Cierra la fila
								strHtml += "</tr>" + Environment.NewLine;
					}
				// Cierra el HTML	
					strHtml += "</table>" + Environment.NewLine;
				// Devuelve el HTML	
					return strHtml;
		}

		/// <summary>
		///		Celdas de la tabla
		/// </summary>
		public CompiledCellCollection Cells { get; private set; } = new CompiledCellCollection();

		/// <summary>
		///		Fila en la que se comienzan a dibujar las celdas internas de la tabla
		/// </summary>
		private int RowStartCells { get; set; }

		/// <summary>
		///		Columna en la que se comienzan a dibujar las celdas internas de la tabla
		/// </summary>
		private int ColumnStartCell { get; set; }
	}
}
