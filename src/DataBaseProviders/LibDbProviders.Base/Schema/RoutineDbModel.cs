using System;

namespace Bau.Libraries.LibDbProviders.Base.Schema
{
	/// <summary>
	///		Clase con los datos de una rutina
	/// </summary>
	public class RoutineDbModel
	{
		/// <summary>
		///		Esquema al que está asociada la rutina
		/// </summary>
		public string Schema { get; set; }

		/// <summary>
		///		Nombre de la rutina
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///		Contenido de la rutina 
		/// </summary>
		public string Content { get; set; }
	}
}
