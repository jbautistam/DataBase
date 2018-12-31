using System;

namespace Bau.Libraries.LibReports.Generator.Data
{
	/// <summary>
	///		Fila leída del <see cref="DataTableReader"/>
	/// </summary>
	internal class DataTableReaderRow
	{
		internal DataTableReaderRow(int rowIndex, DataTableReader tableReader, System.Data.DataRow row)
		{
			TableReader = tableReader;
			RowIndex = rowIndex;
			Row = row;
		}

		/// <summary>
		///		Lector de la tabla de datos
		/// </summary>
		internal DataTableReader TableReader { get; }

		/// <summary>
		///		Indice de fila
		/// </summary>
		internal int RowIndex { get; }

		/// <summary>
		///		Fila de datos
		/// </summary>
		internal System.Data.DataRow Row { get; }
	}
}
