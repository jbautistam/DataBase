using System;

using Bau.Libraries.LibReports.Renderer.Models.Base;

namespace Bau.Libraries.LibReports.Renderer.Models.Contents
{
	/// <summary>
	///		Elemento de lista de un informe
	/// </summary>
	public class ListReport : ContentCommandReportBase
	{ 
		// Enumerados públicos
		/// <summary>
		///		Tipo de lista
		/// </summary>
		public enum ListType
		{
			/// <summary>Desconocido</summary>
			Unknown,
			/// <summary>Ordenado numéricamente</summary>
			Ordered,
			/// <summary>Sin orden</summary>
			Unordered,
			/// <summary>Ordenado en números romanos</summary>
			Roman,
			/// <summary>Orden alfabético</summary>
			Alphabetical
		}

		public ListReport(ContentReportBase parent) : base(parent)
		{
			ListItems = new ListItemsReportCollection();
		}

		/// <summary>
		///		Clona la definición de la lista
		/// </summary>
		public ListReport CloneDefinition(ContentReportBase parent)
		{
			ListReport list = new ListReport(parent);

				// Clona los datos básicos
				base.CloneToTarget(list);
				// Asigna las propiedades
				list.Type = Type;
				list.Symbol = Symbol;
				list.ShowSymbol = ShowSymbol;
				list.SymbolIndent = SymbolIndent;
				list.LineIndent = LineIndent;
				list.LineSpacing = LineSpacing;
				// Devuelve la lista clonada
				return list;
		}

		/// <summary>
		///		Indica el tipo de lista
		/// </summary>
		public ListType Type { get; set; }

		/// <summary>
		///		Símbolo de la lista
		/// </summary>
		public string Symbol { get; set; }

		/// <summary>
		///		Indica si se debe mostrar el símbolo de los elementos de la lista
		/// </summary>
		public bool ShowSymbol { get; set; }

		/// <summary>
		///		Indentación del símbolo con respecto al texto
		/// </summary>
		public double? SymbolIndent { get; set; }

		/// <summary>
		///		Indentación de la línea con respecto al margen izquierdo
		/// </summary>
		public double? LineIndent { get; set; }

		/// <summary>
		///		Espaciado entre los elementos de la lista
		/// </summary>
		public double? LineSpacing { get; set; }

		/// <summary>
		///		Colección de elementos
		/// </summary>
		public ListItemsReportCollection ListItems { get; set; }

		/// <summary>
		///		Comprueba si están definidos elementos de lista
		/// </summary>
		public bool IsDefined
		{
			get { return ListItems.Count != 0; }
		}
	}
}
