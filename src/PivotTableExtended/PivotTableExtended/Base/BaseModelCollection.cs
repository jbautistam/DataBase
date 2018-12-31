using System;
using System.Collections.Generic;
using System.Linq;

using Bau.Libraries.LibCommonHelper.Extensors;

namespace Bau.Libraries.PivotTableExtended.Base
{
	/// <summary>
	///		Colección de <see cref="BaseModel"/>
	/// </summary>
	public class BaseModelCollection<TypeData> : List<TypeData> where TypeData : BaseModel
	{
		/// <summary>
		///		Obtiene el índice de un elemento en la colección
		/// </summary>
		public virtual int IndexOf(string strGuid)
		{ // Busca el elemento en la colección
				for (int intIndex = 0; intIndex < Count; intIndex++)
					if (this[intIndex].GlobalID == strGuid)
						return intIndex;
			// Si ha llegado hasta aquí es porque no ha encontrado nada
				return -1;
		}

		/// <summary>
		///		Busca un elemento por su clave numérica
		/// </summary>
		public virtual TypeData Search(int? intID)
		{ return this.FirstOrDefault<TypeData>(objData => objData.ID == intID);
		}

		/// <summary>
		///		Busca un elemento
		/// </summary>
		public TypeData Search(TypeData objItem)
		{ return this.FirstOrDefault(objData => objData.ID == objItem.ID);
		}

		/// <summary>
		///		Busca un elemento por el Guid
		/// </summary>
		public virtual TypeData Search(string strGuid)
		{ return this.FirstOrDefault<TypeData>(objData => objData.GlobalID.EqualsIgnoreCase(strGuid));
		}

		/// <summary>
		///		Comprueba si existe un dato
		/// </summary>
		public virtual bool Exists(int? intID)
		{ return Search(intID) != null;
		}

		/// <summary>
		///		Comprueba si existe un dato
		/// </summary>
		public virtual bool Exists(string strGlobalID)
		{ return Search(strGlobalID) != null;
		}

		/// <summary>
		///		Elimina un elemento de la colección
		/// </summary>
		public void RemoveByID(int? intID)
		{ for (int intIndex = Count - 1; intIndex >= 0; intIndex--)
				if (this[intIndex].ID == intID)
					RemoveAt(intIndex);
		}

		/// <summary>
		///		Elimina un elemento de la colección
		/// </summary>
		public void RemoveByID(string strKey)
		{ for (int intIndex = Count - 1; intIndex >= 0; intIndex--)
				if (this[intIndex].GlobalID.Equals(strKey, StringComparison.CurrentCultureIgnoreCase))
					RemoveAt(intIndex);
		}

		/// <summary>
		///		Indizador de la colección
		/// </summary>
		public TypeData this[string strGlobalID]
		{ get { return Search(strGlobalID); }
			set
				{ int intIndex = IndexOf(strGlobalID);

						if (intIndex >= 0)
							this[intIndex] = value;
				}
		}
	}
}
