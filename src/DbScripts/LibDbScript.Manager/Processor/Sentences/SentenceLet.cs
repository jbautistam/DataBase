using System;

namespace Bau.Libraries.LibDbScripts.Manager.Processor.Sentences
{
	/// <summary>
	///		Sentencia para ejecución de una expresión
	/// </summary>
	internal class SentenceLet : SentenceBase
	{
		/// <summary>
		///		Tipo de la variable de salida: sólo es necesario cuando no está definida
		/// </summary>
		internal SentenceDeclare.VariableType Type { get; set; }

		/// <summary>
		///		Nombre de variable
		/// </summary>
		internal string Variable { get; set; }

		/// <summary>
		///		Expresión a ejecutar
		/// </summary>
		internal string Expression { get; set; }
	}
}
