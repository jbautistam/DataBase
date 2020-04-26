using System;

namespace Bau.Libraries.LibDbScripts.Manager.Processor.Sentences
{
	/// <summary>
	///		Sentencia de ejecución de un comando en el proveedor
	/// </summary>
	internal abstract class SentenceBaseProvider : SentenceBase
	{
		/// <summary>
		///		Clave del proveedor sobre el que se ejecuta la sentencia
		/// </summary>
		internal string ProviderKey { get; set; }

		/// <summary>
		///		Comando para ejecución en el proveedor
		/// </summary>
		internal Parameters.ProviderSentenceModel Command { get; } = new Parameters.ProviderSentenceModel();
	}
}
