using System;

namespace Bau.Libraries.PivotTableExtended.Base
{
	/// <summary>
	///		Clase base
	/// </summary>
	public class BaseModel
	{	// Variables privadas
			private string strID;

		/// <summary>
		///		Comprueba si un objeto es nulo (o está vacío)
		/// </summary>
		public static bool IsNull(BaseModel objBase)
		{ return objBase == null || objBase.ID == null;
		}

		/// <summary>
		///		ID del elemento
		/// </summary>
		public int? ID { get; set; }

		/// <summary>
		///		ID global alfanumérico
		/// </summary>
		public string GlobalID
		{ get 
				{ // Si no se ha definido ningún ID se crea uno
						if (string.IsNullOrEmpty(strID))
							strID = Guid.NewGuid().ToString();
					// Devuelve el ID
						return strID;
				}
			set { strID = value; }
		}					

		/// <summary>
		///		Indica si el elemento se considera vacío (aún no se ha grabado)
		/// </summary>
		public bool IsEmpty
		{ get { return ID == null; }
		}
	}
}
