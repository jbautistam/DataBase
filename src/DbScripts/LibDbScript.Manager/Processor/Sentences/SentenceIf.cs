using System;

namespace Bau.Libraries.LibDbScripts.Manager.Processor.Sentences
{
	/// <summary>
	///		Sentencia de ejecución de una condición
	/// </summary>
	internal class SentenceIf : SentenceBase
	{
		/// <summary>
		///		Condición
		/// </summary>
		internal string Condition { get; set; }

		/// <summary>
		///		Sentencias a ejecutar si el resultado de la condición es verdadero
		/// </summary>
		internal SentenceCollection SentencesThen { get; } = new SentenceCollection();

		/// <summary>
		///		Sentencias a ejecutar si el resultado de la condición es false
		/// </summary>
		internal SentenceCollection SentencesElse { get; } = new SentenceCollection();
	}
}
