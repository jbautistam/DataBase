using System;

namespace Bau.Libraries.LibDBSchema.DataSchema
{
	/// <summary>
	///		Colecci�n de columnas
	/// </summary>
	public class SchemaColumnsCollection : SchemaItemsCollection<SchemaColumn>
	{
		public SchemaColumnsCollection(Schema parent) : base(parent)
		{
		}

		/// <summary>
		///		Ordena las columnas por su posici�n
		/// </summary>
		public void SortByOrdinalPosition()
		{
			Sort((first, second) => first.OrdinalPosition.CompareTo(second.OrdinalPosition));
		}
	}
}