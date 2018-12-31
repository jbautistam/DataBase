using System;
using System.Collections.Generic;

using Bau.Libraries.LibReports.Renderer.Models.Base;

namespace Bau.Libraries.LibReports.Renderer.Models.Contents
{
	/// <summary>
	///		Tabla de datos de un <see cref="Report"/>
	/// </summary>
	public class TableReport : ContentCommandReportBase
	{
		public TableReport(ContentReportBase parent) : base(parent)
		{
			Header = new CellsReportCollection();
			Body = new CellsReportCollection();
			Foot = new CellsReportCollection();
			CellPadding = new Styles.MarginStyleReport();
			Width = new Styles.UnitStyle(0, Styles.UnitStyle.UnitType.Unknown);
			ColumnsWidth = new List<Styles.UnitStyle>();
		}

		/// <summary>
		///		Clona la definición
		/// </summary>
		public TableReport CloneDefinition(ContentReportBase parent)
		{
			TableReport table = new TableReport(parent);

				// Clona los datos básicos
				base.CloneToTarget(table);
				// Clona el resto de parámetros
				table.Width = Width.Clone();
				table.CellPadding = CellPadding.Clone();
				// Clona los anchos
				foreach (Styles.UnitStyle objUnit in ColumnsWidth)
					table.ColumnsWidth.Add(objUnit.Clone());
				// Clona los códigos
				if (HeaderCode != null)
					table.HeaderCode = HeaderCode.Clone();
				if (BodyCode != null)
					table.BodyCode = BodyCode.Clone();
				if (FootCode != null)
					table.FootCode = FootCode.Clone();
				// Devuelve la tabla clonada
				return table;
		}

		/// <summary>
		///		Ordena los datos de la tabla
		/// </summary>
		public void Sort()
		{
			Header.SortByPosition();
			Body.SortByPosition();
			Foot.SortByPosition();
		}

		/// <summary>
		///		Cabecera de la tabla
		/// </summary>
		public CellsReportCollection Header { get; set; }

		/// <summary>
		///		Código en la cabecera
		/// </summary>
		public CodeReport HeaderCode { get; set; }

		/// <summary>
		///		Celdas de la tabla
		/// </summary>
		public CellsReportCollection Body { get; set; }

		/// <summary>
		///		Código en la cabecera
		/// </summary>
		public CodeReport BodyCode { get; set; }

		/// <summary>
		///		Pie de la tabla
		/// </summary>
		public CellsReportCollection Foot { get; set; }

		/// <summary>
		///		Código en la cabecera
		/// </summary>
		public CodeReport FootCode { get; set; }

		/// <summary>
		///		Padding de las celdas
		/// </summary>
		public Styles.MarginStyleReport CellPadding { get; set; }

		/// <summary>
		///		Ancho de la tabla
		/// </summary>
		public Styles.UnitStyle Width { get; set; }

		/// <summary>
		///		Ancho de las columnas
		/// </summary>
		public List<Styles.UnitStyle> ColumnsWidth { get; set; }
	}
}