using System;

using iTextSharp.text;
using iTextSharp.text.pdf;

using Bau.Libraries.LibCommonHelper.Extensors;
using Bau.Libraries.LibReports.Renderer.Models.Base;
using Bau.Libraries.LibReports.Renderer.Models.Contents;
using Bau.Libraries.LibReports.Renderer.Models.Styles;
using Bau.Libraries.LibReports.Renderer.PDF.Tools;
using Bau.Libraries.LibReports.Renderer.Models;

namespace Bau.Libraries.LibReports.Renderer.PDF
{
	/// <summary>
	///		Clase para exportación a PDF
	/// </summary>
	public class RendererPdf : IReportRenderer
	{ 
		// Estructuras privadas
		/// <summary>
		///		Elemento del PDF
		/// </summary>
		private struct sctPdfElement
		{
			public sctPdfElement(bool isNewPage) : this(null, null, isNewPage) { }

			public sctPdfElement(IElement pdfElement, ContentReportBase content, bool isNewPage = false)
			{
				PdfElement = pdfElement;
				Content = content;
				IsNewPage = isNewPage;
			}

			public IElement PdfElement;
			public ContentReportBase Content;
			public bool IsNewPage;
		}
		// Variables privadas
		private double _pageWidth = 0; // ... ancho de la página del PDF en milímetros

		public RendererPdf()
		{
			//FontsEmbedded = new FontsEmbeddedPdfCollection(null);
			ActualHeaderSelector = new HeaderFootSelector(null, HeaderFootReport.HeaderFootType.Header);
			ActualFooterSelector = new HeaderFootSelector(null, HeaderFootReport.HeaderFootType.Foot);
		}

		/// <summary>
		///		Genera el archivo del informe
		/// </summary>
		public void Render(string fileTarget, string fileXml)
		{
			Document pdfDocument;
			PdfWriter pdfWriter;

				// Asigna las propiedades
				FileName = fileTarget;
				Report = Parse(fileXml);
				//// Asigna las fuentas embebidas
				//FontsEmbedded = new FontsEmbeddedPdfCollection(report.Filter.AdditionalFiles.Select(Models.Base.Parameters.AdditionalFile.FileType.Font));
				// Inicializa las propiedades del documento
				pdfDocument = new Document(GetPdfPageType(Report.PageType, Report.Landscape));
				// Asigna los márgenes
				pdfDocument.SetMargins((float) (Report.Margin.Left ?? pdfDocument.LeftMargin),
									   (float) (Report.Margin.Right ?? pdfDocument.RightMargin),
									   (float) (Report.Margin.Top ?? pdfDocument.TopMargin),
									   (float) (Report.Margin.Bottom ?? pdfDocument.BottomMargin));
				// Obtiene la instancia de PDF Writer
				pdfWriter = PdfWriter.GetInstance(pdfDocument, new System.IO.FileStream(FileName, System.IO.FileMode.Create));
				// Asigna el manejador de eventos de la página
				pdfWriter.PageEvent = new RendererPdfPageHandler(this);
				// Abre el documento
				pdfDocument.Open();
				// Añade la información del documento
				pdfDocument.AddAuthor(Report.Parameters.Author);
				pdfDocument.AddKeywords(Report.Parameters.Keywords);
				pdfDocument.AddSubject(Report.Parameters.Subject);
				pdfDocument.AddTitle(Report.Parameters.Title);
				pdfDocument.AddCreationDate();
				pdfDocument.AddHeader("Comments", Report.Parameters.Comments);
				// Guarda el ancho de la página
				_pageWidth = pdfDocument.PageSize.Width - pdfDocument.LeftMargin - pdfDocument.RightMargin;
				// Añade los diferentes datos del documento
				ConvertDocument(pdfDocument);
				// Cierra el documento (y lo graba)
				pdfDocument.Close();
		}

		/// <summary>
		///		Interpreta un archivo XML
		/// </summary>
		private Report Parse(string fileName)
		{
			return new Parser.ReportParser(System.IO.Path.GetDirectoryName(fileName)).ParseFile(fileName);
		}

		/// <summary>
		///		Obtiene el tipo de página del PDF para el tipo de página del informe
		/// </summary>
		private Rectangle GetPdfPageType(Report.ReportPageType pageIndexType, bool landScape)
		{
			#pragma warning disable 0612
			switch (pageIndexType)
			{
				case Report.ReportPageType.Letter:
					if (landScape)
						return PageSize.LETTER_LANDSCAPE;
					else
						return PageSize.LETTER;
				default:
					if (landScape)
						return PageSize.A4_LANDSCAPE;
					else
						return PageSize.A4;
			}
			#pragma warning restore 0612
		}

		/// <summary>
		///		Convierte el informe a un PDF
		/// </summary>
		private void ConvertDocument(Document pdfDocument)
		{ 
			//! No añade cabecera y pie puesto que se procesan en el RendererPdfPageHandler al finalizar de procesar la página
			AddSection(pdfDocument, Report.Body, false);
		}

		/// <summary>
		///		Añade una sección al PDF
		/// </summary>
		private void AddSection(Document pdfDocument, SectionReport reportSection, bool atCell)
		{
			foreach (sctPdfElement sectionItem in GetSectionElements(reportSection, atCell))
				if (sectionItem.IsNewPage) // ... añade el salto de página
					pdfDocument.NewPage();
				else if (sectionItem.Content != null && sectionItem.Content is WaterMarkReport) // ... asigna la marca de agua
					AssignWaterMark(sectionItem.Content as WaterMarkReport);
				else if (sectionItem.Content != null && sectionItem.Content is HeaderFootSelector) // ... asigna el selector de cabecera / pie
					AssignHeaderFootSelector(sectionItem.Content as HeaderFootSelector);
				else // Añade el elemento al PDF
					pdfDocument.Add(sectionItem.PdfElement);
		}

		/// <summary>
		///		Asigna la marca de agua
		/// </summary>
		private void AssignWaterMark(WaterMarkReport waterMark)
		{
			if (waterMark == null || waterMark.Mode == WaterMarkReport.ModeWaterMark.End)
				ActualWaterMark = null;
			else
				ActualWaterMark = waterMark;
		}

		/// <summary>
		///		Asigna el selector de cabecera o pie
		/// </summary>
		private void AssignHeaderFootSelector(HeaderFootSelector selector)
		{
			if (selector != null)
			{
				if (selector.Type == HeaderFootReport.HeaderFootType.Header)
					ActualHeaderSelector = selector;
				else
					ActualFooterSelector = selector;
			}
		}

		/// <summary>
		///		Obtiene los datos de una sección
		/// </summary>
		private System.Collections.Generic.List<sctPdfElement> GetSectionElements(SectionReport reportSection, bool atCell)
		{
			System.Collections.Generic.List<sctPdfElement> pdfItems = new System.Collections.Generic.List<sctPdfElement>();

				// Añade las secciones del informe
				foreach (ContentReportBase content in reportSection.Contents)
				{ 
					// Añade la página anterior
					if (content.NewPageBefore)
						pdfItems.Add(new sctPdfElement(true));
					// Añade los datos de la sección
					if (content is SectionReport)
						pdfItems.AddRange(GetSectionElements(content as SectionReport, atCell));
					else if (content is ParagraphReport)
						pdfItems.Add(new sctPdfElement(GetParagraphOrTable(content as ParagraphReport, atCell), content));
					else if (content is TableReport)
						pdfItems.Add(new sctPdfElement(GetTable(content as TableReport), content));
					else if (content is ListReport)
						pdfItems.Add(new sctPdfElement(GetList(content as ListReport, 0), content));
					else if (content is ImageReport)
						pdfItems.Add(new sctPdfElement(GetImage(content as ImageReport), content));
					else if (content is HorizontalTableReport)
						pdfItems.Add(new sctPdfElement(GetHorizontalTable(content as HorizontalTableReport), content));
					else if (content is WaterMarkReport)
						pdfItems.Add(new sctPdfElement(null, content as WaterMarkReport));
					else if (content is HeaderFootSelector)
						pdfItems.Add(new sctPdfElement(null, content as HeaderFootSelector));
					else if (content is LinkReport)
						pdfItems.Add(new sctPdfElement(GetLink(content as LinkReport), content));
					else if (content is ChartReport)
						System.Diagnostics.Debug.WriteLine("Generar el informe del gráfico");
					else
						throw new NotImplementedException("No se reconoce el contenido del informe");
					// Añade la página posterior
					if (content.NewPageAfter)
						pdfItems.Add(new sctPdfElement(true));
				}
				// Devuelve la colección de elementos
				return pdfItems;
		}

		/// <summary>
		///		Obtiene únicamente los elementos PDF de los datos de una sección (la utiliza la clase de tratamiento de eventos
		///	de la página)
		/// </summary>
		internal System.Collections.Generic.List<IElement> GetPdfSectionElements(SectionReport reportSection, bool atCell)
		{
			System.Collections.Generic.List<sctPdfElement> sectionItems = GetSectionElements(reportSection, atCell);
			System.Collections.Generic.List<IElement> pdfItems = new System.Collections.Generic.List<IElement>();

				// Añade las secciones del informe
				foreach (sctPdfElement sectionItem in sectionItems)
					pdfItems.Add(sectionItem.PdfElement);
				// Devuelve la colección de elementos
				return pdfItems;
		}

		/// <summary>
		///		Obtiene un párrafo o tabla (no se puede asignar un fondo a un párrafo, por eso al crear un párrafo
		///	que tiene fondo, se crea una tabla con una única celda)
		/// </summary>
		private IElement GetParagraphOrTable(ParagraphReport reportParagraph, bool atCell)
		{
			StyleReport style = reportParagraph.CompactStyle(Report);

				if (!atCell && style.BackGround != null)
					return GetTableFromParagraph(reportParagraph, style);
				else
					return GetParagraph(reportParagraph, style, atCell);
		}

		/// <summary>
		///		Obtiene un párrafo para el PDF
		/// </summary>
		private Paragraph GetParagraph(ParagraphReport reportParagraph, bool atCell)
		{
			return GetParagraph(reportParagraph, reportParagraph.CompactStyle(Report), atCell);
		}

		/// <summary>
		///		Obtiene un párrafo para el PDF
		/// </summary>
		private Paragraph GetParagraph(ParagraphReport reportParagraph, StyleReport style, bool atCell)
		{
			Paragraph pfParagraph = new Paragraph();
			int spanIndex = 0;

				// Asigna el estilo al párrafo
				AssignStyle(pfParagraph, style);
				// Asigna el texto
				foreach (ContentReportBase content in reportParagraph.Contents)
				{
					Phrase phrase = new Phrase();

						// Añade el vínculo o el span							
						if (content is LinkReport)
						{
							LinkReport link = content as LinkReport;

								// Añade un espacio si es necesario
								if (spanIndex > 0 && !link.Text.IsEmpty())
									phrase.Add(" ");
								// Añade el vínculo a la frase
								phrase.Add(GetLink(link));
						}
						else
						{
							SpanReport reportSpan = content as SpanReport;
							Chunk chunk = new Chunk(NormalizeText(reportSpan.Text));

								// Asigna el estilo
								AssignStyle(chunk, content.CompactStyle(Report), atCell);
								// Añade un espacio y la frase
								if (spanIndex > 0 && !reportSpan.Text.IsEmpty())
									phrase.Add(" ");
								phrase.Add(chunk);
						}
						// Añade la frase al párrafo
						pfParagraph.Add(phrase);
						// Incrementa el índice de span dentro del párrafo
						spanIndex++;
				}
				// Devuelve el párrafo
				return pfParagraph;
		}

		/// <summary>
		///		Obtiene una tabla a partir de un párrafo
		/// </summary>
		private PdfPTable GetTableFromParagraph(ParagraphReport reportParagraph, StyleReport style)
		{
			TableReport reportTable = new TableReport(null);
			CellReport reportCell = new CellReport(reportTable);

				// Asigna los datos del párrafo a la celda
				reportCell.StyleClass = style.Clone();
				reportCell.Column = 0;
				reportCell.Row = 0;
				reportCell.Content = reportParagraph;
				reportCell.StyleClass.Margin.Top = null;
				reportCell.StyleClass.Margin.Bottom = null;
				// Crea la tabla
				reportTable.Width = new UnitStyle(100, UnitStyle.UnitType.Percent);
				reportTable.ColumnsWidth = new System.Collections.Generic.List<UnitStyle> { new UnitStyle(100, UnitStyle.UnitType.Percent) };
				reportTable.StyleClass = style.Clone();
				// Añade la celda con el párrafo
				reportTable.Body.Add(reportCell);
				// Devuelve la tabla
				return GetTable(reportTable);
		}

		/// <summary>
		///		Obtiene una tabla de PDF
		/// </summary>
		private PdfPTable GetTable(TableReport reportTable)
		{
			PdfPTable pdfTable = new PdfPTable(reportTable.ColumnsWidth.Count);

				// Ordena las celdas de la tabla
				reportTable.Sort();
				// Añade los anchos de columna a la tabla
				AddTableColumns(pdfTable, reportTable.ColumnsWidth);
				// Añade las filas de cabecera a la tabla
				AddRows(pdfTable, reportTable.Header, true);
				if (reportTable.Header.Count != 0 && reportTable.Body.Count != 0)
					pdfTable.HeaderRows = reportTable.Header.GetMaximumRow();
				// Añade las filas del cuerpo a la tabla
				AddRows(pdfTable, reportTable.Body, true);
				// Añade las filas del pie a la tabla
				AddRows(pdfTable, reportTable.Foot, false);
				// Asigna el estilo a la tabla	
				AssignStyle(pdfTable, reportTable.CompactStyle(Report));
				// Devuelve la tabla generada
				return pdfTable;
		}

		/// <summary>
		///		Obtiene una tabla horizontal sobre el PDF
		/// </summary>
		private PdfPTable GetHorizontalTable(HorizontalTableReport reportTable)
		{
			PdfPTable pdfTable = new PdfPTable(reportTable.ColumnsWidth.Count);

				// Ordena las celdas de la tabla
				reportTable.Sort();
				// Añade los anchos de columna a la tabla
				AddTableColumns(pdfTable, reportTable.ColumnsWidth);
				// Añade las filas de la tabla
				AddRows(pdfTable, reportTable.Body, false);
				// Asigna el estilo a la tabla
				AssignStyle(pdfTable, reportTable.CompactStyle(Report));
				// Devuelve la tabla generada
				return pdfTable;
		}

		/// <summary>
		///		Añade las columnas de la tabla
		/// </summary>
		private void AddTableColumns(PdfPTable pdfTable, System.Collections.Generic.List<UnitStyle> columns)
		{
			float[] widths = new float[columns.Count];
			double remainingWidth = _pageWidth;

				// Asigna el ancho a la tabla
				pdfTable.WidthPercentage = 100;
				// Calcula el ancho que queda en la tabla una vez que se añadan los anchos fijos
				foreach (UnitStyle width in columns)
					if (width.Unit == UnitStyle.UnitType.Pixels)
						remainingWidth -= width.Value;
				// Asigna los anchos
				for (int index = 0; index < columns.Count; index++)
					if (columns[index].Unit == UnitStyle.UnitType.Pixels)
						widths[index] = (float) columns[index].Value;
					else
						widths[index] = (float) (remainingWidth * columns[index].Value / 100);
				// Asigna los anchos
				pdfTable.SetWidths(widths);
		}

		/// <summary>
		///		Añade las filas a la tabla
		/// </summary>
		private void AddRows(PdfPTable pdfTable, CellsReportCollection reportCells, bool blnAddEmptyCells)
		{ 
			// Añade las celdas de una fila
			foreach (CellReport reportCell in reportCells)
			{
				PdfPCell pdfCell = new PdfPCell();

					// Añade el contenido
					if (reportCell.Content is ParagraphReport)
						pdfCell.AddElement(GetParagraph(reportCell.Content as ParagraphReport, true));
					else if (reportCell.Content is ListReport)
						pdfCell.AddElement(GetList(reportCell.Content as ListReport, 0));
					else if (reportCell.Content is ImageReport)
						pdfCell.AddElement(GetImage(reportCell.Content as ImageReport));
					else if (reportCell.Content is TableReport)
						pdfCell.AddElement(GetTable(reportCell.Content as TableReport));
					else if (reportCell.Content is SectionReport)
						AddPdfItemsToCell(pdfCell, GetPdfSectionElements(reportCell.Content as SectionReport, true));
					else if (reportCell.Content is HorizontalTableReport)
						pdfCell.AddElement(GetHorizontalTable(reportCell.Content as HorizontalTableReport));
					else if (reportCell.Content is ChartReport)
						System.Diagnostics.Debug.WriteLine("Añadir el gráfico");
					else if (reportCell.Content is LinkReport)
						pdfCell.AddElement(GetLink(reportCell.Content as LinkReport));
					else
						throw new NotImplementedException("Tipo de contenido desconocido para una tabla");
					// Asigna los span
					pdfCell.Colspan = reportCell.ColumnSpan;
					pdfCell.Rowspan = reportCell.RowSpan;
					// Asigna el estilo a la celda
					AssignStyle(pdfCell, reportCell.CompactStyle(Report));
					// Añade la celda a la tabla
					pdfTable.AddCell(pdfCell);
			}
			// Añade celdas vacías
			if (blnAddEmptyCells && reportCells.GetColumnsNumber() != 0)
				for (int index = reportCells.GetColumnsNumber(); index < pdfTable.NumberOfColumns; index++)
					pdfTable.AddCell(new PdfPCell());
		}

		/// <summary>
		///		Añade una serie de elementos de PDF a otro elemento de PDF
		/// </summary>
		private void AddPdfItemsToCell(PdfPCell pdfElementParent, System.Collections.Generic.List<IElement> pdfElements)
		{
			foreach (IElement pdfElement in pdfElements)
				pdfElementParent.AddElement(pdfElement);
		}

		/// <summary>
		///		Obtiene una lista para el PDF a partir de los datos del informe
		/// </summary>
		private List GetList(ListReport reportList, double leftIndent)
		{
			List pdfList;
			StyleReport style = reportList.CompactStyle(Report);

				// Calcula la indentación izquierda
				leftIndent += reportList.LineIndent ?? 5;
				// Crea la lista (ordenada o sin ordenar)
				switch (reportList.Type)
				{
					case ListReport.ListType.Ordered:
							pdfList = new List(true, false);
						break;
					case ListReport.ListType.Roman:
							pdfList = new RomanList();
						break;
					case ListReport.ListType.Alphabetical:
							pdfList = new List(true, true);
						break;
					default:
							pdfList = new List(false, false);
						break;
				}
				pdfList.Alignindent = true;
				pdfList.Autoindent = true;
				pdfList.IndentationLeft = (float) leftIndent;
				//pdfList.SymbolIndent = (float) (reportList.SymbolIndent ?? 40);
				// Símbolo de la lista
				if (!reportList.ShowSymbol)
					pdfList.SetListSymbol("");
				else if (!reportList.Symbol.IsEmpty())
					pdfList.SetListSymbol(reportList.Symbol);
				// Fuente de la lista
				pdfList.Symbol.Font = GetFont(style.Font);
				// Añade los elementos de la lista
				foreach (ListItemReport listItem in reportList.ListItems)
				{ 
					// Añade el texto del elemento
					pdfList.Add(NormalizeText(listItem.Text));
					// Añade la lista interna
					if (listItem.List != null && listItem.List.ListItems.Count > 0)
						pdfList.Add(GetList(listItem.List, leftIndent));
				}
				// Cambia el estilo de los elementos de la lista
				AssignStyle(pdfList, style, reportList);
				// Devuelve la lista
				return pdfList;
		}

		/// <summary>
		///		Obtiene una imagen para PDF
		/// </summary>
		private IElement GetImage(ImageReport reportImage)
		{
			if (!reportImage.FileName.IsEmpty() && System.IO.File.Exists(reportImage.FileName))
			{
				Image pdfImage = Image.GetInstance(reportImage.FileName);

					// Asigna el ancho y alto
					//if (reportImage.Width.IsDefined)
					//  objPdfImage.Width = (float) reportImage.Width.Value;
					//if (reportImage.Height.IsDefined)
					//  objPdfImage.Height = (float) reportImage.Height.Value;
					// Posición
					if (reportImage.StyleClass.Margin.Left.HasValue)
						pdfImage.IndentationLeft = (float) reportImage.StyleClass.Margin.Left.Value;
					if (reportImage.StyleClass.Margin.Right.HasValue)
						pdfImage.IndentationRight = (float) reportImage.StyleClass.Margin.Right.Value;
					if (reportImage.StyleClass.Margin.Top.HasValue)
						pdfImage.SpacingBefore = (float) reportImage.StyleClass.Margin.Top.Value;
					if (reportImage.StyleClass.Margin.Bottom.HasValue)
						pdfImage.SpacingAfter = (float) reportImage.StyleClass.Margin.Bottom.Value;
					// Alineación (por defecto es a la izquierda)
					if (reportImage.StyleClass.HorizontalAlign == StyleReport.HorizontalAlignType.Center)
						pdfImage.Alignment = Element.ALIGN_CENTER;
					if (reportImage.StyleClass.HorizontalAlign == StyleReport.HorizontalAlignType.Right)
						pdfImage.Alignment = Element.ALIGN_RIGHT;
					// Devuelve la imagen
					return pdfImage;
			}
			else
				return new Paragraph("#Imagen no localizada#");
		}

		/// <summary>
		///		Asigna los datos de un vínculo
		/// </summary>
		private IElement GetLink(LinkReport link)
		{
			Anchor anchor = new Anchor(link.Text);

				// Dependiendo del tipo
				if (link.Type == LinkReport.LinkType.Bookmark)
					anchor.Name = link.Target;
				else
					anchor.Reference = link.Target;
				// Devuelve el vínculo
				return anchor;
		}

		/// <summary>
		///		Asigna el estilo de un párrafo
		/// </summary>
		private void AssignStyle(Paragraph paragraph, StyleReport style)
		{ 
			// Asigna la fuente
			paragraph.Font = GetFont(style.Font);
			// Asigna los márgenes
			AssignMargins(paragraph, style);
			// Asigna el padding
			AssignPaddings(paragraph, style);
			// Alineación horizontal
			paragraph.Alignment = GetHorizontalAlign(style.HorizontalAlign);
			// Espaciado de las líneas del párrafo
			paragraph.SetLeading(0, 1.2f);
		}

		/// <summary>
		///		Asigna los márgenes de un párrafo
		/// </summary>
		private void AssignMargins(Paragraph paragraph, StyleReport style)
		{
			if (style.Margin != null)
			{
				if (style.Margin.Top != null)
					paragraph.SpacingBefore = (float) (style.Margin.Top ?? 0);
				if (style.Margin.Right != null)
					paragraph.IndentationRight = (float) (style.Margin.Right ?? 0);
				if (style.Margin.Bottom != null)
					paragraph.SpacingAfter = (float) (style.Margin.Bottom ?? 0);
				if (style.Margin.Left != null)
					paragraph.IndentationLeft = (float) (style.Margin.Left ?? 0);
			}
		}

		/// <summary>
		///		Asigna los padings de un párrafo
		/// </summary>
		private void AssignPaddings(Paragraph paragraph, StyleReport style)
		{
			//if (style.Padding != null)
			//  { if (style.Padding.Top.HasValue)
			//      paragraph.Chunks[0].
			//  }
		}

		/// <summary>
		///		Asigna el estilo a una tabla
		/// </summary>
		private void AssignStyle(PdfPTable pdfTable, StyleReport reportStyle)
		{   
			pdfTable.SpacingBefore = (float) (reportStyle.Margin.Top ?? 0);
			pdfTable.SpacingAfter = (float) (reportStyle.Margin.Bottom ?? 0);
		}

		/// <summary>
		///		Asigna el estilo a una celda
		/// </summary>
		private void AssignStyle(PdfPCell pdfCell, StyleReport style)
		{ 
			// Color de fondo
			if (style.BackGround != null)
				pdfCell.BackgroundColor = ConvertColor(style.BackGround);
			// Borde superior
			if (style.TopBorder.Visible)
			{
				if (style.TopBorder.Color != null)
					pdfCell.BorderColorTop = ConvertColor(style.TopBorder.Color);
				pdfCell.BorderWidthTop = (float) (style.TopBorder.Width / 2);
			}
			else
				pdfCell.DisableBorderSide(Rectangle.TOP_BORDER);
			// Borde derecho
			if (style.RightBorder.Visible)
			{
				if (style.RightBorder.Color != null)
					pdfCell.BorderColorRight = ConvertColor(style.RightBorder.Color);
				pdfCell.BorderWidthRight = (float) (style.RightBorder.Width / 2);
			}
			else
				pdfCell.DisableBorderSide(Rectangle.RIGHT_BORDER);
			// Borde inferior
			if (style.BottomBorder.Visible)
			{
				if (style.BottomBorder.Color != null)
					pdfCell.BorderColorBottom = ConvertColor(style.BottomBorder.Color);
				pdfCell.BorderWidthBottom = (float) (style.BottomBorder.Width / 2);
			}
			else
				pdfCell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
			// Borde izquierdo
			if (style.LeftBorder.Visible)
			{
				if (style.LeftBorder.Color != null)
					pdfCell.BorderColorLeft = ConvertColor(style.LeftBorder.Color);
				pdfCell.BorderWidthLeft = (float) (style.LeftBorder.Width / 2);
			}
			else
				pdfCell.DisableBorderSide(Rectangle.LEFT_BORDER);
			// Alineación
			pdfCell.HorizontalAlignment = GetHorizontalAlign(style.HorizontalAlign);
			pdfCell.VerticalAlignment = GetVerticalAlign(style.VerticalAlign);
			// Padding
			if (style.Padding != null)
			{
				if (style.Padding.Left.HasValue)
					pdfCell.PaddingLeft = (float) style.Padding.Left.Value;
				if (style.Padding.Right.HasValue)
					pdfCell.PaddingRight = (float) style.Padding.Right.Value;
				if (style.Padding.Top.HasValue)
					pdfCell.PaddingTop = (float) style.Padding.Top.Value;
				if (style.Padding.Bottom.HasValue)
					pdfCell.PaddingBottom = (float) style.Padding.Bottom.Value;
			}

		}

		/// <summary>
		///		Asigna el estilo a un chunck
		/// </summary>
		private void AssignStyle(Chunk chunk, StyleReport reportStyle, bool atCell)
		{ 
			// Asigna el estilo al párrafo
			chunk.Font = GetFont(reportStyle.Font);
			// Asigna el color del fondo
			if (reportStyle.BackGround != null && !atCell)
				chunk.SetBackground(ConvertColor(reportStyle.BackGround));
		}

		/// <summary>
		///		Asigna un estilo a los elementos de una lista
		/// </summary>
		private void AssignStyle(List pdfList, StyleReport style, ListReport reportList)
		{ 
			// Asigna el estilo a los elementos de la lista
			foreach (Chunk chunk in pdfList.Chunks)
				AssignStyle(chunk, style, false);
			// Asigna los estilos a los elementos de una lista
			foreach (IElement listItem in pdfList.Items)
				if (listItem is Paragraph)
				{
					(listItem as Paragraph).Alignment = GetHorizontalAlign(style.HorizontalAlign);
					(listItem as Paragraph).SetLeading(0, 1.2f);
					(listItem as Paragraph).SpacingAfter = (float) (reportList.LineSpacing ?? 2);
				}
			// Cambia el espaciado anterior y posterior a la lista
			if ((style.Margin.Top ?? 0) > 0 && pdfList.GetFirstItem() != null)
				pdfList.GetFirstItem().SpacingBefore = (float) (style.Margin.Top ?? 0);
			if ((style.Margin.Bottom ?? 0) > 0 && pdfList.GetLastItem() != null)
				pdfList.GetLastItem().SpacingAfter = (float) (style.Margin.Bottom ?? 0);
		}

		/// <summary>
		///		Obtiene los datos una fuente
		/// </summary>
		private Font GetFont(FontStyleReport reportFont)
		{
			//BaseFont baseFont;
			int intStyle = 0;

				// Si no se ha definido el estilo, se obtiene el estilo predeterminado
				if (!reportFont.IsDefined)
					reportFont = Report.Styles.SearchDefault().Font;
				// Asigna el nombre y tamaño predeterminado de la fuente
				if (reportFont.Name.IsEmpty())
					reportFont.Name = "Helvetica";
				if (reportFont.Size < 1)
					reportFont.Size = 10;
				// Obtiene el estilo de fuente
				if (reportFont.Bold)
					intStyle = Font.BOLD;
				if (reportFont.Italic)
					intStyle |= Font.ITALIC;
				if (reportFont.Underline)
					intStyle |= Font.UNDERLINE;
				//// Obtiene la fuente base
				//baseFont = FontsEmbedded.GetBaseFont(reportFont.Name);
				// Si hay fuente base, crea los datos a partir de ella
				//if (baseFont != null)
				//	return new Font(baseFont, reportFont.Size, intStyle, ConvertColor(reportFont.Color));
				//else
					return FontFactory.GetFont(reportFont.Name, reportFont.Size, intStyle, ConvertColor(reportFont.Color));
		}

		/// <summary>
		///		Obtiene el texto normalizando con los datos específicos de la página
		/// </summary>
		private string NormalizeText(string text)
		{
			if (text.IsEmpty())
				return text;
			else
			{
				string result = text;

					// Quita los espacios fijos
					result = result.ReplaceWithStringComparison("&nbsp;", " ");
					// Asigna el número de página
					result = result.ReplaceWithStringComparison("#Page", ActualPage.ToString());
					// Devuelve el resultado
					return result;
			}
		}

		/// <summary>
		///		Convierte un color de sistema en un color apropiado para el Pdf
		/// </summary>
		private BaseColor ConvertColor(System.Drawing.Color? objColor)
		{
			return new BaseColor(objColor ?? System.Drawing.Color.Black);
		}

		/// <summary>
		///		Obtiene la alineación horizontal
		/// </summary>
		private int GetHorizontalAlign(StyleReport.HorizontalAlignType align)
		{
			switch (align)
			{
				case StyleReport.HorizontalAlignType.Center:
					return Element.ALIGN_CENTER;
				case StyleReport.HorizontalAlignType.Justified:
					return Element.ALIGN_JUSTIFIED;
				case StyleReport.HorizontalAlignType.Right:
					return Element.ALIGN_RIGHT;
				default:
					return Element.ALIGN_LEFT;
			}
		}

		/// <summary>
		///		Obtiene la alineación vertical
		/// </summary>
		private int GetVerticalAlign(StyleReport.VerticalAlignType align)
		{
			switch (align)
			{
				case StyleReport.VerticalAlignType.Middle:
					return Element.ALIGN_MIDDLE;
				case StyleReport.VerticalAlignType.Bottom:
					return Element.ALIGN_BOTTOM;
				default:
					return Element.ALIGN_TOP;
			}
		}

		/// <summary>
		///		Marca de agua actual
		/// </summary>
		internal WaterMarkReport ActualWaterMark { get; private set; }

		/// <summary>
		///		Selector de cabecera actual
		/// </summary>
		internal HeaderFootSelector ActualHeaderSelector { get; private set; }

		/// <summary>
		///		Selector de pie actual
		/// </summary>
		internal HeaderFootSelector ActualFooterSelector { get; private set; }

		/// <summary>
		///		Número de página actual
		/// </summary>
		internal int ActualPage { get; set; }

		///// <summary>
		/////		Fuentes embebidas
		///// </summary>
		//internal FontsEmbeddedPdfCollection FontsEmbedded { get; private set; }

		/// <summary>
		///		Nombre del archivo destino
		/// </summary>
		public string FileName { get; private set; }

		/// <summary>
		///		Informe a generar
		/// </summary>
		public Report Report { get; private set; }
	}
}