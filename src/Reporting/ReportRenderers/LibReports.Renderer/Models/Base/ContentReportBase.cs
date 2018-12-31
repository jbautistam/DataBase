using System;

using Bau.Libraries.LibCommonHelper.Extensors;

namespace Bau.Libraries.LibReports.Renderer.Models.Base
{
	/// <summary>
	///		Base para el contenido de un informe
	/// </summary>
	public abstract class ContentReportBase : ClassBase
	{
		public ContentReportBase(ContentReportBase parent)
		{
			Parent = parent;
			ClassId = null;
			StyleClass = new Styles.StyleReport(parent, new Guid().ToString());
		}

		/// <summary>
		///		Rutina común para clonar los datos compartidos
		/// </summary>
		protected void CloneToTarget(ContentReportBase target)
		{
			target.Parent = Parent;
			target.ClassId = ClassId;
			target.StyleClass = StyleClass.Clone();
			target.NewPageBefore = NewPageBefore;
			target.NewPageAfter = NewPageAfter;
		}

		/// <summary>
		///		Obtiene el estilo del elemento
		/// </summary>
		public Styles.StyleReport CompactStyle(Report report)
		{
			Styles.StyleReport style = new Styles.StyleReport(null, null);

				// Obtiene el estilo del elemento padre
				//if (Parent != null)
				//  style = Parent.CompactStyle(report);
				// Obtiene el estilo a partir del classId
				if (!ClassId.IsEmpty())
					style = report.Styles.Search(ClassId);
				else
					style = report.Styles.SearchDefault();
				// Mezcla el estilo con el actual
				if (StyleClass != null)
					style = StyleClass.Merge(style);
				// Devuelve el estilo
				return style;
		}

		/// <summary>
		///		Elemento padre
		/// </summary>
		public ContentReportBase Parent { get; set; }

		/// <summary>
		///		ID de la clase del estilo
		/// </summary>
		public string ClassId { get; set; }

		/// <summary>
		///		Estilo del contenido
		/// </summary>
		public Styles.StyleReport StyleClass { get; set; }

		/// <summary>
		///		Indica si se debe añadir una página antes del contenido
		/// </summary>
		public bool NewPageBefore { get; set; }

		/// <summary>
		///		Indica si se debe añadir una página después del contenido
		/// </summary>
		public bool NewPageAfter { get; set; }
	}
}
