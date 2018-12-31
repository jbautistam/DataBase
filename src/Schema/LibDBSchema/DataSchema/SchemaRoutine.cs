using System;

namespace Bau.Libraries.LibDBSchema.DataSchema
{
	/// <summary>
	///		Clase que mantiene los datos de una rutina
	/// </summary>
	public class SchemaRoutine : SchemaItem
	{ 
		// Enumerados
		/// <summary>
		///		Tipo de rutina
		/// </summary>
		public enum RoutineType
		{
			/// <summary>Desconocido. No se deber�a utilizar</summary>
			Unknown,
			/// <summary>Procedimiento</summary>
			Procedure,
			/// <summary>Funci�n</summary>
			Function
		}

		public SchemaRoutine(Schema parent) : base(parent)
		{
		}

		/// <summary>
		///		Definici�n de la rutina
		/// </summary>
		public string Definition { get; set; }

		/// <summary>
		///		Tipo de la rutina
		/// </summary>
		public RoutineType Type { get; set; }
	}
}
