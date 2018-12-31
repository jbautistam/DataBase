using System;

namespace Bau.Libraries.PivotTableExtended.Definitions
{
	/// <summary>
	///		Clase abstracta para las definiciones de grupos
	/// </summary>
	public class GroupModel : Base.BaseModel
	{
		/// <summary>
		///		Tipo del grupo
		/// </summary>
		public enum GroupType
			{ 
				/// <summary>Fila</summary>
				Row,
				/// <summary>Columna</summary>
				Column,
				/// <summary>Celda</summary>
				Cell,
				/// <summary>Grupo de totales</summary>
				Total
			}
		/// <summary>
		///		Tipo de cálculo (para el caso de las celdas)
		/// </summary>
		public enum ComputeType
			{
				/// <summary>Desconocido. No se debería utilizar</summary>
				Unknown,
				/// <summary>Suma de valores</summary>
				Sum,
				/// <summary>Máximo de valores</summary>
				Maximum,
				/// <summary>Mínimo de valores</summary>
				Minimum,
				/// <summary>Media de valores</summary>
				Average
			}

		public GroupModel(GroupType intIDType, string strTitle, string strField, bool blnWithTotal, 
											int intOrder, ComputeType intIDCompute = ComputeType.Unknown)
		{ Type = intIDType;
			Title = strTitle;
			Field = strField;
			WithTotal = blnWithTotal;
			Order = intOrder;
			IDCompute = intIDCompute;
		}

		/// <summary>
		///		Tipo de grupo
		/// </summary>
		public GroupType Type { get; private set; }

		/// <summary>
		///		Título del grupo
		/// </summary>
		public string Title { get; private set; }

		/// <summary>
		///		Nombre del campo que recoge el grupo
		/// </summary>
		public string Field { get; private set; }

		/// <summary>
		///		Indica si en este caso se añade un total
		/// </summary>
		public bool WithTotal { get; private set; }

		/// <summary>
		///		Orden
		/// </summary>
		public int Order { get; internal set; }

		/// <summary>
		///		Tipo de cálculo
		/// </summary>
		public ComputeType IDCompute { get; private set; }
	}
}
