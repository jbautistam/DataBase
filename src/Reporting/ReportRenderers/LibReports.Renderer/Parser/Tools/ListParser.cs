using System;

using Bau.Libraries.LibCommonHelper.Extensors;
using Bau.Libraries.LibReports.Renderer.Models.Contents;
using Bau.Libraries.LibMarkupLanguage;

namespace Bau.Libraries.LibReports.Renderer.Parser.Tools
{
	/// <summary>
	///		Intérprete de listas
	/// </summary>
	internal class ListParser : ParserBase<ListReport>
	{
		internal ListParser(ReportParser reportParser) : base(reportParser) { }

		/// <summary>
		///		Interpreta la lista a partir de un nodo XML
		/// </summary>
		protected override ListReport ParseInner(MLNode nodeML)
		{
			ListReport list = new ListReport(Parent);

				// Añade los datos de estilo
				if (list.ClassId.IsEmpty())
					list.ClassId = Parent.ClassId;
				// Asigna los datos de los atributos
				list.Type = nodeML.Attributes["Type"].Value.GetEnum(ListReport.ListType.Unordered);
				list.Symbol = nodeML.Attributes["Symbol"].Value;
				list.SymbolIndent = nodeML.Attributes["SymbolIndent"].Value.GetDouble();
				list.ShowSymbol = nodeML.Attributes["ShowSymbol"].Value.GetBool(true);
				list.LineIndent = nodeML.Attributes["LineIndent"].Value.GetDouble();
				list.LineSpacing = nodeML.Attributes["LineSpacing"].Value.GetDouble();
				// Lee los datos del nodo
				foreach (MLNode childML in nodeML.Nodes)
					switch (childML.Name)
					{
						//case "DataTableProvider":
						//		list.Command = ReportParser.ParserLocator.ParseDataComand(list, childML);
						//	break;
						case "ListItem":
								list.ListItems.Add(ParseListItem(list, childML));
							break;
						default:
							throw new NotImplementedException($"Error al interpretar una lista. Nodo desconocido: {nodeML.Name}");
					}
				// Devuelve los datos de la lista
				return list;
		}

		/// <summary>
		///		Interpreta el elemento de la lista
		/// </summary>
		private ListItemReport ParseListItem(ListReport list, MLNode nodeML)
		{
			ListItemReport listItem = new ListItemReport(list);

				// Interpreta los datos básicos
				base.ParseContentCommon(listItem, nodeML);
				if (listItem.ClassId.IsEmpty())
					listItem.ClassId = list.ClassId;
				// Interpreta el contenido del elemento de la lista
				if (nodeML.Nodes.Count == 0)
					listItem.Text = ParserHelper.Normalize(nodeML.Value);
				else
					foreach (MLNode childML in nodeML.Nodes)
						switch (childML.Name)
						{
							case "Text":
									listItem.Text = ParserHelper.Normalize(childML.Value);
								break;
							case "List":
									listItem.List = Parse(list, childML);
								break;
							default:
								throw new NotImplementedException($"No se reconoce el nodo de la lista - Nodo: {childML.Name}");
						}
				// Devuelve el elemento de la lista
				return listItem;
		}
	}
}
