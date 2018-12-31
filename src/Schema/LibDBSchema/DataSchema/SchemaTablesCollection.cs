using System;

namespace Bau.Libraries.LibDBSchema.DataSchema
{
	/// <summary>
	///		Colección de tablas
	/// </summary>
	public class SchemaTablesCollection : SchemaItemsCollection<SchemaTable>
	{
		public SchemaTablesCollection(Schema parent) : base(parent)
		{
		}
	}
}
