using System;

using Bau.Libraries.LibCommonHelper.Extensors;
using Bau.Libraries.LibReports.Renderer.Models.Contents;
using Bau.Libraries.LibMarkupLanguage;

namespace Bau.Libraries.LibReports.Renderer.Parser.Tools
{
	/// <summary>
	///		Intérprete de tablas horizontales
	/// </summary>
	internal class TableHorizontalParser : TableBaseParser<HorizontalTableReport>
	{
		internal TableHorizontalParser(ReportParser reportParser) : base(reportParser) { }

		/// <summary>
		///		Interpreta los datos de una tabla horizontal
		/// </summary>
		protected override HorizontalTableReport ParseInner(MLNode nodeML)
		{
			HorizontalTableReport table = new HorizontalTableReport(Parent);

				// Asigna los datos
				table.CellPadding = new StyleParser(ReportParser).ParseMargin("CellPadding", nodeML);
				table.TablesPerRow = nodeML.Attributes["TablesPerRow"].Value.GetInt(3);
				// Asigna los anchos de columnas y de la tabla
				table.ColumnsWidth = GetColumnsWidth(nodeML.Attributes["ColumnsWidth"].Value);
				table.Width = ParserHelper.GetUnit(nodeML.Attributes["Width"].Value);
				// Interpreta las partes de la tabla
				foreach (MLNode childML in nodeML.Nodes)
					switch (childML.Name)
					{
						//case "DataTableProvider":
						//		table.Command = ReportParser.ParserLocator.ParseDataComand(table, childML);
						//	break;
						case "Body":
								table.Body = GetCells(table, childML);
							break;
					}
				// Devuelve la tabla interpretada
				return table;
		}
	}
}
