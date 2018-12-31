using System;

namespace Bau.Libraries.PivotTableExtended.Results
{
	/// <summary>
	///		Tabla pivotal con los datos en crudo
	/// </summary>
	public class RawPivotTable
	{
		public RawPivotTable(Definitions.GroupModelCollection objColGroups)
		{ Groups = objColGroups;
		}

		/// <summary>
		///		Obtiene una cadena con la información de las tablas
		/// </summary>
		public string GetDebugStructure()
		{ string strDebug;

				// Añade la información de las etiquetas
					strDebug = "Información de filas" + Environment.NewLine;
					strDebug += RowsLabels.GetDebugStructure();
					strDebug += "Información de columnas" + Environment.NewLine;
					strDebug += ColumnsLabels.GetDebugStructure();
					strDebug += "Información de celdas" + Environment.NewLine;
					strDebug += Cells.GetDebugStructure();
					strDebug += "----------------------------------------------------------------------" + Environment.NewLine;
				// Devuelve la información de depuración
					return strDebug;
		}

		/// <summary>
		///		Obtiene la colección de celdas compilada de la tabla
		/// </summary>
		public CompiledPivotTable GetCompiledTable()
		{ CompiledPivotTable objTable = new CompiledPivotTable();

				// Compila la tabla
					objTable.Compile(this);
				// Devuelve la tabla compilada
					return objTable;
		}

		/// <summary>
		///		Definiciones de grupos
		/// </summary>
		public Definitions.GroupModelCollection Groups { get; private set; }

		/// <summary>
		///		Etiquetas de fila
		/// </summary>
		public LabelModelCollection RowsLabels { get; set; }

		/// <summary>
		///		Etiquetas de columna
		/// </summary>
		public LabelModelCollection ColumnsLabels { get; set; }

		/// <summary>
		///		Celdas
		/// </summary>
		public CellModelCollection Cells { get; private set; } = new CellModelCollection();
	}
}
