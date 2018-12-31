using System;

namespace Bau.Libraries.LibReports.Renderer.Models.Contents
{
	/// <summary>
	///		Definición de los datos de una columna de una serie de informe <see cref="ChartSerieColumnDataReport"/>
	/// </summary>
	public class ChartSerieDataDefinition : Base.ClassBase
	{
		/// <summary>
		///		Clona la definición
		/// </summary>
		internal ChartSerieDataDefinition CloneDefinition()
		{
			ChartSerieDataDefinition definition = new ChartSerieDataDefinition();

				// Asigna las propiedades
				definition.Title = Title;
				definition.FieldTitle = FieldTitle;
				definition.FieldValue = FieldValue;
				definition.FieldMaximum = FieldMaximum;
				definition.FieldMinimum = FieldMinimum;
				// Devuelve el objeto clonado
				return definition;
		}

		/// <summary>
		///		Título de la columna de datos
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		///		Campo con el título de los datos
		/// </summary>
		public string FieldTitle { get; set; }

		/// <summary>
		///		Campo del que se recoge el valor
		/// </summary>
		public string FieldValue { get; set; }

		/// <summary>
		///		Campo del que se recoge el valor máximo
		/// </summary>
		public string FieldMaximum { get; set; }

		/// <summary>
		///		Campo del que se recoge el valor mínimo
		/// </summary>
		public string FieldMinimum { get; set; }
	}
}
