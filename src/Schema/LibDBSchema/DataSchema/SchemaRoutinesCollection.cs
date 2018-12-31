using System;

namespace Bau.Libraries.LibDBSchema.DataSchema
{
	/// <summary>
	///		Colecci�n de rutinas
	/// </summary>
	public class SchemaRoutinesCollection : SchemaItemsCollection<SchemaRoutine>
	{
		public SchemaRoutinesCollection(Schema parent) : base(parent)
		{
		}

		/// <summary>
		///		Obtiene una colecci�n de procedimientos
		/// </summary>
		public SchemaRoutinesCollection SearchProcedures()
		{
			return SearchByType(true);
		}

		/// <summary>
		///		Obtiene una colecci�n de funciones
		/// </summary>
		public SchemaRoutinesCollection SearchFunctions()
		{
			return SearchByType(false);
		}

		/// <summary>
		///		Obtiene una colecci�n de procedimientos o funciones
		/// </summary>
		private SchemaRoutinesCollection SearchByType(bool procedures)
		{
			SchemaRoutinesCollection routines = new SchemaRoutinesCollection(Parent);

				// Recorre la colecci�n buscando los elementos
				foreach (SchemaRoutine routine in this)
					if ((procedures && routine.Type == SchemaRoutine.RoutineType.Procedure) ||
							(!procedures && routine.Type == SchemaRoutine.RoutineType.Function))
						routines.Add(routine);
				// Devuelve la colecci�n
				return routines;
		}
	}
}
