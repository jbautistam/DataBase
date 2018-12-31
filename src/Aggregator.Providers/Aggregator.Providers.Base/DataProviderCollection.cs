using System;
using System.Collections.Generic;
using System.Linq;

namespace Bau.Libraries.Aggregator.Providers.Base
{
	/// <summary>
	///		Colección de <see cref="IDataProvider"/>
	/// </summary>
	public class DataProviderCollection : List<IDataProvider>
	{
		/// <summary>
		///		Añade un elemento a la colección (sólo si no está vacío)
		/// </summary>
		public new void Add(IDataProvider content)
		{
			if (content != null)
				base.Add(content);
		}

		/// <summary>
		///		Busca un elemento a partir de su ID
		/// </summary>
		public IDataProvider Search(string key)
		{
			if (string.IsNullOrEmpty(key) && Count > 0)
				return this[0];
			else
				return this.FirstOrDefault(data => data.Key.Equals(key, StringComparison.CurrentCultureIgnoreCase));
		}

		/// <summary>
		///		Comprueba si existe un parámetro en la lista
		/// </summary>
		public bool Exists(string id)
		{
			return Search(id) != null;
		}

		/// <summary>
		///		Ultimo elemento añadido en la colección
		/// </summary>
		public IDataProvider LastItem
		{
			get
			{
				if (Count > 0)
					return this[Count - 1];
				else
					return null;
			}
		}

		/// <summary>
		///		Indizador por ID
		/// </summary>
		public IDataProvider this[string id]
		{
			get { return Search(id); }
			set
			{
				IDataProvider data = Search(id);

					// Asigna los datos o añade el elemento a la colección
					if (data != null)
						data = value;
					else
						Add(value);
			}
		}
	}
}
