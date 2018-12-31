using System;

using Bau.Libraries.LibReports.Renderer.Models.Base;

namespace Bau.Libraries.LibReports.Renderer.Models.Contents
{
	/// <summary>
	///		Sección de informe condicional
	/// </summary>
	public class ConditionalSectionReport : ContentReportBase
	{
		public ConditionalSectionReport(ContentReportBase parent) : base(parent)
		{
		}

		/// <summary>
		///		Clona la celda
		/// </summary>
		public ConditionalSectionReport Clone(ContentReportBase parent)
		{
			ConditionalSectionReport conditional = new ConditionalSectionReport(parent);

				// Asigna los datos básicos
				base.CloneToTarget(conditional);
				// Clona los objetos hijos
				conditional.ThenContent = ThenContent;
				conditional.ElseContent = ElseContent;
				// Devuelve el objeto clonado
				return conditional;
		}

		/// <summary>
		///		Condición
		/// </summary>
		public string Condition { get; set; }

		/// <summary>
		///		Contenido de la sección condicional cuando la condición es cierta
		/// </summary>
		public ContentReportBase ThenContent { get; set; }

		/// <summary>
		///		Contenido de la celda condicional cuando la condición es false
		/// </summary>
		public ContentReportBase ElseContent { get; set; }
	}
}
