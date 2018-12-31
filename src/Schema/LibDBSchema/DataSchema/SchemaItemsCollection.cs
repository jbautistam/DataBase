using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibDBSchema.DataSchema
{
	/// <summary>
	///		Colección base de esquema
	/// </summary>
	public abstract class SchemaItemsCollection<TypeData> : List<TypeData> where TypeData : SchemaItem
	{
		public SchemaItemsCollection(Schema parent)
		{
			Parent = parent;
		}

		/// <summary>
		///		Busca un elemento
		/// </summary>
		public virtual TypeData Search(string fullName)
		{ 
			// Busca el elemento
			foreach (TypeData item in this)
				if (item.FullName == fullName)
					return item;
			// Si ha llegado hasta aquí es porque no ha encontrado nada
			return null;
		}

		/// <summary>
		///		Elimina un elemento por su nombre completo
		/// </summary>
		public void RemoveByFullName(string fullName)
		{
			TypeData item = Search(fullName);

				// Si se ha encontrado, lo elimina
				if (item != null)
					Remove(item);
		}

		/// <summary>
		///		Esquema al que pertenece la colección
		/// </summary>
		public Schema Parent { get; }
	}
}
