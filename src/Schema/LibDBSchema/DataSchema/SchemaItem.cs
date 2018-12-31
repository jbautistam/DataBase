using System;

namespace Bau.Libraries.LibDBSchema.DataSchema
{
	/// <summary>
	///		Clase base para los elementos del esquema
	/// </summary>
	public abstract class SchemaItem
	{
		public SchemaItem(Schema schema)
		{
			Parent = schema;
		}

		/// <summary>
		///		Normaliza una cadena
		/// </summary>
		private string Normalize(string value)
		{ 
			// Normaliza los valores
			if (!string.IsNullOrEmpty(value))
			{
				value = value.Replace(":", "_");
				value = value.Replace("\\", "_");
				value = value.Replace(".", "_");
			}
			// Devuelve la cadena normalizada
			return value;
		}

		/// <summary>
		///		Esquema al que pertenecen los datos
		/// </summary>
		public Schema Parent { get; private set; }

		/// <summary>
		///		Identificador del objeto
		/// </summary>
		public string ID
		{
			get { return $"{Normalize(Catalog)}_{Schema}_{Name}"; }
		}

		/// <summary>
		///		Nombre completo del objeto
		/// </summary>
		public string FullName
		{
			get { return $"{Catalog}.{Schema}.{Name}"; }
		}

		/// <summary>
		///		Catálogo
		/// </summary>
		public string Catalog { get; set; }

		/// <summary>
		///		Nombre de esquema
		/// </summary>
		public string Schema { get; set; }

		/// <summary>
		///		Nombre del elemento
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///		Descripción
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		///		Fecha de creación
		/// </summary>
		public DateTime DateCreated { get; set; }

		/// <summary>
		///		Fecha de modificación
		/// </summary>
		public DateTime DateUpdated { get; set; }
	}
}
