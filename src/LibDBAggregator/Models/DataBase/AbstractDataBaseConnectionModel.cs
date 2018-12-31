using System;
using System.Collections.Generic;

using Bau.Libraries.LibDBProvidersBase.Parameters;

namespace Bau.Libraries.LibDBAggregator.Models.DataBase
{
	/// <summary>
	///		Clase base para las conexiones a base de datos
	/// </summary>
	public abstract class AbstractDataBaseConnectionModel : AbstractConnectionModel
	{
		/// <summary>
		///		Convierte los parámetros
		/// </summary>
		protected ParametersDBCollection ConvertParameters(Dictionary<string, object> parameters)
		{
			var parametersDB = new ParametersDBCollection();

				// Convierte los parámetros
				if (parameters != null)
					foreach (KeyValuePair<string, object> parameter in parameters)
						parametersDB.Add(new ParameterDB(parameter.Key, parameter.Value, System.Data.ParameterDirection.Input));
				// Devuelve la colección
				return parametersDB;
		}
	}
}
