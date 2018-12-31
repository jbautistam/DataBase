using System;
using System.Data;

namespace Bau.Libraries.LibDBProvidersBase.Extensors
{
	/// <summary>
	///		Extensor para <see cref="System.Data.DataRow"/>
	/// </summary>
	public static class DataRowExtensor
	{
		///// <summary>
		/////		Obtiene un valor de una fila
		///// </summary>
		//public static TypeData IisNull<TypeData>(this DataRow row, string field, TypeData defaultValue = null) where TypeData : class
		//{
		//	if (row.IsNull(field))
		//		return defaultValue;
		//	else
		//		return (TypeData) row[field];
		//}

		/// <summary>
		///		Obtiene un valor de una celda convertido en cadena
		/// </summary>
		public static string IisNull(this DataRow row, string field, string defaultValue = null)
		{
			if (row.IsNull(field))
				return defaultValue;
			else
				return row[field].ToString();
		}
		/// <summary>
		///		Obtiene el valor de un campo de un DataRow
		/// </summary>
		public static object IisNull(this DataRow row, string field, object defaultValue = null)
		{
			if (row.IsNull(field))
				return defaultValue;
			else
				return row[field];
		}

		/// <summary>
		///		Obtiene el valor de un campo de un DataRow
		/// </summary>
		public static bool IisNull(this DataRow row, string field, bool defaultValue)
		{
			if (row.IsNull(field))
				return defaultValue;
			else
				return Convert.ToBoolean(row[field]);
		}

		/// <summary>
		///		Obtiene el valor de un campo de un DataRow
		/// </summary>
		public static int? IisNull(this DataRow row, string field, int? defaultValue = null)
		{
			if (row.IsNull(field))
				return defaultValue;
			else
				return Convert.ToInt32(row[field]);
		}

		/// <summary>
		///		Obtiene el valor de un campo de un DataRow
		/// </summary>
		public static double? IisNull(this DataRow row, string field, double? defaultValue = null)
		{
			if (row.IsNull(field))
				return defaultValue;
			else
				return Convert.ToDouble(row[field]);
		}

		/// <summary>
		///		Obtiene el valor de un campo de un DataRow
		/// </summary>
		public static DateTime? IisNull(this DataRow row, string field, DateTime? defaultValue = null)
		{
			if (row.IsNull(field))
				return defaultValue;
			else
				return Convert.ToDateTime(row[field]);
		}
	}
}
