using System;

namespace Bau.Libraries.LibDbScripts.Manager.Processor.Sentences
{
	/// <summary>
	///		Sentencia para declaración de una variable
	/// </summary>
	internal class SentenceDeclare : SentenceBase
	{
		/// <summary>
		///		Tipo de variable
		/// </summary>
		internal enum VariableType
		{
			/// <summary>Desconocida: se utiliza cuando el compilador infiere el tipo de la variable</summary>
			Unknown,
			/// <summary>Cadena</summary>
			String,
			/// <summary>Valor numérico</summary>
			Numeric,
			/// <summary>Valor lógico</summary>
			Boolean,
			/// <summary>Valor de fecha</summary>
			Date
		}

		/// <summary>
		///		Nombre de variable
		/// </summary>
		internal string Name { get; set; }

		/// <summary>
		///		Tipo de la variable
		/// </summary>
		internal VariableType Type { get; set; }

		/// <summary>
		///		Valor que se debe asignar a la variable
		/// </summary>
		internal object Value { get; set; }
	}
}
