using System;

using Bau.Libraries.LibDBProvidersBase;
using Bau.Libraries.LibDBSchema;

namespace Bau.Libraries.LibDBSchemaProvider.Interfaces
{
	/// <summary>
	///		Interface para los proveedores que puedan leer un esquema de datos
	/// </summary>
	public interface ISchemaProvider<TypeData> where TypeData : IConnectionString
	{
		/// <summary>
		///		Carga el esquema de una base de datos
		/// </summary>
		Schema LoadSchema(TypeData connectionString);

		/// <summary>
		///		Carga las tablas del esquema de una base de datos
		/// </summary>
		void LoadSchemaTables(Schema schema, TypeData connectionString);

		/// <summary>
		///		Carga las vistas del esquema de una base de datos
		/// </summary>
		void LoadSchemaViews(Schema schema, TypeData connectionString);

		/// <summary>
		///		Carga las rutinas del esquema de una base de datos
		/// </summary>
		void LoadSchemaRoutines(Schema schema, TypeData connectionString);
	}
}