using System;

using Bau.Libraries.LibCommonHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibReports.Renderer.Models;
using Bau.Libraries.LibReports.Renderer.Models.Base;
using Bau.Libraries.LibReports.Renderer.Models.Styles;

namespace Bau.Libraries.LibReports.Renderer.Parser.Tools
{
	/// <summary>
	///		Parser para los estilos
	/// </summary>
	internal class StyleParser : ParserBase<StyleReport>
	{
		internal StyleParser(ReportParser reportParser) : base(reportParser) { }

		/// <summary>
		///		Carga la colección de estilos de un nodo XML
		/// </summary>
		protected override StyleReport ParseInner(MLNode nodeML)
		{
			return ParseNode(Parent, nodeML);
		}

		/// <summary>
		///		Interpreta un estilo
		/// </summary>
		internal StyleReport ParseNode(ContentReportBase parent, MLNode nodeML)
		{
			StyleReport style = new StyleReport(Parent, nodeML.Attributes["id"].Value);

				// Asigna el resto de los estilos
				style.HorizontalAlign = GetHorizontalAlign(nodeML.Attributes["HorizontalAlign"].Value);
				style.VerticalAlign = GetVerticalAlign(nodeML.Attributes["VerticalAlign"].Value);
				style.Margin = ParseMargin("Margin", nodeML);
				style.Padding = ParseMargin("Padding", nodeML);
				style.Angle = nodeML.Attributes["Angle"].Value.GetInt(0);
				style.Spacing = nodeML.Attributes["Spacing"].Value.GetDouble(1.2);
				style.BackGround = ParserHelper.GetColor(nodeML.Attributes["Backcolor"].Value);
				style.IsDefault = nodeML.Attributes["Default"].Value.GetBool();
				// Interpreta los bordes
				ParseBorders(style, nodeML);
				// Asigna la fuente
				style.Font = GetFont(nodeML.Attributes["Font"].Value);
				// Devuelve el estilo
				return style;
		}

		/// <summary>
		///		Obtiene una fuente. El formato del atributo es del tipo Font="Name:Courier;Size:10;Bold;Italic;Underline;"
		/// </summary>
		private FontStyleReport GetFont(string fontDefinition)
		{
			FontStyleReport font = new FontStyleReport();

			// Obtiene los datos de la fuente
			if (!fontDefinition.IsEmpty())
			{
				string[] fontParts = fontDefinition.Split(';');

					if (fontParts != null && fontParts.Length > 0)
						foreach (string fontStyle in fontParts)
							if (fontStyle.EqualsIgnoreCase("Bold"))
								font.Bold = true;
							else if (fontStyle.EqualsIgnoreCase("Italic"))
								font.Italic = true;
							else if (fontStyle.EqualsIgnoreCase("Underline"))
								font.Underline = true;
							else if (!fontStyle.IsEmpty())
							{
								string[] fontStyles = fontStyle.Split(':');

									if (fontStyles != null && fontStyles.Length == 2)
									{ 
										// Quita los espacios
										fontStyles[0] = fontStyles[0].TrimIgnoreNull();
										fontStyles[1] = fontStyles[1].TrimIgnoreNull();
										// Interpreta el contenido
										if (fontStyles[0].EqualsIgnoreCase("Name"))
											font.Name = fontStyles[1];
										else if (fontStyles[0].EqualsIgnoreCase("Size"))
											font.Size = fontStyles[1].GetInt(9);
										else if (fontStyles[0].EqualsIgnoreCase("Color"))
											font.Color = ParserHelper.GetColor(fontStyles[1]);
									}
							}
			}
			// Devuelve la fuente
			return font;
		}

		/// <summary>
		///		Interpreta los bordes de un estilo definido en el XML
		/// </summary>
		private void ParseBorders(StyleReport style, MLNode nodeML)
		{
			if (!nodeML.Attributes["Border"].Value.IsEmpty())
			{
				style.TopBorder = AssignBorder(nodeML.Attributes["Border"].Value);
				style.LeftBorder = AssignBorder(nodeML.Attributes["Border"].Value);
				style.RightBorder = AssignBorder(nodeML.Attributes["Border"].Value);
				style.BottomBorder = AssignBorder(nodeML.Attributes["Border"].Value);
			}
			else
			{
				style.TopBorder = AssignBorder(nodeML.Attributes["BorderTop"].Value);
				style.LeftBorder = AssignBorder(nodeML.Attributes["BorderLeft"].Value);
				style.RightBorder = AssignBorder(nodeML.Attributes["BorderRight"].Value);
				style.BottomBorder = AssignBorder(nodeML.Attributes["BorderBottom"].Value);
			}
		}

		/// <summary>
		///		Asigna el estilo de un borde. El estilo en el XML tiene este formato: Line:Solid;Width:1;Color:#000000;
		/// </summary>
		private BorderStyleReport AssignBorder(string style)
		{
			BorderStyleReport border = new BorderStyleReport();

				// Interpreta el estilo
				if (!style.IsEmpty())
				{
					string[] partsStyle = style.Split(';');

						// Asigna las propiedades básicas
						border.Type = BorderStyleReport.BorderType.Solid;
						border.Width = 1;
						// Asigna los estilos del atributo
						if (partsStyle != null && partsStyle.Length > 0)
							foreach (string stylePart in partsStyle)
							{
								string[] borderParts = stylePart.Split(':');

									if (borderParts != null && borderParts.Length == 2 && !borderParts[0].IsEmpty() && !borderParts[1].IsEmpty())
									{ 
										// Quita los espacios
										borderParts[0] = borderParts[0].Trim();
										borderParts[1] = borderParts[1].Trim();
										// Color
										if (borderParts[0].EqualsIgnoreCase("Color"))
											border.Color = ParserHelper.GetColor(borderParts[1]);
										// Ancho
										if (borderParts[0].EqualsIgnoreCase("Width"))
											border.Width = borderParts[1].GetDouble(0);
										// Tipo de líena
										if (borderParts[0].EqualsIgnoreCase("Line"))
											border.Type = GetBorderType(borderParts[1]);
									}
							}
				}
				// Devuelve el estilo
				return border;
		}

		/// <summary>
		///		Obtiene el tipo de borde
		/// </summary>
		private BorderStyleReport.BorderType GetBorderType(string value)
		{
			return value.GetEnum(BorderStyleReport.BorderType.Solid);
		}

		/// <summary>
		///		Obtiene la alineación horizontal
		/// </summary>
		private StyleReport.HorizontalAlignType GetHorizontalAlign(string align)
		{
			return align.GetEnum(StyleReport.HorizontalAlignType.Unkown);
		}

		/// <summary>
		///		Obtiene la alineación vertical
		/// </summary>
		private StyleReport.VerticalAlignType GetVerticalAlign(string align)
		{
			return align.GetEnum(StyleReport.VerticalAlignType.Unknown);
		}

		/// <summary>
		///		Interpreta los datos de un margen
		/// </summary>
		internal MarginStyleReport ParseMargin(string prefix, MLNode nodeML)
		{
			MarginStyleReport margin = new MarginStyleReport();

				// Interpreta el margen
				if (!nodeML.Attributes[prefix].Value.IsEmpty())
				{
					margin.Top = nodeML.Attributes[prefix].Value.GetDouble();
					margin.Left = nodeML.Attributes[prefix].Value.GetDouble();
					margin.Right = nodeML.Attributes[prefix].Value.GetDouble();
					margin.Bottom = nodeML.Attributes[prefix].Value.GetDouble();
				}
				else
				{
					margin.Top = nodeML.Attributes[prefix + "Top"].Value.GetDouble();
					margin.Left = nodeML.Attributes[prefix + "Left"].Value.GetDouble();
					margin.Right = nodeML.Attributes[prefix + "Right"].Value.GetDouble();
					margin.Bottom = nodeML.Attributes[prefix + "Bottom"].Value.GetDouble();
				}
				// Devuelve el margen
				return margin;
		}
	}
}
