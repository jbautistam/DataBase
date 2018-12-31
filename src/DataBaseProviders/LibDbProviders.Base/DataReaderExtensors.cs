using System;
using System.Data;

namespace Bau.Libraries.LibDbProviders.Base
{
	/// <summary>
	///		Funciones de extensión de <see cref="DataTable"/>
	/// </summary>
	public static class DataTableExtensors
	{
		/// <summary>
		///		Obtiene el valor de un campo de un <see cref="DataRow"/>
		/// </summary>
		public static object IisNull(this DataRow row, string field, object defaultValue = null)
		{ 
			object result = row[field];

				if (result is DBNull)
					return defaultValue;
				else
					return result;
		}

		/// <summary>
		///		Obtiene el valor de un campo de un <see cref="DataRow"/>
		/// </summary>
		public static TypeData IisNull<TypeData>(this DataRow row, string field, object defaultValue = null)
		{ 
			object result = row[field];

				if (result is DBNull)
					return (TypeData) defaultValue;
				else
					return (TypeData) result;
		}
	}
}
