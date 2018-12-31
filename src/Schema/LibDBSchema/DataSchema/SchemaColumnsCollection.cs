using System;

namespace Bau.Libraries.LibDBSchema.DataSchema
{
	/// <summary>
	///		Colección de columnas
	/// </summary>
	public class SchemaColumnsCollection : SchemaItemsCollection<SchemaColumn>
	{
		public SchemaColumnsCollection(Schema parent) : base(parent)
		{
		}

		/// <summary>
		///		Ordena las columnas por su posición
		/// </summary>
		public void SortByOrdinalPosition()
		{
			Sort((first, second) => first.OrdinalPosition.CompareTo(second.OrdinalPosition));
		}
	}
}