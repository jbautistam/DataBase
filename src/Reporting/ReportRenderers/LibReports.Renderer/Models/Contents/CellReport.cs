using System;

using Bau.Libraries.LibReports.Renderer.Models.Base;

namespace Bau.Libraries.LibReports.Renderer.Models.Contents
{
	/// <summary>
	///		Celda de un <see cref="TableReport"/>
	/// </summary>
	public class CellReport : ContentReportBase
	{
		public CellReport(ContentReportBase parent) : base(parent)
		{
			Width = new Styles.UnitStyle();
			RowSpan = 1;
			ColumnSpan = 1;
		}

		/// <summary>
		///		Clona la celda
		/// </summary>
		public CellReport Clone(ContentReportBase parent)
		{
			CellReport cell = new CellReport(parent);

				// Asigna los datos básicos
				base.CloneToTarget(cell);
				// Asigna los datos
				cell.Width = Width.Clone();
				cell.Content = Content;
				cell.Row = Row;
				cell.Column = Column;
				cell.RowSpan = RowSpan;
				cell.ColumnSpan = ColumnSpan;
				// Devuelve el objeto clonado
				return cell;
		}

		/// <summary>
		///		Ancho de la celda
		/// </summary>
		public Styles.UnitStyle Width { get; set; }

		/// <summary>
		///		Contenido de la celda
		/// </summary>
		public ContentReportBase Content { get; set; }

		/// <summary>
		///		Número de fila
		/// </summary>
		public int Row { get; set; }

		/// <summary>
		///		Número de columna
		/// </summary>
		public int Column { get; set; }

		/// <summary>
		///		Span de fila
		/// </summary>
		public int RowSpan { get; set; }

		/// <summary>
		///		Span de columna
		/// </summary>
		public int ColumnSpan { get; set; }
	}
}
