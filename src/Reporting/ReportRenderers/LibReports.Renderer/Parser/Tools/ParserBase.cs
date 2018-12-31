using System;

using Bau.Libraries.LibCommonHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibReports.Renderer.Models.Base;

namespace Bau.Libraries.LibReports.Renderer.Parser.Tools
{
	/// <summary>
	///		Base de los intérpretes
	/// </summary>
	internal abstract class ParserBase<TypeData> : IReportItemParser where TypeData : ClassBase
	{
		internal ParserBase(ReportParser reportParser)
		{
			ReportParser = reportParser;
		}

		/// <summary>
		///		Interpreta un nodo
		/// </summary>
		internal TypeData Parse(ContentReportBase parent, MLNode nodeML)
		{
			TypeData content;

				// Asigna el elemento padre
				Parent = parent;
				// Interpreta el contenido
				content = ParseInner(nodeML);
				// Interpreta el contenido común
				if (content is ContentReportBase)
					ParseContentCommon(content as ContentReportBase, nodeML);
				// Devuelve el contenido
				return content;
		}

		/// <summary>
		///		Implementa la interface
		/// </summary>
		ClassBase IReportItemParser.Parse(ContentReportBase parent, MLNode nodeML)
		{
			return Parse(parent, nodeML);
		}

		/// <summary>
		///		Interpreta el contenido de un nodo
		/// </summary>
		protected abstract TypeData ParseInner(MLNode nodeML);

		/// <summary>
		///		Interpreta las partes comunes de un archivo XML
		/// </summary>
		protected void ParseContentCommon(ContentReportBase content, MLNode nodeML)
		{
			content.ID = nodeML.Attributes["id"].Value;
			content.ClassId = nodeML.Attributes["ClassId"].Value;
			content.StyleClass = new StyleParser(ReportParser).Parse(content, nodeML);
			content.NewPageBefore = nodeML.Attributes["NewPageBefore"].Value.GetBool();
			content.NewPageAfter = nodeML.Attributes["NewPageAfter"].Value.GetBool();
		}

		/// <summary>
		///		Parser de informe al que se asocian los datos
		/// </summary>
		internal ReportParser ReportParser { get; }

		/// <summary>
		///		Padre del elemento que se está interpretando
		/// </summary>
		internal ContentReportBase Parent { get; set; }

		/// <summary>
		///		Clase con métodos comunes para el parser
		/// </summary>
		internal ParserBaseHelper ParserHelper { get; } = new ParserBaseHelper();
	}
}
