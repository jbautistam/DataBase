using System;
using System.Linq;

using Bau.Libraries.LibCommonHelper.Extensors;

namespace Bau.Libraries.PivotTableExtended.Definitions
{
	/// <summary>
	///		Colección de <see cref="GroupModel"/>
	/// </summary>
	public class GroupModelCollection : Base.BaseModelCollection<GroupModel>
	{
		/// <summary>
		///		Añade una definición de grupo
		/// </summary>
		internal void Add(GroupModel.GroupType intIDType, string strTitle, string strField, bool blnWithTotal, int? intOrder, 
											GroupModel.ComputeType intIDComputeType = GroupModel.ComputeType.Unknown)
		{ Add(new GroupModel(intIDType, strTitle, strField, blnWithTotal, intOrder ?? Count, intIDComputeType));
		}

		/// <summary>
		///		Obtiene los grupos de determinado tipo
		/// </summary>
		internal GroupModelCollection GetGroups(GroupModel.GroupType intIDType)
		{ GroupModelCollection objColGroups = new GroupModelCollection();

				// Asigna los grupos del tipo
					foreach (GroupModel objGroup in this)
						if (objGroup.Type == intIDType)
							objColGroups.Add(objGroup);
				// Normaliza los órdenes
					for (int intOrder = 0; intOrder < objColGroups.Count; intOrder++)
						objColGroups[intOrder].Order = intOrder;
				// Devuelve la colección de grupos
					return objColGroups;
		}

		/// <summary>
		///		Busca el grupo en el que se define el campo
		/// </summary>
		internal GroupModel SearchByField(string strName)
		{ return this.First<GroupModel>(objGroup => objGroup.Field.EqualsIgnoreCase(strName));
		}
	}
}
