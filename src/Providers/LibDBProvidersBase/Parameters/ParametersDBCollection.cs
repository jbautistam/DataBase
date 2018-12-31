using System;
using System.Data;

namespace Bau.Libraries.LibDBProvidersBase.Parameters
{
	/// <summary>
	///		Clase que almacena los par�metros de un comando
	/// </summary>
	public class ParametersDBCollection : System.Collections.Generic.List<ParameterDB>
	{
		/// <summary>
		///		A�ade un par�metro a la colecci�n de par�metros del comando
		/// </summary>
		public void Add(string name, string value, int length, ParameterDirection direction = ParameterDirection.Input)
		{ 
			// Corta la cadena si supera la longitud m�xima
			if (!string.IsNullOrEmpty(value) && value.Length > length)
				value = value.Substring(0, length);
			// A�ade el par�metro
			Add(new ParameterDB(name, value, direction, length));
		}

		/// <summary>
		///		A�ade un par�metro a la colecci�n de par�metros del comando
		/// </summary>
		public void Add(string name, object value, ParameterDirection direction = ParameterDirection.Input, bool skipValueZero = true)
		{
			if (value is Enum)
			{
				int? intValue = (int) ((object) value);

					// Convierte el valor 0 del enumerado en un valor NULL
					if (skipValueZero && (intValue ?? 0) == 0)
						intValue = null;
					// A�ade el valor del par�metro
					Add(new ParameterDB(name, intValue, direction));
			}
			else
				Add(new ParameterDB(name, value, direction));
		}

		/// <summary>
		///		A�ade un par�metro a la colecci�n de par�metros del comando
		/// </summary>
		public void Add(string name, byte[] buffer, ParameterDirection direction = ParameterDirection.Input)
		{
			Add(new ParameterDB(name, buffer, direction));
		}

		/// <summary>
		///		A�ade un par�metro de tipo Text
		/// </summary>
		public void AddText(string name, string value, ParameterDirection direction = ParameterDirection.Input)
		{
			ParameterDB parameter = new ParameterDB(name, value, direction);

				// Indica que es un par�metro de texto
				parameter.IsText = true;
				// A�ade el par�metro a la colecci�n
				Add(parameter);
		}

		/// <summary>
		///		Obtiene un par�metro de la colecci�n a partir del nombre, si no lo encuentra devuelve un par�metro vac�o
		/// </summary>
		public ParameterDB Search(string name)
		{ 
			// Recorre la colecci�n de par�metros buscando el elemento adecuado
			foreach (ParameterDB parameter in this)
				if (parameter.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
					return parameter;
			// Devuelve un objeto vac�o
			return new ParameterDB(name, null, ParameterDirection.Input);
		}

		/// <summary>
		///		Obtiene el valor de un par�metro o un valor predeterminado si es null
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
		///		Obtiene una fecha normalizada al inicio o fin del d�a
		/// </summary>
		private DateTime? GetNormalizedDate(DateTime? value, int hour, int minute, int second)
		{
			DateTime normalized = value ?? DateTime.Now;

				// Devuelve la fecha normalizada
				return new DateTime(normalized.Year, normalized.Month, normalized.Day, hour, minute, second);
		}

		/// <summary>
		///		Indizador de la colecci�n por el nombre de par�metro
		/// </summary>
		public ParameterDB this[string name]
		{
			get { return Search(name); }
		}
	}
}