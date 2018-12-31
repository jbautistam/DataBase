using System;

namespace Bau.Libraries.LibDBSchema.DataSchema
{
	/// <summary>
	///		Colección de <see cref="SchemaTrigger"/>
	/// </summary>
	public class SchemaTriggersCollection : SchemaItemsCollection<SchemaTrigger>
	{
		public SchemaTriggersCollection(Schema parent) : base(parent)
		{
		}

		/// <summary>
		///		Busca los triggers de una tabla
		/// </summary>
		public SchemaTriggersCollection SearchByTable(string table)
		{
			SchemaTriggersCollection triggers = new SchemaTriggersCollection(base.Parent);

				// Recorre la colección
				foreach (SchemaTrigger trigger in this)
					if (trigger.Table.Equals(table, StringComparison.CurrentCultureIgnoreCase))
						triggers.Add(trigger);
				// Devuelve la colección de triggers encontrados
				return triggers;
		}
	}
}
