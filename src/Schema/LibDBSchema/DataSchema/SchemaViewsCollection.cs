using System;

namespace Bau.Libraries.LibDBSchema.DataSchema
{
	/// <summary>
	///		Colección de vistas
	/// </summary>
	public class SchemaViewsCollection : SchemaItemsCollection<SchemaView>
	{
		public SchemaViewsCollection(Schema parent) : base(parent)
		{
		}
	}
}
