using System;
using System.Collections.Generic;

using iTextSharp.text.pdf;
using iTextSharp.text;
using Bau.Libraries.LibCommonHelper.Extensors;
using Bau.Libraries.LibReports.Renderer.Models.Base;
using Bau.Libraries.LibReports.Renderer.Models.Contents;

namespace Bau.Libraries.LibReports.Renderer.PDF.Tools
{
	/// <summary>
	///		Handler para los eventos de página
	/// </summary>
	internal class RendererPdfPageHandler : PdfPageEventHelper
	{ 
		// Variables privadas
		private RendererPdf _rendererPdf;

		internal RendererPdfPageHandler(RendererPdf rendererPdf)
		{
			_rendererPdf = rendererPdf;
			rendererPdf.ActualPage = 0;
		}

		/// <summary>
		///		Trata el evento de fin de página
		/// </summary>
		public override void OnEndPage(PdfWriter writer, Document document)
		{ 
			// Incrementa el número de página
			_rendererPdf.ActualPage++;
			// Escribe la cabecera
			WriteHeader(writer, document);
			// Escribe el pie
			WriteFoot(writer, document);
			// Escribe el WaterMark
			WriteWaterMark(document);
		}

		/// <summary>
		///		Escribe la cabecera
		/// </summary>
		private void WriteHeader(PdfWriter pdfWriter, Document pdfDocument)
		{
			if (_rendererPdf.ActualHeaderSelector == null || _rendererPdf.ActualHeaderSelector.Visible)
			{
				HeaderFootReport headerReport = _rendererPdf.Report.HeadersAndFoots.Search(_rendererPdf.ActualPage,
																						   HeaderFootReport.HeaderFootType.Header);
				SectionReport sectionHeader;

					// Obtiene la sección que actúa como cabecera (o la predeterminada)
					if (headerReport == null)
						sectionHeader = _rendererPdf.Report.Header.Content;
					else
						sectionHeader = headerReport.Content;
					// Dibuja la cabecera
					if (sectionHeader.Contents.Count > 0)
					{
						PdfPTable pdfTable = ComputeTable(sectionHeader, pdfDocument);

							// Escribe la tabla de cabecera directamente en una posición absoluta
							//! Tener en cuenta que el origen de coordenadas de un PDF está en la esquina inferior izquierda
							pdfTable.WriteSelectedRows(0, -1, pdfDocument.LeftMargin,
														  pdfDocument.PageSize.Height + pdfTable.TotalHeight - pdfDocument.TopMargin + 10,
														  pdfWriter.DirectContent);
					}
			}
		}

		/// <summary>
		///		Escribe el pie
		/// </summary>
		private void WriteFoot(PdfWriter pdfWriter, Document pdfDocument)
		{
			if (_rendererPdf.ActualFooterSelector == null || _rendererPdf.ActualFooterSelector.Visible)
			{
				HeaderFootReport footReport = _rendererPdf.Report.HeadersAndFoots.Search(_rendererPdf.ActualPage,
																						 HeaderFootReport.HeaderFootType.Foot);
				SectionReport sectionFoot;

					// Obtiene la sección que actúa como pie para esta página o la predeterminada
					if (footReport == null)
						sectionFoot = _rendererPdf.Report.Foot.Content;
					else
						sectionFoot = footReport.Content;
					// Dibuja el pie
					if (sectionFoot.Contents.Count > 0)
					{
						PdfPTable pdfTable = ComputeTable(sectionFoot, pdfDocument);

						// Escribe la tabla de pie directamente en una posición absoluta
						//! Tener en cuenta que el origen de coordenadas de un PDF está en la esquina inferior izquierda
						pdfTable.WriteSelectedRows(0, -1, pdfDocument.LeftMargin,
												   pdfDocument.BottomMargin, pdfWriter.DirectContent);
					}
			}
		}

		/// <summary>
		///		Calcula la tabla de una sección
		/// </summary>
		private PdfPTable ComputeTable(SectionReport section, Document pdfDocument)
		{
			PdfPTable pdfTable = new PdfPTable(1);
			List<IElement> elements;

				// Asigna el ancho de la página a toda la tabla
				pdfTable.TotalWidth = pdfDocument.PageSize.Width - pdfDocument.LeftMargin - pdfDocument.RightMargin;
				// Obtiene los elementos de la sección
				elements = _rendererPdf.GetPdfSectionElements(section, false);
				// Añade los elementos como celdas de tabla
				foreach (IElement pdfElement in elements)
				{
					PdfPCell pdfCell = new PdfPCell(pdfTable);

						// Crea la celda
						pdfCell.Border = PdfPCell.NO_BORDER;
						pdfCell.VerticalAlignment = Element.ALIGN_BOTTOM;
						// Añade el elemento a la celda
						pdfCell.AddElement(pdfElement);
						// Añade la celda a la tabla
						pdfTable.AddCell(pdfCell);
				}
				// Devuelve la tabla
				return pdfTable;
		}

		/// <summary>
		///		Escribe la marca de agua
		/// </summary>
		private void WriteWaterMark(Document pdfDocument)
		{
			if (_rendererPdf.ActualWaterMark != null &&
					  _rendererPdf.ActualWaterMark.Mode == WaterMarkReport.ModeWaterMark.Start)
			{ 
				// Asigna el color de fondo de la página
				if (_rendererPdf.ActualWaterMark.BackgroundPage != null)
				{
					Rectangle rectangle = new Rectangle(pdfDocument.PageSize);

						rectangle.BackgroundColor = new BaseColor(_rendererPdf.ActualWaterMark.BackgroundPage ?? System.Drawing.Color.White);
						pdfDocument.Add(rectangle);
				}
				// Asigna la imagen de marca de agua
				foreach (ContentReportBase content in _rendererPdf.ActualWaterMark.Images)
					if (content != null && content is ImageReport)
					{
						ImageReport image = content as ImageReport;

							if (!image.FileName.IsEmpty() && System.IO.File.Exists(image.FileName))
							{
								Image imagePdf = Image.GetInstance(image.FileName);

									// Si la imagen debe ocupar toda la página, se asigna al fondo
									if (_rendererPdf.ActualWaterMark.ImagesMustFitPage)
									{ 
										// Cambia el tamaño de la imagen
										imagePdf.ScaleToFit(3000, 770);
										// Se indica que la imagen debe almacenarse como fondo
										imagePdf.Alignment = Image.UNDERLYING;
										// Coloca la imagen en una posición absoluta
										imagePdf.SetAbsolutePosition(7, 69);
									}
									// Imprime la imagen como fondo de página
									pdfDocument.Add(imagePdf);
							}
					}
			}
		}
	}
}