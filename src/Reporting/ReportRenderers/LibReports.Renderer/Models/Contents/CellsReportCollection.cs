using System;

using Bau.Libraries.LibReports.Renderer.Models.Base;

namespace Bau.Libraries.LibReports.Renderer.Models.Contents
{
	/// <summary>
	///		Colección de <see cref="CellReport"/>
	/// </summary>
	public class CellsReportCollection : Base.ClassBaseCollection<CellReport>
	{
		/// <summary>
		///		Añade una celda con texto a la colección
		/// </summary>
		public CellReport Add(ContentReportBase parent, int row, int column, string text, string classId,
							  Styles.StyleReport style = null, int rowSpan = 1, int columnSpan = 1)
		{
			return Add(parent, row, column, new ParagraphReport(null, text), classId, style, rowSpan, columnSpan);
		}

		/// <summary>
		///		Añade una celda de tipo tabla a la colección
		/// </summary>
		public CellReport Add(ContentReportBase parent, int row, int column, TableReport table, int rowSpan, int columnSpan)
		{
			return Add(parent, row, column, table, null, null, rowSpan, columnSpan);
		}

		/// <summary>
		///		Añade una celda a la colección
		/// </summary>
		public CellReport Add(ContentReportBase parent, int row, int column, ContentReportBase content, string classId,
							  Styles.StyleReport style = null, int rowSpan = 1, int columnSpan = 1)
		{
			CellReport cell = new CellReport(parent);

				// Asigna los parámetros a la celda
				cell.ClassId = classId;
				cell.StyleClass = style;
				cell.Content = content;
				cell.Row = row;
				cell.Column = column;
				cell.RowSpan = rowSpan;
				cell.ColumnSpan = columnSpan;
				// Asigna el parent correcto
				cell.Content.Parent = cell;
				// Añade la celda a la colección
				Add(cell);
				// Devuelve la celda
				return cell;
		}

		/// <summary>
		///		Ordena las celdas de la colección por fila y columna
		/// </summary>
		public void SortByPosition()
		{
			Sort(delegate (CellReport first, CellReport second)
							  {
								  if (first.Row == second.Row)
									  return first.Column.CompareTo(second.Column);
								  else
									  return first.Row.CompareTo(second.Row);
							  }
					  );
		}

		/// <summary>
		///		Obtiene el número máximo de columnas
		/// </summary>
		public int GetColumnsNumber()
		{
			CellReport objRightCell = null;

				// Recorre la colección buscando la celda que se encuentra más a la derecha (la que tiene 
				// el mayor índice de columna 
				foreach (CellReport cell in this)
					if (objRightCell == null || cell.Column > objRightCell.Column)
						objRightCell = cell;
				// Devuelve el máximo número de columnas: es decir, el número de columna más el columnSpan
				if (objRightCell != null)
					return objRightCell.Column + objRightCell.ColumnSpan - 1;
				else
					return 0;
		}

		/// <summary>
		///		Obtiene el número máximo de fila
		/// </summary>
		public int GetMaximumRow()
		{
			int row = 0;

				// Recorre las celdas buscando el máximo número de fila
				foreach (CellReport cell in this)
					if (cell.Row > row)
						row = cell.Row;
				// Devuelve el número máximo de fila
				return row;
		}
	}
}
