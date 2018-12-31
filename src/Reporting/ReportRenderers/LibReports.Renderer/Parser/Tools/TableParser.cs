using System;

using Bau.Libraries.LibCommonHelper.Extensors;
using Bau.Libraries.LibReports.Renderer.Models.Contents;
using Bau.Libraries.LibMarkupLanguage;

namespace Bau.Libraries.LibReports.Renderer.Parser.Tools
{
	/// <summary>
	///		Intérprete de tablas
	/// </summary>
	internal class TableParser : TableBaseParser<TableReport>
	{
		internal TableParser(ReportParser reportParser) : base(reportParser) { }

		/// <summary>
		///		Interpreta una tabla
		/// </summary>
		protected override TableReport ParseInner(MLNode nodeML)
		{
			TableReport table = new TableReport(Parent);

				// Asigna los datos de padding
				table.CellPadding = new StyleParser(ReportParser).ParseMargin("CellPadding", nodeML);
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
						case "Header":
								table.Header = GetCells(table, childML);
								table.HeaderCode = ParseCode(table, childML);
							break;
						case "Body":
								table.Body = GetCells(table, childML);
								table.BodyCode = ParseCode(table, childML);
							break;
						case "Foot":
								table.Foot = GetCells(table, childML);
								table.FootCode = ParseCode(table, childML);
							break;
						default:
							throw new NotImplementedException("Nodo desconocido - Nodo: " + childML.Name);
					}
				// Devuelve la tabla
				return table;
		}
	}
}
