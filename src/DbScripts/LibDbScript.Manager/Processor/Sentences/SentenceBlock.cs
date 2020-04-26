using System;

namespace Bau.Libraries.LibDbScripts.Manager.Processor.Sentences
{
	/// <summary>
	///		Bloque de sentencias
	/// </summary>
	internal class SentenceBlock : SentenceBase
	{
		/// <summary>
		///		Nombre / mensaje del bloque
		/// </summary>
		internal string Name { get; set; }

		/// <summary>
		///		Sentencias a ejecutar
		/// </summary>
		internal SentenceCollection Sentences { get; } = new SentenceCollection();
	}
}