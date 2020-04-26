using System;

namespace Bau.Libraries.LibDbScripts.Parser
{
	/// <summary>
	///		Datos de una sección de un script de SQL
	/// </summary>
	public class SqlSectionModel
	{
		/// <summary>
		///		Tipo de sección
		/// </summary>
		public enum SectionType
		{
			/// <summary>Comando SQL</summary>
			Sql,
			/// <summary>Comentario</summary>
			Comment,
			/// <summary>Comando externo (SqlCmd)</summary>
			ExternalCommand
		}

		public SqlSectionModel(SectionType type, string content)
		{
			Type = type;
			Content = content;
		}

		/// <summary>
		///		Tipo
		/// </summary>
		public SectionType Type { get; }

		/// <summary>
		///		Contenido
		/// </summary>
		public string Content { get; set; }
	}
}
