using System;

namespace Bau.Libraries.LibDBSchema.DataSchema
{
	/// <summary>
	///		Clase con los datos de una tabla
	/// </summary>
	public class SchemaTable : SchemaItem
	{ 
		// Enumerados
		/// <summary>
		///		Tipo de tabla
		/// </summary>
		public enum TableType
		{
			/// <summary>Desconocido. No se debería utilizar</summary>
			Unknown,
			/// <summary>Tabla</summary>
			Table,
			/// <summary>Vista</summary>
			View
		}

		public SchemaTable(Schema parent) : base(parent)
		{
			Columns = new SchemaColumnsCollection(parent);
			Constraints = new SchemaConstraintsCollection(parent);
		}

		/// <summary>
		///		Tipo
		/// </summary>
		public TableType Type { get; set; }

		/// <summary>
		///		Columnas
		/// </summary>
		public SchemaColumnsCollection Columns { get; }

		/// <summary>
		///		Restricciones
		/// </summary>
		public SchemaConstraintsCollection Constraints { get; }
	}
}