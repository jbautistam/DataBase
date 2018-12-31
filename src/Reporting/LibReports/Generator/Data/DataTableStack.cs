using System;
using System.Collections.Generic;
using System.Data;

using Bau.Libraries.Aggregator.Providers.Base;
using Bau.Libraries.LibCommonHelper.Extensors;

namespace Bau.Libraries.LibReports.Generator.Data
{
	/// <summary>
	///		Pila de <see cref="DataTable"/>
	/// </summary>
	internal class DataTableStack
	{
		public DataTableStack(Dictionary<string, object> parameters)
		{
			ReportFilters = parameters;
		}

		/// <summary>
		///		Añade una tabla de datos
		/// </summary>
		internal void Add(DataTable table)
		{
			Tables.Add(new DataTableReader(ActualReader, table));
		}

		/// <summary>
		///		Comprueba si está vacío
		/// </summary>
		internal bool IsEmpty()
		{
			return Tables.Count == 0;
		}

		/// <summary>
		///		Obtiene las filas de la tabla actual
		/// </summary>
		internal IEnumerable<DataTableReaderRow> GetRows()
		{
			// Obtiene las filas
			while (ActualReader?.RowIndex < ActualReader?.Table.Rows.Count)
				yield return ActualReader.ReadRow();
			// Quita el elemento de la cola
			Pop();
		}

		/// <summary>
		///		Quita el último elemento de la cola
		/// </summary>
		private void Pop()
		{
			if (Tables.Count > 0)
				Tables.RemoveAt(Tables.Count - 1);
		}

		/// <summary>
		///		Elimina una tabla de la cola
		/// </summary>
		internal void Remove(DataTable table)
		{
			for (int index = Tables.Count - 1; index >= 0; index--)
				if (Tables[index].Table.Equals(table))
					Tables.RemoveAt(index);
		}

		/// <summary>
		///		Obtiene la fila que se está tratando en este momento
		/// </summary>
		internal DataTableReaderRow GetActualRow()
		{
			if (ActualReader == null)
				return null;
			else
				return ActualReader.GetActualRow();
		}

		/// <summary>
		///		Obtiene el valor de un campo para la fila actual o alguna de las anteriores
		/// </summary>
		internal object GetRowValue(string field)
		{
			DataTableReader previous = ActualReader; // ... aún no se ha añadido el DataTable que se está leyendo, por eso no mira en ActualReader.Parent

				// Normaliza el nombre de columna
				field = NormalizeColumnName(field);
				// Recorre las filas de datos buscando el campo del registro correspondiente desde la última fila hasta la primera. 
				// Es decir, primero obtiene el valor de registro más cercano a la sección que se está procesando
				while (previous != null)
				{
					DataTableReaderRow row = previous.GetActualRow();

						// Obtiene el campo de la fila
						if (row != null && ExistsColumn(row.Row, field))
							return row.Row[field];
						// Pasa al elemento anterior de la pila si no ha encontrado nada
						previous = previous.Parent;
				}
				// Si ha llegado hasta aquí es porque no ha encontrado nada
				return null;
		}

		/// <summary>
		///		Normaliza el nombre de columna
		/// </summary>
		/// <remarks>
		///		Si el nombre de columna está asociado a un filtro puede que tenga una arroba delante (@CustomerId, por ejemplo)
		/// </remarks>
		private string NormalizeColumnName(string fieldName)
		{
			// Comprueba si existe la columna
			if (!string.IsNullOrEmpty(fieldName))
			{ 
				// Normaliza el nombre del parámetro
				fieldName = fieldName.Trim();
				if (fieldName.StartsWith("@"))
					fieldName = fieldName.Substring(1);
			}
			// Devuelve el nombre normalizado
			return fieldName;
		}

		/// <summary>
		///		Comprueba si existe una columna
		/// </summary>
		private bool ExistsColumn(DataRow row, string fieldName)
		{ 
			// Comprueba si existe la columna
			foreach (DataColumn column in row.Table.Columns)
				if (column.Caption.Equals(fieldName, StringComparison.CurrentCultureIgnoreCase))
					return true;
			// Si ha llegado hasta aquí es porque no existe la columna
			return false;
		}

		/// <summary>
		///		Filtros del informe
		/// </summary>
		internal Dictionary<string, object> ReportFilters { get; } = new Dictionary<string, object>();

		/// <summary>
		///		Lector actual
		/// </summary>
		internal DataTableReader ActualReader
		{
			get
			{
				if (Tables.Count == 0)
					return null;
				else
					return Tables[Tables.Count - 1];
			}
		}

		/// <summary>
		///		Tablas
		/// </summary>
		private List<DataTableReader> Tables { get; } = new List<DataTableReader>();
	}
}
