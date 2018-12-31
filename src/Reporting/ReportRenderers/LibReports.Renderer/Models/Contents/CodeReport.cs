using System;

namespace Bau.Libraries.LibReports.Renderer.Models.Contents
{
	/// <summary>
	///		Clase con los datos de una sentencia de código
	/// </summary>
	public class CodeReport : Base.ContentReportBase
	{
		public CodeReport(Base.ContentReportBase parent) : base(parent) { }

		/// <summary>
		///		Clona el código
		/// </summary>
		internal CodeReport Clone()
		{
			CodeReport code = new CodeReport(Parent);

				// Clona el código
				code.Code = Code;
				// Devuelve el dato clonado
				return code;
		}

		/// <summary>
		///		Código
		/// </summary>
		public string Code { get; set; }
	}
}
