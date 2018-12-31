using System;

using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibReports.Renderer.Models.Contents;

namespace Bau.Libraries.LibReports.Renderer.Parser.Tools
{
	/// <summary>
	///		Intérprete de <see cref="CodeReport"/>
	/// </summary>
	internal class CodeParser : ParserBase<Models.Contents.CodeReport>
	{
		internal CodeParser(ReportParser reportParser) : base(reportParser) { }

		/// <summary>
		///		Interpreta el contenido del nodo
		/// </summary>
		protected override CodeReport ParseInner(MLNode nodeML)
		{
			CodeReport objFormula = new CodeReport(Parent);

				// Asigna el contenido a la fórmula
				objFormula.Code = nodeML.Value;
				// Devuelve la formula
				return objFormula;
		}
	}
}
