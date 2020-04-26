using System;
using System.Collections.Generic;

using Bau.Libraries.Aggregator.Providers.Base;

namespace Bau.Libraries.LibDbScripts.Manager.Processor.Context
{
	/// <summary>
	///		Clase con los datos de un lote pendiente de ejecución
	/// </summary>
	internal class BatchTransactionModel
	{
		public BatchTransactionModel(IDataProvider provider)
		{
			Provider = provider;
		}

		/// <summary>
		///		Proveedor sobre el que se ejecuta la transacción
		/// </summary>
		internal IDataProvider Provider { get; }

		/// <summary>
		///		Comandos asociados a la transacción
		/// </summary>
		internal List<DataProviderCommand> Commands { get; } = new List<DataProviderCommand>();
	}
}
