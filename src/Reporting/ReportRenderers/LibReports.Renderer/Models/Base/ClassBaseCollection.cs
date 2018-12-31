using System;
using System.Collections.Generic;
using System.Linq;

namespace Bau.Libraries.LibReports.Renderer.Models.Base
{
	/// <summary>
	///		Colección de <see cref="ClassBase"/>
	/// </summary>
	public class ClassBaseCollection<TypeData> : List<TypeData> where TypeData : ClassBase
	{
		/// <summary>
		///		Añade un elemento a la colección (sólo si no está vacío)
		/// </summary>
		public new void Add(TypeData content)
		{
			if (content != null)
				base.Add(content);
		}

		/// <summary>
		///		Busca un elemento a partir de su ID
		/// </summary>
		public TypeData Search(string id)
		{
			return this.FirstOrDefault((data) => data.ID.Equals(id, StringComparison.CurrentCultureIgnoreCase));
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
		public TypeData LastItem
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
		public TypeData this[string id]
		{
			get { return Search(id); }
			set
			{
				TypeData data = Search(id);

					// Asigna los datos o añade el elemento a la colección
					if (data != null)
						data = value;
					else
						Add(value);
			}
		}
	}
}
