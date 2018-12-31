using System;
using System.Collections.Generic;
using System.Linq;

namespace Bau.Libraries.LibDbProviders.Base.Schema
{
	/// <summary>
	///		Clase con los datos de una tabla de base de datos
	/// </summary>
	public class TableDbModel
	{
		/// <summary>
		///		Añade un campo a la tabla
		/// </summary>
		public void AddField(string name, FieldDbModel.Fieldtype type, string dbType, int length, bool isPrimaryKey, bool isRequired)
		{
			FieldDbModel field = Fields.FirstOrDefault(item => item.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));

				// Si no existía el campo, se crea
				if (field == null)
				{
					field = new FieldDbModel { Name = name };
					Fields.Add(field);
				}
				// Asigna las propiedades
				field.Type = type;
				field.DbType = dbType;
				field.Length = length;
				field.IsKey = isPrimaryKey;
				field.IsRequired = isRequired;
		}

		/// <summary>
		///		Esquema de la tabla
		/// </summary>
		public string Schema { get; set; }

		/// <summary>
		///		Nombre de la tabla
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///		Campos de la tabla
		/// </summary>
		public List<FieldDbModel> Fields { get; } = new List<FieldDbModel>();
	}
}
