using System;

namespace Bau.Libraries.LibDbScripts.Manager.Processor.Sentences
{
	/// <summary>
	///		Sentencia de ejecución de un bucle 
	/// </summary>
	internal class SentenceFor : SentenceBase
	{
		/// <summary>
		///		Normaliza el inicio / fin de la sentencia para evitar bucles infinitos
		/// </summary>
		internal void Normalize()
		{
			// Normaliza el incremento
			if (Step == 0)
				Step = 1;
			// Si el incremento es positivo, el inicio debe ser menor que el final y viceversa
			if ((Step > 0 && Start > End) || (Step < 0 && Start < End))
			{
				int inter = Start;

					Start = End;
					End = inter;
			}
		}

		/// <summary>
		///		Nombre de variable
		/// </summary>
		internal string Variable { get; set; }

		/// <summary>
		///		Valor inicial
		/// </summary>
		internal int Start { get; set; }

		/// <summary>
		///		Valor final
		/// </summary>
		internal int End { get; set; }

		/// <summary>
		///		Valor de incremento
		/// </summary>
		internal int Step { get; set; }

		/// <summary>
		///		Sentencias a ejecutar
		/// </summary>
		internal SentenceCollection Sentences { get; } = new SentenceCollection();
	}
}
