using System;

using Bau.Libraries.LibCommonHelper.Extensors;
using Bau.Libraries.LibReports.Renderer.Models.Base;
using Bau.Libraries.LibReports.Renderer.Models.Contents;
using Bau.Libraries.LibMarkupLanguage;

namespace Bau.Libraries.LibReports.Renderer.Parser.Tools
{
	/// <summary>
	///		Clase base para los intérpretes de tablas
	/// </summary>
	internal abstract class TableBaseParser<TypeData> : ParserBase<TypeData> where TypeData : ContentReportBase
	{
		internal TableBaseParser(ReportParser reportParser) : base(reportParser) { }

		/// <summary>
		///		Obtiene los anchos de las columnas
		/// </summary>
		protected System.Collections.Generic.List<Models.Styles.UnitStyle> GetColumnsWidth(string columnsWidth)
		{
			System.Collections.Generic.List<Models.Styles.UnitStyle> objColWidths = new System.Collections.Generic.List<Models.Styles.UnitStyle>();

				// Interpreta la colección de anchos
				if (!columnsWidth.IsEmpty())
				{
					string[] partsColumnsWidth = columnsWidth.Split(';');

						// Añade los anchos de columnas
						foreach (string strWidth in partsColumnsWidth)
							if (!strWidth.IsEmpty())
								objColWidths.Add(ParserHelper.GetUnit(strWidth));
				}
				// Devuelve la colección de anchos
				return objColWidths;
		}

		/// <summary>
		///		Interpreta los nodos de código
		/// </summary>
		protected CodeReport ParseCode(ContentReportBase parent, MLNode nodeML)
		{
			CodeReport code = null;

				// Carga los nodos de código
				foreach (MLNode childML in nodeML.Nodes)
					if (childML.Name == "Code")
					{
						CodeReport codeInner = ReportParser.ParserLocator.Parse<CodeReport>(parent, childML);

							if (!(codeInner?.Code).IsEmpty())
							{
								if (code == null)
									code = codeInner;
								else
									code.Code += Environment.NewLine + codeInner.Code;
							}
					}
				// Devuelve el código
				return code;
		}

		/// <summary>
		///		Obtiene una serie de celdas de una tabla
		/// </summary>
		protected CellsReportCollection GetCells(ContentReportBase parent, MLNode nodeML)
		{
			CellsReportCollection cells = new CellsReportCollection();

				// Rellena la colección de celdas
				foreach (MLNode cellML in nodeML.Nodes)
					if (cellML.Name == "Cell")
						cells.Add(GetCell(parent, cellML));
				// Devuelve la colección de celdas
				return cells;
		}

		/// <summary>
		///		Obtiene los datos de una celda
		/// </summary>
		private CellReport GetCell(ContentReportBase parent, MLNode cellML)
		{
			CellReport cell = new CellReport(parent);

				// Interpreta los datos comunes
				base.ParseContentCommon(cell, cellML);
				// Asigna los datos de la celda
				cell.Row = cellML.Attributes["Row"].Value.GetInt(0);
				cell.Column = cellML.Attributes["Column"].Value.GetInt(0);
				cell.RowSpan = cellML.Attributes["RowSpan"].Value.GetInt(1);
				cell.ColumnSpan = cellML.Attributes["ColumnSpan"].Value.GetInt(1);
				cell.Width = ParserHelper.GetUnit(cellML.Attributes["Width"].Value);
				// Asigna el contenido de la celda
				foreach (MLNode childML in cellML.Nodes)
					cell.Content = ReportParser.ParserLocator.Parse<ContentReportBase>(cell, childML);
				// Si no se ha asignado ningún contenido es porque el valor de la celda directamente se debe considerar un párrafo
				if (cell.Content == null)
					cell.Content = new ParagraphParser(ReportParser).Parse(cell, cellML);
				// Si el contenido de la celda es un párrafo y no tiene alineación, se asigna una alineación heredada
				if (cell.Content is ParagraphReport)
				{
					if (cell.StyleClass.HorizontalAlign == Models.Styles.StyleReport.HorizontalAlignType.Unkown)
						cell.StyleClass.HorizontalAlign = Models.Styles.StyleReport.HorizontalAlignType.Justified;
					if (cell.StyleClass.VerticalAlign == Models.Styles.StyleReport.VerticalAlignType.Unknown)
						cell.StyleClass.VerticalAlign = Models.Styles.StyleReport.VerticalAlignType.Top;
				}
				// Devuelve la celda
				return cell;
		}
	}
}
