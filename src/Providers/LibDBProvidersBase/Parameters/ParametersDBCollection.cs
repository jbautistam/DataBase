using System;
using System.Data;

namespace Bau.Libraries.LibDBProvidersBase.Parameters
{
	/// <summary>
	///		Clase que almacena los parámetros de un comando
	/// </summary>
	public class ParametersDBCollection : System.Collections.Generic.List<ParameterDB>
	{
		/// <summary>
		///		Añade un parámetro a la colección de parámetros del comando
		/// </summary>
		public void Add(string name, string value, int length, ParameterDirection direction = ParameterDirection.Input)
		{ 
			// Corta la cadena si supera la longitud máxima
			if (!string.IsNullOrEmpty(value) && value.Length > length)
				value = value.Substring(0, length);
			// Añade el parámetro
			Add(new ParameterDB(name, value, direction, length));
		}

		/// <summary>
		///		Añade un parámetro a la colección de parámetros del comando
		/// </summary>
		public void Add(string name, object value, ParameterDirection direction = ParameterDirection.Input, bool skipValueZero = true)
		{
			if (value is Enum)
			{
				int? intValue = (int) ((object) value);

					// Convierte el valor 0 del enumerado en un valor NULL
					if (skipValueZero && (intValue ?? 0) == 0)
						intValue = null;
					// Añade el valor del parámetro
					Add(new ParameterDB(name, intValue, direction));
			}
			else
				Add(new ParameterDB(name, value, direction));
		}

		/// <summary>
		///		Añade un parámetro a la colección de parámetros del comando
		/// </summary>
		public void Add(string name, byte[] buffer, ParameterDirection direction = ParameterDirection.Input)
		{
			Add(new ParameterDB(name, buffer, direction));
		}

		/// <summary>
		///		Añade un parámetro de tipo Text
		/// </summary>
		public void AddText(string name, string value, ParameterDirection direction = ParameterDirection.Input)
		{
			ParameterDB parameter = new ParameterDB(name, value, direction);

				// Indica que es un parámetro de texto
				parameter.IsText = true;
				// Añade el parámetro a la colección
				Add(parameter);
		}

		/// <summary>
		///		Obtiene un parámetro de la colección a partir del nombre, si no lo encuentra devuelve un parámetro vacío
		/// </summary>
		public ParameterDB Search(string name)
		{ 
			// Recorre la colección de parámetros buscando el elemento adecuado
			foreach (ParameterDB parameter in this)
				if (parameter.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
					return parameter;
			// Devuelve un objeto vacío
			return new ParameterDB(name, null, ParameterDirection.Input);
		}

		/// <summary>
		///		Obtiene el valor de un parámetro o un valor predeterminado si es null
		/// </summary>
		public object iisNull(string name)
		{
			ParameterDB parameter = Search(name);

				if (parameter.Value == DBNull.Value)
					return null;
				else
					return parameter.Value;
		}

		/// <summary>
		///		Normaliza las fechas
		/// </summary>
		public void NormalizeDates(ref DateTime? start, ref DateTime? end)
		{ 
			// Cambia las fechas
			SwapDates(ref start, ref end);
			// Normaliza las fechas de inicio o fin
			if (start != null)
				start = GetNormalizedDate(start, 0, 0, 0);
			if (end != null)
				end = GetNormalizedDate(end, 23, 59, 59);
		}

		/// <summary>
		///		Intercambia dos fechas para un filtro
		/// </summary>
		private void SwapDates(ref DateTime? from, ref DateTime? to)
		{
			if (from != null && to != null && from > to)
			{
				DateTime? value = from;

					from = to;
					to = value;
			}
		}

		/// <summary>
		///		Obtiene una fecha normalizada al inicio o fin del día
		/// </summary>
		private DateTime? GetNormalizedDate(DateTime? value, int hour, int minute, int second)
		{
			DateTime normalized = value ?? DateTime.Now;

				// Devuelve la fecha normalizada
				return new DateTime(normalized.Year, normalized.Month, normalized.Day, hour, minute, second);
		}

		/// <summary>
		///		Indizador de la colección por el nombre de parámetro
		/// </summary>
		public ParameterDB this[string name]
		{
			get { return Search(name); }
		}
	}
}