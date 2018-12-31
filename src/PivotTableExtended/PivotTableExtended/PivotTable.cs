using System;
using System.Data;

namespace Bau.Libraries.PivotTableExtended
{
	/// <summary>
	///		Clase con los datos de una tabla pivotal
	/// </summary>
	public class PivotTable
	{
		/// <summary>
		///		Añade una definición de fila
		/// </summary>
		public void AddGroupRow(string strTitle, string strField, bool blnWithTotal, int? intOrder = null)
		{ Groups.Add(Definitions.GroupModel.GroupType.Row, strTitle, strField, blnWithTotal, intOrder);
		}

		/// <summary>
		///		Añade una definición de columna
		/// </summary>
		public void AddGroupColumn(string strTitle, string strField, bool blnWithTotal, int? intOrder = null)
		{ Groups.Add(Definitions.GroupModel.GroupType.Column, strTitle, strField, blnWithTotal, intOrder);
		}

		/// <summary>
		///		Añade una definición de celda
		/// </summary>
		public void AddGroupCell(string strTitle, string strField, Definitions.GroupModel.ComputeType intCompute, int? intOrder = null)
		{ Groups.Add(Definitions.GroupModel.GroupType.Cell, strTitle, strField, false, intOrder, intCompute);
		}

		/// <summary>
		///		Calcula la tabla pivotal resultante
		/// </summary>
		public Results.RawPivotTable Compute(DataTable objDataTable)
		{	if (Groups.GetGroups(Definitions.GroupModel.GroupType.Row).Count < 1)
				throw new NotImplementedException("No se han definido las filas de la tabla");
			else if (Groups.GetGroups(Definitions.GroupModel.GroupType.Column).Count < 1)
				throw new NotImplementedException("No se han definido las columnas de la tabla");
			else if (Groups.GetGroups(Definitions.GroupModel.GroupType.Cell).Count < 1)
				throw new NotImplementedException("No se han definido las celdas de la tabla");
			else
				return new Generator.PivotGenerator(Groups).Compute(objDataTable);
		}

		/// <summary>
		///		Definiciones de grupos asociadas a la tabla
		/// </summary>
		internal Definitions.GroupModelCollection Groups { get; private set; } = new Definitions.GroupModelCollection();
	}
}
