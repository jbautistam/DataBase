using System;
using System.Data;

namespace Bau.Libraries.LibReports.Generator.Data
{
	/// <summary>
	///		Lector de los datos de un <see cref="DataTable"/>
	/// </summary>
	internal class DataTableReader
	{
		internal DataTableReader(DataTableReader parent, DataTable table)
		{
			Parent = parent;
			Table = table;
		}

		/// <summary>
		///		Obtiene la fila actual
		/// </summary>
		internal DataTableReaderRow GetActualRow()
		{
			return ActualRow;
		}

		/// <summary>
		///		Lee la fila actual
		/// </summary>
		internal DataTableReaderRow ReadRow()
		{
			// Asigna la fila leída actual
			if (RowIndex < Table.Rows.Count)
			{
				ActualRow = new DataTableReaderRow(RowIndex, this, Table.Rows[RowIndex]);
				RowIndex++;
			}
			else
				ActualRow = null;
			// Devuelve la fila actual
			return ActualRow;
		}

		/// <summary>
		///		Fila actual
		/// </summary>
		internal int RowIndex { get; private set; } = 0;

		/// <summary>
		///		Ultima fila leída
		/// </summary>
		internal DataTableReaderRow ActualRow { get; private set; }

		/// <summary>
		///		Lector padre
		/// </summary>
		internal DataTableReader Parent { get; }

		/// <summary>
		///		Tabla de datos
		/// </summary>
		internal DataTable Table { get; }
	}
}
