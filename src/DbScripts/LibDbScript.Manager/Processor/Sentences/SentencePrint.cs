using System;

namespace Bau.Libraries.LibDbScripts.Manager.Processor.Sentences
{
	/// <summary>
	///		Sentencia para imprimir un mensaje
	/// </summary>
	internal class SentencePrint : SentenceBase
	{
		/// <summary>
		///		Imprime un mensaje
		/// </summary>
		internal string Message { get; set; }
	}
}
