using System;
using System.Data;

namespace Bau.Libraries.LibDBProvidersBase
{
	/// <summary>
	///		Funciones de extensión de <see cref="IDataReader"/>
	/// </summary>
	public static class DataReaderExtensors
	{
		/// <summary>
		///		Obtiene el valor de un campo de un IDataReader
		/// </summary>
		public static object IisNull(this IDataReader reader, string field, object defaultValue = null)
		{
			if (reader.IsDBNull(reader.GetOrdinal(field)))
				return defaultValue;
			else
				return reader.GetValue(reader.GetOrdinal(field));
		}

		/// <summary>
		///		Obtiene el valor de un campo de un IDataReader
		/// </summary>
		public static bool IisNull(this IDataReader reader, string field, bool defaultValue)
		{
			if (reader.IsDBNull(reader.GetOrdinal(field)))
				return defaultValue;
			else
				return reader.GetBoolean(reader.GetOrdinal(field));
		}

		/// <summary>
		///		Obtiene el valor de un campo de un IDataReader
		/// </summary>
		public static int IisNull(this IDataReader reader, string field, int defaultValue)
		{
			if (reader.IsDBNull(reader.GetOrdinal(field)))
				return defaultValue;
			else
				return reader.GetInt32(reader.GetOrdinal(field));
		}

		/// <summary>
		///		Obtiene el valor de un campo de un IDataReader
		/// </summary>
		public static double IisNull(this IDataReader reader, string field, double defaultValue)
		{
			if (reader.IsDBNull(reader.GetOrdinal(field)))
				return defaultValue;
			else
				return reader.GetDouble(reader.GetOrdinal(field));
		}

		/// <summary>
		///		Obtiene el valor de un campo de un IDataReader
		/// </summary>
		public static DateTime IisNull(this IDataReader reader, string field, DateTime defaultValue)
		{
			if (reader.IsDBNull(reader.GetOrdinal(field)))
				return defaultValue;
			else
				return reader.GetDateTime(reader.GetOrdinal(field));
		}
	}
}
