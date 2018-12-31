using System;
using System.Collections.Generic;

using Bau.Libraries.LibDataStructures.Base;

namespace Bau.Libraries.LibDBAggregator.Models
{
	/// <summary>
	///		Conexión a un origen de datos
	/// </summary>
	public abstract class AbstractConnectionModel : BaseExtendedModel
	{
		/// <summary>
		///		Obtiene un <see cref="System.Data.DataTable"/>
		/// </summary>
		public abstract System.Data.DataTable GetDataTable(string command, Dictionary<string, object> parameters);
	}
}
