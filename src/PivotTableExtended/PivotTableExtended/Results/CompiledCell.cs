using System;

namespace Bau.Libraries.PivotTableExtended.Results
{
	/// <summary>
	///		Datos de una celda compilada
	/// </summary>
	public class CompiledCell
	{
		/// <summary>
		///		Tipo de celda
		/// </summary>
		public enum CellType
			{ 
				/// <summary>Cabecera</summary>
				Header,
				/// <summary>Contenido</summary>
				Content
			}

		public CompiledCell(Definitions.GroupModel objGroup, LabelModel objLabelRow, LabelModel objLabelColumn, 
												CellType intType, int intRow, int intColumn, int intRowSpan, int intColSpan,
												bool blnIsTotal, string strTitle, double dblValue)
		{ Group = objGroup;
			LabelRow = objLabelRow;
			LabelColumn = objLabelColumn;
			Type = intType;
			Row = intRow;
			Column = intColumn;
			RowSpan = intRowSpan;
			ColSpan = intColSpan;
			IsTotal = blnIsTotal;
			Title = strTitle;
			Value = dblValue;
		}

		/// <summary>
		///		Grupo al que pertenece la etiqueta
		/// </summary>
		public Definitions.GroupModel Group { get; private set; }

		/// <summary>
		///		Etiqueta a la que se asocia la celda para la fila
		/// </summary>
		public LabelModel LabelRow { get; private set; }

		/// <summary>
		///		Etiqueta a la que se asocia la celda para la columna
		/// </summary>
		public LabelModel LabelColumn { get; private set; }

		/// <summary>
		///		Tipo de celda
		/// </summary>
		public CellType Type { get; set; }

		/// <summary>
		///		Fila
		/// </summary>
		public int Row { get; set; }

		/// <summary>
		///		Columna
		/// </summary>
		public int Column { get; set; }

		/// <summary>
		///		Span de fila
		/// </summary>
		public int RowSpan { get; set; }

		/// <summary>
		///		Span de columna
		/// </summary>
		public int ColSpan { get; set; }

		/// <summary>
		///		Indica si es una celda de total
		/// </summary>
		public bool IsTotal { get; set; }

		/// <summary>
		///		Título
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		///		Valor
		/// </summary>
		public double Value { get; set; }
	}
}
