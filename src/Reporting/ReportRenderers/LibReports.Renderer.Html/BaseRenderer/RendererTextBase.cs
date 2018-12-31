using System;
using System.Text;

namespace Bau.Libraries.LibReports.Renderer.Html.BaseRenderer
{
	/// <summary>
	///		Clase base para los renderer sobre archivos de texto (XML, Texto, HTML...)
	/// </summary>
	public abstract class RendererTextBase : IReportRenderer
	{ 
		// Variables privadas
		private StringBuilder _builder;
		private int _indent = 0;

		public RendererTextBase()
		{
			_builder = new StringBuilder();
		}

		/// <summary>
		///		Renderiza los datos de un informe
		/// </summary>
		public void Render(string fileTarget, string fileXml)
		{
			// Asigna las propiedades
			FileName = fileTarget;
			Report = new Parser.ReportParser(System.IO.Path.GetDirectoryName(fileTarget)).ParseFile(fileXml);
			// Renderiza el informe
			Process();
		}

		/// <summary>
		///		Procesa y graba el informe
		/// </summary>
		protected abstract void Process();

		/// <summary>
		///		Limpia el texto
		/// </summary>
		protected void Clear()
		{
			_builder = new StringBuilder();
			_indent = 0;
		}

		/// <summary>
		///		Indenta el texto a la derecha
		/// </summary>
		protected void IndentRight()
		{
			_indent += 2;
		}

		/// <summary>
		///		Indenta el texto a la izquierda
		/// </summary>
		protected void IndentLeft()
		{
			_indent -= 2;
			if (_indent < 0)
				_indent = 0;
		}

		/// <summary>
		///		Añade un texto indentándolo a la derecha
		/// </summary>
		protected void AppendRight(string text, bool newLine = true)
		{
			IndentRight();
			Append(text, newLine);
		}

		/// <summary>
		///		Añade un texto indentándolo a la izquierda
		/// </summary>
		protected void AppendLeft(string text, bool newLine = true)
		{
			IndentLeft();
			Append(text);
		}

		/// <summary>
		///		Añade un texto indentado
		/// </summary>
		protected void Append(string text, bool newLine = true)
		{
			if (!string.IsNullOrEmpty(text))
			{ 
				// Añade la indentación
				for (int index = 0; index < _indent; index++)
					_builder.Append(' ');
				// Añade el texto
				if (newLine)
					_builder.AppendLine(text);
				else
					_builder.Append(text);
			}
		}

		/// <summary>
		///		Añade un salto de línea
		/// </summary>
		protected void AppendLine()
		{
			_builder.AppendLine();
		}

		/// <summary>
		///		Graba un archivo de texto
		/// </summary>
		protected void Save()
		{ 
			LibCommonHelper.Files.HelperFiles.MakePath(System.IO.Path.GetDirectoryName(FileName));
			LibCommonHelper.Files.HelperFiles.SaveTextFile(FileName, _builder.ToString(), Encoding.UTF8);
		}

		/// <summary>
		///		Nombre de archivo
		/// </summary>
		protected string FileName { get; private set; }

		/// <summary>
		///		Informe
		/// </summary>
		protected Models.Report Report { get; private set; }
	}
}