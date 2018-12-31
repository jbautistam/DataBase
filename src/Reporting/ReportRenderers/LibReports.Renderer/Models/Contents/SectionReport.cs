using System;

using Bau.Libraries.LibReports.Renderer.Models.Base;

namespace Bau.Libraries.LibReports.Renderer.Models.Contents
{
	/// <summary>
	///		Definición de una sección
	/// </summary>
	public class SectionReport : ContentCommandReportBase
	{
		public SectionReport(ContentReportBase parent) : base(parent) { }

		/// <summary>
		///		Clona la definición de la sección
		/// </summary>
		public SectionReport CloneDefinition(ContentReportBase parent)
		{
			SectionReport section = new SectionReport(parent);

				// Clona los datos básicos
				base.CloneToTarget(section);
				// Devuelve la sección clonada
				return section;
		}

		/// <summary>
		///		Contenido
		/// </summary>
		public ClassBaseCollection<ContentReportBase> Contents { get; } = new ClassBaseCollection<ContentReportBase>();
	}
}
