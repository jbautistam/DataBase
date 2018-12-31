using System;

namespace Bau.Libraries.LibDBSchema.DataSchema
{
	/// <summary>
	///		Clase con los datos de una restricci�n
	/// </summary>
	public class SchemaConstraint : SchemaItem
	{ 
		// Enumerados
		/// <summary>
		///		Tipo de restricci�n
		/// </summary>
		public enum Constratype
		{
			/// <summary>Desconocido</summary>
			Unknown,
			/// <summary>Clave primaria</summary>
			PrimaryKey,
			/// <summary>Clave for�nea</summary>
			ForeignKey,
			/// <summary>Unico</summary>
			Unique
		}

		public SchemaConstraint(Schema parent) : base(parent)
		{
		}

		/// <summary>
		///		Tabla
		/// </summary>
		public string Table { get; set; }

		/// <summary>
		///		Columna
		/// </summary>
		public string Column { get; set; }

		/// <summary>
		///		Tipo de restricci�n
		/// </summary>
		public Constratype Type { get; set; }

		/// <summary>
		///		Posici�n de la restricci�n
		/// </summary>
		public int Position { get; set; }
	}
}