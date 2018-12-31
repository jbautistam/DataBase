using System;

using Bau.Libraries.LibCommonHelper.Extensors;

namespace Bau.Libraries.LibReports.Renderer.Models.Base
{
	/// <summary>
	///		Clase base para los objetos de la librería
	/// </summary>
	public class ClassBase
	{
		/// <summary>
		///		Comprueba si el proveedor de datos está vacío
		/// </summary>
		public virtual bool IsEmpty()
		{
			return ID.IsEmpty() || Guid.TryParse(ID, out Guid objGuid);
		}

		/// <summary>
		///		ID del objeto
		/// </summary>
		public virtual string ID { get; set; } = Guid.NewGuid().ToString();
	}
}
