using System;

namespace Bau.Libraries.LibDbProviders.Base.Schema
{
	/// <summary>
	///		Clase base para los datos del modelo
	/// </summary>
	public abstract class BaseSchemaDbModel
	{
		/// <summary>
		///		Esquema de la tabla
		/// </summary>
		public string Schema { get; set; }

		/// <summary>
		///		Catálogo
		/// </summary>
		public string Catalog { get; set; }

		/// <summary>
		///		Nombre completo
		/// </summary>
		public string FullName
		{
			get { return $"{Catalog}.{Schema}.{Name}"; }
		}


		/// <summary>
		///		Nombre de la tabla
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///		Descripción de la tabla
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		///		Fecha de creación
		/// </summary>
		public DateTime CreatedAt { get; set; }

		/// <summary>
		///		Fecha de modificación
		/// </summary>
		public DateTime UpdatedAt { get; set; }
	}
}
