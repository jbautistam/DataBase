using System;

using Bau.Libraries.LibCommonHelper.Extensors;
using Bau.Libraries.LibReports.Renderer.Models;
using Bau.Libraries.LibReports.Renderer.Models.Base;
using Bau.Libraries.LibReports.Renderer.Models.Contents;
using Bau.Libraries.LibReports.Renderer.Models.Styles;

namespace Bau.Libraries.LibReports.Renderer.Html
{
	/// <summary>
	///		Representación de un informe en HTML
	/// </summary>
	public class RendererHTML : BaseRenderer.RendererTextBase
	{   
		// Variables privadas
		private Charts.RendererChartGoogle _chartRenderer = new Charts.RendererChartGoogle();

		/// <summary>
		///		Representa un informe como HTML
		/// </summary>
		protected override void Process()
		{ 
			// Añade los datos del informe
			Append("<html>");
			AppendRight("<head>");
			AddStyles();
			AppendLeft("</head>");
			AppendRight("<body>");
			AddBody();
			AppendLeft("</body>");
			AppendLeft("</html>");
			// Graba el texto
			Save();
		}

		/// <summary>
		///		Añade los estilos
		/// </summary>
		private void AddStyles()
		{ 
			// Cabecera
			AppendRight("<style>");
			// Estilos
			foreach (StyleReport style in Report.Styles)
			{ 
				// Cabecera
				AppendRight("." + style.ID + " {");
				IndentRight();
				// Fuente
				Append(GetStyle(false, style));
				// Cierra
				AppendLeft("}");
			}
			// Cierre
			AppendLeft("</style>");
		}

		/// <summary>
		///		Añade el cuerpo con los datos del mensaje
		/// </summary>
		private void AddBody()
		{
			AddContent(Report.Header);
			AddContent(Report.Body);
			AddContent(Report.Foot);
		}

		/// <summary>
		///		Añade al HTML el contenido
		/// </summary>
		private void AddContent(ContentReportBase content)
		{
			if (content is ParagraphReport)
				AddParagraph(content as ParagraphReport);
			else if (content is ImageReport)
				AddImage(content as ImageReport);
			else if (content is TableReport)
				AddTable(content as TableReport);
			else if (content is SectionReport)
				AddSection(content as SectionReport);
			else if (content is ListReport)
				AddList(content as ListReport);
			else if (content is HorizontalTableReport)
				AddHorizontalTable(content as HorizontalTableReport);
			else if (content is WaterMarkReport)
				AddWatermark(content as WaterMarkReport);
			else if (content is ChartReport)
				AddChart(content as ChartReport);
			else if (content is HeaderFootReport)
				System.Diagnostics.Debug.WriteLine("No hace nada");
			else if (content is LinkReport)
				AddLink(content as LinkReport);
			else
				throw new NotImplementedException("No se ha definido este tipo de sección");
		}

		/// <summary>
		///		Añade una nueva página
		/// </summary>
		private void AddNewPageBefore(ContentReportBase content)
		{
			if (content.NewPageBefore)
				Append("<hr>", true);
		}

		/// <summary>
		///		Añade una nueva página posterior
		/// </summary>
		private void AddNewPageAfter(ContentReportBase content)
		{
			if (content.NewPageAfter)
				Append("<hr>", true);
		}

		/// <summary>
		///		Añade una sección al HTML
		/// </summary>
		private void AddSection(SectionReport section)
		{ 
			// Salto de página anterior
			AddNewPageBefore(section);
			// Cabecera
			base.AppendRight("<div>");
			// Contenido
			foreach (ContentReportBase content in section.Contents)
				AddContent(content);
			// Cierre
			base.AppendLeft("</div>");
			// Salto de página posterior
			AddNewPageAfter(section);
		}

		/// <summary>
		///		Añade una marca de agua
		/// </summary>
		private void AddWatermark(WaterMarkReport waterMark)
		{
			if (waterMark.Mode == WaterMarkReport.ModeWaterMark.Start)
				foreach (ContentReportBase image in waterMark.Images)
					if (image != null && image is ImageReport)
						AddImage(image as ImageReport);
		}

		/// <summary>
		///		Añade un párrafo
		/// </summary>
		private void AddParagraph(ParagraphReport paragraph, bool addHeader = true)
		{ 
			// Salto de página
			AddNewPageBefore(paragraph);
			// Cabecera del párrafo
			if (addHeader)
				AppendRight(string.Format("<p {0}>", GetStyles(paragraph, null)));
			// Texto
			if (paragraph.IsFullParagraph)
				Append(paragraph.GetText());
			else
				foreach (ContentReportBase content in paragraph.Contents)
					if (content is SpanReport)
					{
						SpanReport span = content as SpanReport;

							Append(string.Format("<span {0}>{1}</span>", GetStyles(span, null), span.Text));
					}
					else if (content is LinkReport)
						AddLink(content as LinkReport);
			// Fin del párrafo
			if (addHeader)
			{
				Append("</p>");
				IndentLeft();
			}
			// Salto de página posterior
			AddNewPageAfter(paragraph);
		}

		/// <summary>
		///		Añade una imagen
		/// </summary>
		private void AddImage(ImageReport image)
		{ 
			// Salto de página
			AddNewPageBefore(image);
			// Imagen
			AppendRight($"<img src='{image.FileName}' {GetStyles(image, image.Width)}>");
			IndentLeft();
			// Salto de página posterior
			AddNewPageAfter(image);
		}

		/// <summary>
		///		Añade los datos de una tabla
		/// </summary>
		private void AddTable(TableReport table)
		{
			if (table.Header.Count > 0 || table.Body.Count > 0 || table.Foot.Count > 0)
			{ 
				// Salto de página
				AddNewPageBefore(table);
				// Ordena las celdas de la tabla
				table.Sort();
				// Cabecera
				AppendRight(string.Format("<table width='100%' border='0' {0}>", GetStyles(table, table.Width)));
				// Filas
				AddRows(table.Header, true);
				AddRows(table.Body, false);
				AddRows(table.Foot, false);
				// Fin
				AppendLeft("</table>");
				// Salto de página posterior
				AddNewPageAfter(table);
			}
		}

		/// <summary>
		///		Añade los datos de una tabla horizontal
		/// </summary>
		private void AddHorizontalTable(HorizontalTableReport table)
		{
			if (table.Body.Count > 0)
			{ 
				// Salto de página
				AddNewPageBefore(table);
				// Ordena las celdas de la tabla
				table.Sort();
				// Cabecera
				AppendRight(string.Format("<table width='100%' border = '0' {0}>", GetStyles(table, table.Width)));
				// Celdas
				AddRows(table.Body, false);
				// Fin
				AppendLeft("</table>");
				// Salto de página posterior
				AddNewPageAfter(table);
			}
		}

		/// <summary>
		///		Añade una cabecera a la tabla
		/// </summary>
		private void AddRows(CellsReportCollection cells, bool isHeader)
		{
			int lastRow = -1;

				// Crea la tabla con las filas
				foreach (CellReport cell in cells)
				{ 
					// Cabecera de fila
					if (lastRow != cell.Row)
					{ 
						// Fin de fila
						if (lastRow != -1)
							AppendLeft("</tr>");
						// Asigna la fila
						lastRow = cell.Row;
						// Abre una nueva fila
						AppendRight("<tr>");
					}
					// Cabecera de celda
					AppendRight(GetCellHeader(cell, isHeader));
					// Contenido de la celda
					if (cell.Content is ParagraphReport)
					{
						if ((cell.Content as ParagraphReport).Contents.Count == 0 ||
								  (cell.Content as ParagraphReport).GetText().IsEmpty())
							Append("&nbsp;");
						else
							AddParagraph(cell.Content as ParagraphReport, false);
					}
					else if (cell.Content is ImageReport)
						AddImage(cell.Content as ImageReport);
					else if (cell.Content is TableReport)
						AddTable(cell.Content as TableReport);
					else if (cell.Content is SectionReport)
						AddSection(cell.Content as SectionReport);
					else if (cell.Content is ChartReport)
						AddChart(cell.Content as ChartReport);
					else
						throw new NotImplementedException("Tipo de contenido desconocido");
					// Cierre de la celda
					if (isHeader)
						AppendLeft("</th>");
					else
						AppendLeft("</td>");
				}
				// Cierre de la última fila abierta
				if (lastRow != -1)
					AppendLeft("</tr>");
		}

		/// <summary>
		///		Obtiene la cabecera de una celda
		/// </summary>
		private string GetCellHeader(CellReport cell, bool isHeader)
		{
			string text = "<";
			UnitStyle width = new UnitStyle(0, UnitStyle.UnitType.Unknown);

				// Etiqueta
				if (isHeader)
				{ 
					// Etiqueta
					text += "th ";
					// Ancho y demás
					if (cell.Width.IsDefined)
					{
						width.Value = cell.Width.Value;
						width.Unit = cell.Width.Unit;
					}
				}
				else
					text += "td ";
				if (cell.RowSpan > 1)
					text += " rowspan = '" + cell.RowSpan.ToString() + "'";
				if (cell.ColumnSpan > 1)
					text += " colspan = '" + cell.ColumnSpan.ToString() + "'";
				text += GetStyles(cell, width);
				// Devuelve el texto
				return text + ">";
		}

		/// <summary>
		///		Añade los elementos de la lista
		/// </summary>
		private void AddList(ListReport list)
		{
			string type = "ul";

				// Salto de página
				AddNewPageBefore(list);
				// Cambia la cabecera si es una lista numerada
				if (list.Type != ListReport.ListType.Unordered)
					type = "ol";
				// Cabecera
				AppendRight($"<{type}>");
				// Elementos de la lista
				foreach (ListItemReport listItem in list.ListItems)
				{ 
					// Cabecera
					AppendRight("<li>");
					// Texto
					if (!listItem.Text.IsEmpty())
						Append(listItem.Text);
					// Lista interna
					if (listItem.List.IsDefined)
						AddList(listItem.List);
					// Fin
					AppendLeft("</li>");
				}
				// Fin
				AppendLeft($"</{type}>");
				// Salto de página posterior
				AddNewPageAfter(list);
		}

		/// <summary>
		///		Añade un gráfico al informe
		/// </summary>
		private void AddChart(ChartReport chart)
		{
			Append(_chartRenderer.Render(chart));
		}

		/// <summary>
		///		Añade un vínculo
		/// </summary>
		private void AddLink(LinkReport link)
		{
			if (link.Type == LinkReport.LinkType.Bookmark)
				Append($"<a name='{link.Target}'></a>");
			else
				Append($"<a href='{link.Target.TrimIgnoreNull().Replace("'", "\"")}'>{link.Text}</a>");
		}

		/// <summary>
		///		Obtiene el estilo
		/// </summary>
		private string GetStyles(ContentReportBase content, UnitStyle width)
		{
			string style = "";

				// Añade la clase
				if (!content.ClassId.IsEmpty())
					style += "class='" + content.ClassId + "'";
				if (content.StyleClass != null && content.StyleClass.IsDefined)
					style += GetStyle(true, content.StyleClass, width);
				// Devuelve el estilo
				return style;
		}

		/// <summary>
		///		Obtiene el HTML de un estilo (atributo style)
		/// </summary>
		private string GetStyle(bool addHeader, StyleReport style, UnitStyle width = null)
		{
			string result = "";

				// Obtiene el estilo
				if (style != null && style.IsDefined)
				{ 
					// Fuente
					result += GetStyleFont(style.Font);
					// Alineación horizontal / vertical
					result += GetStyleHorizontalAlign(style.HorizontalAlign);
					result += GetStyleVerticalAlign(style.VerticalAlign);
					// Asigna el ancho
					if (width != null && width.IsDefined)
					{
						result += "width:" + width.Value;
						switch (width.Unit)
						{
							case UnitStyle.UnitType.Percent:
									result += "%";
								break;
							case UnitStyle.UnitType.Pixels:
									result += "px";
								break;
						}
						result += ";";
					}
					// Bordes
					result += GetStyleBorder("left", style.LeftBorder);
					result += GetStyleBorder("right", style.RightBorder);
					result += GetStyleBorder("top", style.TopBorder);
					result += GetStyleBorder("bottom", style.BottomBorder);
					// Márgenes
					result += GetStyleMargin("left", style.Margin.Left);
					result += GetStyleMargin("right", style.Margin.Right);
					result += GetStyleMargin("top", style.Margin.Top);
					result += GetStyleMargin("bottom", style.Margin.Bottom);
					// Colores
					if (style.Font.Color != null)
						result += "color:" + GetHTMLColor(style.Font.Color ?? System.Drawing.Color.Black) + ";";
					if (style.BackGround != null)
						result += "background-color:" + GetHTMLColor(style.BackGround ?? System.Drawing.Color.White) + ";";
					// Fin del estilo
					if (addHeader && !result.IsEmpty())
						result = " style='" + result + "'";
				}
				// Devuelve la cadena de estilo
				return result;
		}

		/// <summary>
		///		Obtiene el estilo de la alineación horizontal
		/// </summary>
		private string GetStyleHorizontalAlign(StyleReport.HorizontalAlignType align)
		{
			switch (align)
			{
				case StyleReport.HorizontalAlignType.Right:
					return "text-align: right;";
				case StyleReport.HorizontalAlignType.Center:
					return "text-align: center;";
				default:
					return "";
			}
		}

		/// <summary>
		///		Obtiene el estilo de la alineación vertical
		/// </summary>
		private string GetStyleVerticalAlign(StyleReport.VerticalAlignType align)
		{
			switch (align)
			{
				case StyleReport.VerticalAlignType.Bottom:
					return "vertical-align: text-bottom;";
				case StyleReport.VerticalAlignType.Middle:
					return "vertical-align: middle;";
				case StyleReport.VerticalAlignType.Top:
					return "vertical-align: text-top;";
				default:
					return "";
			}
		}

		/// <summary>
		///		Obtiene el estilo asociado a una fuente
		/// </summary>
		private string GetStyleFont(FontStyleReport font)
		{
			string style = "";

				// Asigna el estilo de la fuente
				if (font != null)
				{
					if (font.Bold)
						style += "font-weight: bold;";
					if (font.Underline)
						style += "text-decoration: underline;";
					if (font.Italic)
						style += "font-style: italic;";
					if (font.Size > 0)
						style += "font-size: " + font.Size + "pt;";
				}
				// Devuelve el estilo
				return style;
		}

		/// <summary>
		///		Obtiene el estilo del borde
		/// </summary>
		private string GetStyleBorder(string position, BorderStyleReport border)
		{
			string style = "";

				// Asigna los stilos del borde
				if (border.Visible)
				{ 
					// Cabecera
					style = "border-" + position + ":";
					// Ancho
					style += border.Width + "px ";
					// Estilo
					switch (border.Type)
					{
						case BorderStyleReport.BorderType.Dash:
								style += " dashed";
							break;
						case BorderStyleReport.BorderType.Dot:
								style += " dotted";
							break;
						case BorderStyleReport.BorderType.DotDash:
								style += " dashed";
							break;
						case BorderStyleReport.BorderType.Double:
								style += " double";
							break;
						default:
								style += " solid";
							break;
					}
					// Color
					style += " " + GetHTMLColor(border.Color ?? System.Drawing.Color.Black);
					// Fin
					style += ";";
				}
				// Devuelve el estilo
				return style;
		}

		/// <summary>
		///		Obtiene el estilo del margen
		/// </summary>
		private string GetStyleMargin(string margin, double? width)
		{
			if (width != null && width != 0)
				return $"margin-{margin}: {(width ?? 0)};";
			else
				return "";
		}

		/// <summary>
		///		Obtiene una cadena HTML de un color
		/// </summary>
		private string GetHTMLColor(System.Drawing.Color color)
		{
			return string.Format("#{0:x2}{1:x2}{2:x2}", color.R, color.G, color.B);
		}
	}
}
