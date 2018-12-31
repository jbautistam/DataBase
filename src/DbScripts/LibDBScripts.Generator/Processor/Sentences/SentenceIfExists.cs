using System;

namespace Bau.Libraries.LibDbScripts.Generator.Processor.Sentences
{
	/// <summary>
	///		Sentencia de ejecución de una comprobación de datos
	/// </summary>
	internal class SentenceIfExists : SentenceBaseProvider
	{
		/// <summary>
		///		Sentencias a ejecutar si existe el registro
		/// </summary>
		internal SentenceCollection SentencesThen { get; } = new SentenceCollection();

		/// <summary>
		///		Sentencias a ejecutar si no existe el registro
		/// </summary>
		internal SentenceCollection SentencesElse { get; } = new SentenceCollection();
	}
}
