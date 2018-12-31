using System;
using System.Collections.Generic;

using Bau.Libraries.LibReports.Renderer.Models.Base;

namespace Bau.Libraries.LibReports.Renderer.Models.Contents
{
	/// <summary>
	///		Tabla colocada en horizontal
	/// </summary>
	public class HorizontalTableReport : ContentCommandReportBase
	{
		public HorizontalTableReport(ContentReportBase parent) : base(parent)
		{
			CellPadding = new Styles.MarginStyleReport();
			ColumnsWidth = new List<Styles.UnitStyle>();
			Body = new CellsReportCollection();
		}

		/// <summary>
		///		Clona la definición
		/// </summary>
		public HorizontalTableReport CloneDefinition(ContentReportBase parent)
		{
			HorizontalTableReport table = new HorizontalTableReport(parent);

				// Clona los datos básicos
				base.CloneToTarget(table);
				// Clona el resto de parámetros
				table.CellPadding = CellPadding.Clone();
				table.TablesPerRow = TablesPerRow;
				// Clona los anchos
				foreach (Styles.UnitStyle objUnit in ColumnsWidth)
					table.ColumnsWidth.Add(objUnit);
				// Devuelve la definicón
				return table;
		}

		/// <summary>
		///		Ordena los datos de la tabla
		/// </summary>
		public void Sort()
		{
			Body.SortByPosition();
		}

		/// <summary>
		///		Padding de las celdas
		/// </summary>
		public Styles.MarginStyleReport CellPadding { get; set; }

		/// <summary>
		///		Columnas (en realidad tablas) que aparecen en cada línea en horizontal
		/// </summary>
		public int TablesPerRow { get; set; }

		/// <summary>
		///		Ancho de las columnas
		/// </summary>
		public List<Styles.UnitStyle> ColumnsWidth { get; set; }

		/// <summary>
		///		Ancho de la tabla
		/// </summary>
		public Styles.UnitStyle Width { get; set; }

		/// <summary>
		///		Cuerpo de la tabla (celdas)
		/// </summary>
		public CellsReportCollection Body { get; set; }
	}
}
