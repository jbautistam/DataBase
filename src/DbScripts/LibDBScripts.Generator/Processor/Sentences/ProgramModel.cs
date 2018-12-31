using System;

namespace Bau.Libraries.LibDbScripts.Generator.Processor.Sentences
{
	/// <summary>
	///		Clase con las instrucciones de un programa
	/// </summary>
	internal class ProgramModel
	{
		/// <summary>
		///		Instrucciones del programa
		/// </summary>
		internal SentenceCollection Sentences { get; } = new SentenceCollection();
	}
}
