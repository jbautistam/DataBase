using System;

namespace Bau.Libraries.LibDBProvidersBase.Parameters
{
	/// <summary>
	///		Clase con los datos de un parámetro
	/// </summary>
	public class ParameterDB
	{
		public ParameterDB() { }

		public ParameterDB(string name, object value, System.Data.ParameterDirection direction) 
					: this(name, value, direction, 0)
		{ 
		}

		public ParameterDB(string name, object value, System.Data.ParameterDirection direction, int length)
		{
			Name = name;
			Value = value;
			Direction = direction;
			Length = length;
		}

		/// <summary>
		///		Obtiene un valor de tipo objeto o nulo para la base de datos
		/// </summary>
		public object GetDBValue()
		{
			if (Value == null)
				return DBNull.Value;
			else
				return Value;
		}

		/// <summary>
		///		Nombre del parámetro
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///		Valor del parámetro
		/// </summary>
		public object Value { get; set; }

		/// <summary>
		///		Dirección del parámetro
		/// </summary>
		public System.Data.ParameterDirection Direction { get; set; }

		/// <summary>
		///		Longitud
		/// </summary>
		public int Length { get; set; }

		/// <summary>
		///		Indica si es un parámetro de texto
		/// </summary>
		public bool IsText { get; set; }
	}
}