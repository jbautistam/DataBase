using System;

namespace Bau.Libraries.LibDbScripts.Generator.Processor.Sentences
{
	/// <summary>
	///		Sentencia de inicio de lote
	/// </summary>
	internal class SentenceDataBatch : SentenceBase
	{
		/// <summary>
		///		Tipo de comando
		/// </summary>
		internal enum BatchCommand
		{
			/// <summary>Arranca una transacción</summary>
			BeginTransaction,
			/// <summary>Confirma una transacción</summary>
			CommitTransaction,
			/// <summary>Deshace la transacción</summary>
			RollbackTransaction
		}

		/// <summary>
		///		Clave del proveedor
		/// </summary>
		internal string ProviderKey { get; set; }

		/// <summary>
		///		Indica el tipo de sentencia
		/// </summary>
		internal BatchCommand Type { get; set; }
	}
}
