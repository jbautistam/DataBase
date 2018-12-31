using System;

namespace Bau.Libraries.PivotTableExtended.Results
{
	/// <summary>
	///		Celda resultado
	/// </summary>
	public class CellModel
	{
		public CellModel(Definitions.GroupModel objGroup, LabelModel objRow, LabelModel objColumn, double dblValue)
		{ Group = objGroup;
			Row = objRow;
			Column = objColumn;
			ComputeValue = dblValue;
			CountItems = 1;
		}

		/// <summary>
		///		Obtiene el título completo de la celda
		/// </summary>
		internal string GetFullTitle()
		{ return Row.GetFullTitle() + " -> " + Column.GetFullTitle() + " (" + Group.Title + ")";
		}

		/// <summary>
		///		Grupo al que se asocia
		/// </summary>
		public Definitions.GroupModel Group { get; private set; }

		/// <summary>
		///		Fila
		/// </summary>
		public LabelModel Row { get; private set; }

		/// <summary>
		///		Columna
		/// </summary>
		public LabelModel Column { get; private set; }

		/// <summary>
		///		Valor
		/// </summary>
		public double Value 
		{ get
				{ return ComputeValue;
				}
		}

		/// <summary>
		///		Valor (para los cálculos internos como la media)
		/// </summary>
		internal double ComputeValue { get; set; }

		/// <summary>
		///		Número de elementos (para los cálculos internos como la media)
		/// </summary>
		internal int CountItems { get; set; }
	}
}
