using System;
using System.Collections.Generic;

using Bau.Libraries.LibCommonHelper.Extensors;

namespace Bau.Libraries.PivotTableExtended.Results
{
	/// <summary>
	///		Colección de <see cref="CompiledCell"/>
	/// </summary>
	public class CompiledCellCollection : List<CompiledCell>
	{
		/// <summary>
		///		Añade una cabecera
		/// </summary>
		internal void AddHeader(Definitions.GroupModel objGroup, LabelModel objLabel, int intRow, int intColumn, 
														int intRowSpan, int intColSpan, bool blnIsForRow, bool blnIsTotal)
		{ Add(new CompiledCell(objGroup, blnIsForRow ? objLabel : null, blnIsForRow ? null : objLabel, 
													 CompiledCell.CellType.Header, intRow, intColumn,
													 intRowSpan, intColSpan, blnIsTotal, objLabel.Title, 0));
		}

		/// <summary>
		///		Añade una celda
		/// </summary>
		internal void AddCell(int intRow, int intColumn, CellModel objCell, bool blnIsTotal)
		{ Add(new CompiledCell(objCell.Group, objCell.Row, objCell.Column, CompiledCell.CellType.Content,
													 intRow, intColumn, 1, 1, blnIsTotal, 
													 objCell.Row.GetFullTitle() + " / " + objCell.Column.GetFullTitle() + " (" + objCell.Group?.Title + ")",
													 objCell.Value));
		}

		/// <summary>
		///		Obtiene el número máximo de filas
		/// </summary>
		internal int GetNextRow()
		{ int intRow = 0;

				// Busca el máximo número de filas
					foreach (CompiledCell objCell in this)
						if (objCell.Row + objCell.RowSpan > intRow)
							intRow = objCell.Row + objCell.RowSpan;
				// Devuelve el número máximo de filas
					return intRow;
		}

		/// <summary>
		///		Obtiene el número máximo de columnas
		/// </summary>
		internal int GetNextColumn()
		{ int intColumn = 0;

				// Busca el máximo número de columnas
					foreach (CompiledCell objCell in this)
						if (objCell.Column + objCell.ColSpan > intColumn)
							intColumn = objCell.Column + objCell.ColSpan;
				// Devuelve el número máximo de columnas
					return intColumn;
		}

		/// <summary>
		///		Obtiene la información de depuración
		/// </summary>
		internal string GetDebugInfo()
		{ string strDebug = "";

				// Obtiene la información de depuración de las celdas
					foreach (CompiledCell objCell in this)
						{ strDebug += objCell.Title + " Type: " + objCell.Type + " Value: " + objCell.Value.ToString() + 
														" Row: " + objCell.Row + " Column: " + objCell.Column + " RowSpan: " + objCell.RowSpan + " ColSpan: " + objCell.ColSpan +
														" Total: " + objCell.IsTotal + Environment.NewLine;
						}
				// Devuelve la información de depuración
					return strDebug;
		}

		/// <summary>
		///		Ordena las celdas
		/// </summary>
		internal void SortByRows()
		{ Sort((objFirst, objSecond) => 
							{ if (objFirst.Row == objSecond.Row)
									return objFirst.Column.CompareTo(objSecond.Column);
								else
									return objFirst.Row.CompareTo(objSecond.Row);
							}
					);
		}

		/// <summary>
		///		Busca una etiqueta
		/// </summary>
		internal CompiledCell SearchByHeaderLabel(LabelModel objLabel)
		{ // Recorre las celdas
				foreach (CompiledCell objCell in this)
					if (objCell.Type == CompiledCell.CellType.Header &&
							(objCell.LabelRow != null && objCell.LabelRow.IsEqual(objLabel)) ||
							(objCell.LabelColumn != null && objCell.LabelColumn.IsEqual(objLabel)))
						return objCell;
			// Si ha llegado hasta aquí es porque no ha encontrado nada
				return null;
		}

		/// <summary>
		///		Obtiene la celda que se encuentra en una fila y columna
		/// </summary>
		public CompiledCell this[int intRow, int intColumn]
		{ get
				{ // Busca la celda
						foreach (CompiledCell objCell in this)
							if (objCell.Row == intRow && objCell.Column == intColumn)
								return objCell;
					// Si ha llegado hasta aquí es porque no ha encontrado nada
						return null;
				}
		}
	}
}
