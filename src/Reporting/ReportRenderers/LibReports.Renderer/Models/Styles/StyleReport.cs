using System;

using Bau.Libraries.LibCommonHelper.Extensors;
using Bau.Libraries.LibReports.Renderer.Models.Base;

namespace Bau.Libraries.LibReports.Renderer.Models.Styles
{
	/// <summary>
	///		Clase con los datos de un estilo
	/// </summary>
	public class StyleReport : Base.ClassBase
	{
		/// <summary>
		///		Modo de alineación horizontal
		/// </summary>
		public enum HorizontalAlignType
		{
			/// <summary>Desconocido</summary>
			Unkown,
			/// <summary>Izquierda</summary>
			Left,
			/// <summary>Derecha</summary>
			Right,
			/// <summary>Centro</summary>
			Center,
			/// <summary>Justificado</summary>
			Justified
		}

		/// <summary>
		///		Modo de alineación vertical
		/// </summary>
		public enum VerticalAlignType
		{
			/// <summary>Desconocido</summary>
			Unknown,
			/// <summary>Superior</summary>
			Top,
			/// <summary>Inferior</summary>
			Bottom,
			/// <summary>Centro</summary>
			Middle
		}

		public StyleReport(ContentReportBase parent, string id)
		{ 
			// Inicializa el ID
			ID = id;
			Parent = parent;
			// Inicializa la fuente
			Font = new FontStyleReport();
			// Inicializa la alineación
			HorizontalAlign = HorizontalAlignType.Unkown;
			VerticalAlign = VerticalAlignType.Unknown;
			// Asigna el margen y padding
			Margin = new MarginStyleReport();
			Padding = new MarginStyleReport();
			// Inicializa los bordes
			TopBorder = new BorderStyleReport();
			BottomBorder = new BorderStyleReport();
			LeftBorder = new BorderStyleReport();
			RightBorder = new BorderStyleReport();
		}

		/// <summary>
		///		Clona los datos del estilo
		/// </summary>
		public StyleReport Clone()
		{
			StyleReport style = new StyleReport(Parent, ID);

				// Asigna las propiedades
				style.Font = Font.Clone();
				style.BackGround = BackGround;
				style.HorizontalAlign = HorizontalAlign;
				style.VerticalAlign = VerticalAlign;
				style.Angle = Angle;
				style.IsDefault = IsDefault;
				// Asigna el margen, padding y espaciado
				style.Margin = Margin.Clone();
				style.Padding = Padding.Clone();
				style.Spacing = Spacing;
				// Asigna los bordes
				style.TopBorder = TopBorder.Clone();
				style.BottomBorder = BottomBorder.Clone();
				style.LeftBorder = LeftBorder.Clone();
				style.RightBorder = RightBorder.Clone();
				// Devuelve el estilo clonado
				return style;
		}

		/// <summary>
		///		Mezcla el estilo
		/// </summary>
		internal StyleReport Merge(StyleReport styleParent)
		{
			StyleReport style = Clone();

				// Mezcla los estilos
				if (styleParent != null)
				{ 
					// Mezcla la fuente
					if (!style.Font.IsDefined)
						style.Font = styleParent.Font.Clone();
					else
					{
						if (style.Font.Name.IsEmpty())
							style.Font.Name = styleParent.Font.Name;
						if (style.Font.Size == 0)
							style.Font.Size = styleParent.Font.Size;
						if (style.Font.Color == null)
							style.Font.Color = styleParent.Font.Color;
					}
					// Mezcla el color del fondo
					style.BackGround = style.BackGround ?? styleParent.BackGround;
					// Mezcla las alineaciones
					if (style.HorizontalAlign == HorizontalAlignType.Unkown)
						style.HorizontalAlign = styleParent.HorizontalAlign;
					if (style.VerticalAlign == VerticalAlignType.Unknown)
						style.VerticalAlign = styleParent.VerticalAlign;
					// Mezca los ángulos
					style.Angle = style.Angle ?? styleParent.Angle;
					// Mezcla los márgenes
					MergeMargins(style.Margin, styleParent.Margin);
					MergeMargins(style.Padding, styleParent.Padding);
					// Mezcla los bordes
					MergeBorders(style, styleParent);
				}
				// Devuelve el estilo
				return style;
		}

		/// <summary>
		///		Mezcla los bordes
		/// </summary>
		private void MergeBorders(StyleReport style, StyleReport styleParent)
		{
			if (!style.LeftBorder.Visible && styleParent.LeftBorder.Visible)
				style.LeftBorder = styleParent.LeftBorder;
			if (!style.TopBorder.Visible && styleParent.TopBorder.Visible)
				style.TopBorder = styleParent.TopBorder;
			if (!style.RightBorder.Visible && styleParent.RightBorder.Visible)
				style.RightBorder = styleParent.RightBorder;
			if (!style.BottomBorder.Visible && styleParent.BottomBorder.Visible)
				style.BottomBorder = styleParent.BottomBorder;
		}

		/// <summary>
		///		Mezcla los márgenes
		/// </summary>
		private void MergeMargins(MarginStyleReport margin, MarginStyleReport marginParent)
		{
			if (margin.Top == null && marginParent.Top != null)
				margin.Top = marginParent.Top;
			if (margin.Right == null && marginParent.Right != null)
				margin.Right = marginParent.Right;
			if (margin.Bottom == null && marginParent.Bottom != null)
				margin.Bottom = marginParent.Bottom;
			if (margin.Left == null && marginParent.Left != null)
				margin.Left = marginParent.Left;
		}

		/// <summary>
		///		Objeto padre
		/// </summary>
		public ContentReportBase Parent { get; private set; }

		/// <summary>
		///		Indica si es el estilo predeterminado
		/// </summary>
		public bool IsDefault { get; set; }

		/// <summary>
		///		Fuente 
		/// </summary>
		public FontStyleReport Font { get; set; }

		/// <summary>
		///		Color de fondo
		/// </summary>
		public System.Drawing.Color? BackGround { get; set; }

		/// <summary>
		///		Alineación horizontal
		/// </summary>
		public HorizontalAlignType HorizontalAlign { get; set; }

		/// <summary>
		///		Alineación vertical
		/// </summary>
		public VerticalAlignType VerticalAlign { get; set; }

		/// <summary>
		///		Angulo de la etiqueta
		/// </summary>
		public double? Angle { get; set; }

		/// <summary>
		///		Margen asociado al estilo
		/// </summary>
		public MarginStyleReport Margin { get; set; }

		/// <summary>
		///		Padding asociado al estilo
		/// </summary>
		public MarginStyleReport Padding { get; set; }

		/// <summary>
		///		Espaciado
		/// </summary>
		public double? Spacing { get; set; }

		/// <summary>
		///		Borde superior
		/// </summary>
		public BorderStyleReport TopBorder { get; set; }

		/// <summary>
		///		Borde inferior
		/// </summary>
		public BorderStyleReport BottomBorder { get; set; }

		/// <summary>
		///		Borde izquierdo
		/// </summary>
		public BorderStyleReport LeftBorder { get; set; }

		/// <summary>
		///		Borde derecho
		/// </summary>
		public BorderStyleReport RightBorder { get; set; }

		/// <summary>
		///		Indica si se ha definido el estilo
		/// </summary>
		public bool IsDefined
		{
			get
			{
				return Font.IsDefined || BackGround != null || HorizontalAlign != HorizontalAlignType.Unkown ||
							   VerticalAlign != VerticalAlignType.Unknown || Angle != null ||
							   Margin.Visible || TopBorder.Visible || BottomBorder.Visible ||
							   LeftBorder.Visible || RightBorder.Visible;
			}
		}
	}
}
