using System;

namespace Bau.Libraries.LibDbScripts.Manager.Processor.Sentences
{
	/// <summary>
	///		Sentencia de ejecución de un bucle por cada registro leido del proveedor
	/// </summary>
	internal class SentenceForEach : SentenceBaseProvider
	{
		/// <summary>
		///		Instrucciones que se deben ejecutar cuando se encuentra algún dato
		/// </summary>
		internal SentenceCollection SentencesWithData { get; } = new SentenceCollection();

		/// <summary>
		///		Instrucciones que se deben ejecutar cuando no se encuentra ningún dato
		/// </summary>
		internal SentenceCollection SentencesEmptyData { get; } = new SentenceCollection();
	}
}
