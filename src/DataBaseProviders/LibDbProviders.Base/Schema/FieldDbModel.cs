using System;

namespace Bau.Libraries.LibDbProviders.Base.Schema
{
	/// <summary>
	///		Datos de un campo de base de datos
	/// </summary>
	public class FieldDbModel
	{
		// Enumerados públicos
		/// <summary>
		///		Tipo de campo
		/// </summary>
		public enum Fieldtype
		{
			/// <summary>Desconocido. No se debería utilizar</summary>
			Unknown,
			/// <summary>Cadena</summary>
			String,
			/// <summary>Fecha</summary>
			Date,
			/// <summary>Número entero</summary>
			Integer,
			/// <summary>Número decimal</summary>
			Decimal,
			/// <summary>Valor lógico</summary>
			Boolean
		}

		/// <summary>
		///		Nombre del campo
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///		Tipo del campo
		/// </summary>
		public Fieldtype Type { get; set; }

		/// <summary>
		///		Tipo original del campo en la base de datos
		/// </summary>
		public string DbType { get; set; }

		/// <summary>
		///		Indica si el campo es clave
		/// </summary>
		public bool IsKey { get; set; }

		/// <summary>
		///		Longitud del campo
		/// </summary>
		public int Length { get; set; }

		/// <summary>
		///		Indica si es un campo obligatorio
		/// </summary>
		public bool IsRequired { get; set; }

		/// <summary>
		///		Formato del campo
		/// </summary>
		public string Format { get; set; }
	}
}
