using System;

using Bau.Libraries.PivotTableExtended.Definitions;

namespace Bau.Libraries.PivotTableExtended.Results
{
	/// <summary>
	///		Colección de <see cref="CellModel"/>
	/// </summary>
	public class CellModelCollection : System.Collections.Generic.Dictionary<string, CellModel>
	{
		/// <summary>
		///		Añade un valor al diccionario
		/// </summary>
		internal void Add(GroupModel objGroup, LabelModel objLabelRow, LabelModel objLabelColumn, double dblValue)
		{ string strKey = GetCellKey(objLabelRow, objLabelColumn);
			CellModel objCell;

				if (TryGetValue(strKey, out objCell))
					{ // Calcula la fórmula
							switch (objGroup.IDCompute)
								{	case GroupModel.ComputeType.Sum:
									case GroupModel.ComputeType.Average:
											objCell.ComputeValue += dblValue;
										break;
									case GroupModel.ComputeType.Maximum:
											if (dblValue > objCell.ComputeValue)
												objCell.ComputeValue = dblValue;
										break;
									case GroupModel.ComputeType.Minimum:
											if (dblValue < objCell.ComputeValue)
												objCell.ComputeValue = dblValue;
										break;
								}
						// Incrementa el número de elementos de la celda
							objCell.CountItems++;
					}
				else
					Add(strKey, new CellModel(objGroup, objLabelRow, objLabelColumn, dblValue));
		}

		/// <summary>
		///		Obtiene la estructura de las celdas
		/// </summary>
		internal string GetDebugStructure()
		{ string strDebug = "";

				// Calcula la cadena de depuración
					foreach (System.Collections.Generic.KeyValuePair<string, CellModel> objKeyValue in this)
						strDebug += "\tGrupo: " + objKeyValue.Value.Group?.Title + "\tFila: " + objKeyValue.Value.Row.GetFullTitle() + 
													"\tColumna: " + objKeyValue.Value.Column.GetFullTitle() + "\tValor: " + objKeyValue.Value.Value + Environment.NewLine;
				// Devuelve la cadena
					return strDebug;
		}
		/// <summary>
		///		Obtiene la clave de una celda
		/// </summary>
		internal string GetCellKey(LabelModel objLabelRow, LabelModel objLabelColumn)
		{ return objLabelRow.GetFullID() + "##" + objLabelColumn.GetFullID();
		}

		///// <summary>
		/////		Obtiene la clave de una celda
		///// </summary>
		//internal string GetCellKey(GroupModel objGroup, LabelModel objLabelRow, LabelModel objLabelColumn)
		//{ return objGroup.GlobalID + "##" + objLabelRow.GetFullID() + "##" + objLabelColumn.GetFullID();
		//}
	}
}
