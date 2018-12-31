using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibDbScripts.Generator.Processor.Sentences.Parameters
{
	/// <summary>
	///		Sentencia que se envía al proveedor
	/// </summary>
	internal class ProviderSentenceModel
	{
		/// <summary>
		///		Clona la sentencia
		/// </summary>
		internal ProviderSentenceModel Clone()
		{
			ProviderSentenceModel target = new ProviderSentenceModel();

				// Añade los comandos
				foreach (ProviderCommandModel command in Commands)
					target.Commands.Add(new ProviderCommandModel
													{
														Name = command.Name,
														Value = command.Value
													}
										);
				// Añade los filtros
				foreach (FilterModel filter in Filters)
					target.Filters.Add(filter.Clone());
				// Devuelve el comando clonado
				return target;
		}

		/// <summary>
		///		Comandos
		/// </summary>
		internal List<ProviderCommandModel> Commands { get; } = new List<ProviderCommandModel>();

		/// <summary>
		///		Filtros
		/// </summary>
		internal List<FilterModel> Filters { get; } = new List<FilterModel>();
	}
}
