using System;

namespace Bau.Libraries.LibReports.Renderer.Models.Base
{
	/// <summary>
	///		Base de un elemento de contenido con un comando SQL
	/// </summary>
	public abstract class ContentCommandReportBase : ContentReportBase
	{
		public ContentCommandReportBase(ContentReportBase parent) : base(parent) { }

		///// <summary>
		/////		Comando SQL
		///// </summary>
		//public Commands.DataProviderCommand Command { get; set; } = new Commands.DataProviderCommand();
	}
}
