using System;

namespace Bau.Libraries.LibDbScripts.Manager.Processor.Sentences
{
	/// <summary>
	///		Clase con los datos de una excepción
	/// </summary>
	internal class SentenceException : SentenceBase
	{
		/// <summary>
		///		Mensaje de error
		/// </summary>
		internal string Message { get; set; }
	}
}
