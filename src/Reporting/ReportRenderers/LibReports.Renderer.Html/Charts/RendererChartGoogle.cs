using System;
using System.Text;

using Bau.Libraries.LibCommonHelper.Extensors;
using Bau.Libraries.LibReports.Renderer.Models.Contents;

namespace Bau.Libraries.LibReports.Renderer.Html.Charts
{
	/// <summary>
	///		Clase de obtención de los elementos de dibujo para un gráfico utilizando las API de javaScript de Google
	/// </summary>
	internal class RendererChartGoogle
	{ 
		// Variables privadas
		private StringBuilder _builder = new StringBuilder();

		/// <summary>
		///		Representa un gráfico
		/// </summary>
		internal string Render(ChartReport chart)
		{ 
			// Vacía el buffer
			_builder.Clear();
			// Genera el gráfico
			if (chart != null)
			{ 
				// Obtiene el script principal (importación de Google Chart)
				CreateJavaScriptChart();
				// Crea el gráfico
				CreateChartCode(chart);
			}
			// Devuelve el HTML del gráfico
			return _builder.ToString();
		}

		/// <summary>
		///		Añade el javaScript de tratamiento de gráficos
		/// </summary>
		private void CreateJavaScriptChart()
		{
			if (!IsAddChartScript)
			{ 
				// Añade el script
				Append(@"<script type='text/javascript' src='https://www.google.com/jsapi'></script>
														<script type='text/javascript'>

															// Load the Visualization API and the piechart package.
															google.load('visualization', '1', {'packages':['corechart']});

															// Set a callback to run when the Google Visualization API is loaded.
															// google.setOnLoadCallback(drawChart);
														</script>");
				// Indica que ya se ha añadido el script de gráficos
				IsAddChartScript = true;
			}
		}

		/// <summary>
		///		Añade los datos de un gráfico
		/// </summary>
		private void CreateChartCode(ChartReport chart)
		{
			string idChart = "chart_div_" + IndexChart++;

				// Crea el div que contendrá el gráfico
				Append("<div id='" + idChart + "' style='width:100%;height:100%'></div>");
				// Abre el script de dibujo
				Append("<script type='text/javascript'>");
				//string strData = GetData(chart);
				//string strDraw = GetDraw(chart);

				// Añade el contenido del gráfico al script
				Append(" function drawChart" + idChart + "() {");
				// Crea los datos de la tabla
				Append(GetData(chart));
				// Asigna las opciones al gráfico
				Append("var options = " + GetOptions(chart) + ";");
				// Crea el gráfico
				Append(GetDrawFunction(chart, idChart));
				// Dibuja el gráfico
				Append("chart.draw(data, options);");
				// Cierra la función
				Append("}");
				// Añade la llamada a la función y cierra el script
				Append("drawChart" + idChart + "();");
				Append("</script>");
		}

		/// <summary>
		///		Obtiene la función de dibujo
		/// </summary>
		private string GetDrawFunction(ChartReport chart, string idChart)
		{
			string drawFunction = "";

				// Obtiene la función de dibujo
				switch (chart.IDType)
				{
					case ChartReport.ChartType.Pie:
					case ChartReport.ChartType.Donut:
						drawFunction = "PieChart";
						break;
					case ChartReport.ChartType.HorizontalBar:
						drawFunction = "BarChart";
						break;
					case ChartReport.ChartType.ColumnBar:
						drawFunction = "ColumnChart";
						break;
					case ChartReport.ChartType.Area:
						drawFunction = "AreaChart";
						break;
					case ChartReport.ChartType.Line:
					case ChartReport.ChartType.Spline:
						drawFunction = "LineChart";
						break;
					default:
						drawFunction = "PieChart";
						break;
				}
				// Calcula la función
				return $"var chart = new google.visualization.{drawFunction}(document.getElementById('{idChart}'))";
		}

		/// <summary>
		///		Obtiene los datos del gráfico
		/// </summary>
		private string GetData(ChartReport chart)
		{
			switch (chart.IDType)
			{
				case ChartReport.ChartType.Pie:
				case ChartReport.ChartType.Donut:
					return GetDataPie(chart);
				case ChartReport.ChartType.HorizontalBar:
				case ChartReport.ChartType.ColumnBar:
				case ChartReport.ChartType.Area:
				case ChartReport.ChartType.Line:
				case ChartReport.ChartType.Spline:
					return GetDataBar(chart);
				default:
					return "";
			}
		}

		/// <summary>
		///		Obtiene los datos para un gráfico de barras
		/// </summary>
		private string GetDataPie(ChartReport chart)
		{
			string data = "var data = new google.visualization.DataTable();";
			bool first = true;

				// Añade los datos
				data += "data.addColumn('string', 'Series');";
				data += "data.addColumn('number', 'Tartas');";
				data += "data.addRows([";
				// Añade los datos de la primera serie
				foreach (ChartSerieReport serie in chart.Series)
					if (first)
					{
						string strDataInner = "";

						// Añade los datos a la cadena intermedia
						foreach (ChartSerieColumnReport column in serie.Columns)
							if (column.Data.Count > 0)
								strDataInner = strDataInner.AddWithSeparator("[" + GetData(column.Title, column.Data[0].Value) + "]", ",");
						//foreach (ChartSerieColumnReport data in serie.Data)
						//	strDataInner = strDataInner.AddWithSeparator("[" + GetData(data) + "]", ",");
						// Añade los datos a la cadena de datos
						data += strDataInner;
						// Indica que ya no es la primera serie
						first = false;
					}
				data += "]);";
				// Devuelve la cadena con los datos
				return data;
		}

		/// <summary>
		///		Obtiene los datos para una barra
		/// </summary>
		/// <remarks>
		/// El formato de DataTable para el gráfico es el siguiente:
		///		['City', '2010 Population', '2000 Population'],
		///		['New York City, NY', 8175000, 8008000],
		///		['Los Angeles, CA', 3792000, 3694000],
		///		['Chicago, IL', 2695000, 2896000],
		///		['Houston, TX', 2099000, 1953000],
		///		['Philadelphia, PA', 1526000, 1517000]
		/// </remarks>
		private string GetDataBar(ChartReport chart)
		{
			string data = "var data = google.visualization.arrayToDataTable([";
			bool first = true;

				// Añade los datos de la primera serie
				foreach (ChartSerieReport serie in chart.Series)
					if (first && serie.Columns.Count > 0)
					{
						string headers = "'" + NormalizeJavaScript(serie.Title) + "'";

							// Añade los nombres de las columnas
							foreach (ChartSerieDataDefinition definition in serie.DataDefinitions)
								headers = headers.AddWithSeparator($"'{NormalizeJavaScript(definition.Title)}'", ",");
							// Añade la cabecera a los datos
							data += $"[{headers}]";
							// Añade los datos
							foreach (ChartSerieColumnReport column in serie.Columns)
							{
								string dataInner = $"'{NormalizeJavaScript(column.Title)}'";

									// Añade los datos de la columna
									foreach (ChartSerieColumnDataReport columnData in column.Data)
										dataInner = dataInner.AddWithSeparator(NormalizeJavaScript(columnData.Value), ",");
									// Añade los datos intermedios
									data = data.AddWithSeparator($"[{dataInner}]", ",");
							}
							// Indica que se han añadido ya los datos
							first = false;
					}
				// Añade el cierre de los datos
				data += "]);";
				// Devuelve los datos
				return data;
		}

		/// <summary>
		///		Obtiene la cadena de un dato
		/// </summary>
		private string GetData(string title, double value)
		{
			return string.Format("'{0}', {1}", NormalizeJavaScript(title), Math.Round(value, 2).ToString(System.Globalization.CultureInfo.InvariantCulture));
		}

		/// <summary>
		///		Codifica una cadena para JavaScript
		/// </summary>
		private string NormalizeJavaScript(string value)
		{ 
			// Normaliza la cadena
			if (!value.IsEmpty())
				value = value.Replace("\'", "\\'");
			// Devuelve la cadena
			return value;
		}

		/// <summary>
		///		Normaliza un numérico para JavaScript
		/// </summary>
		private string NormalizeJavaScript(double dblValue)
		{
			return string.Format("{0:#0.00}", dblValue).Replace(',', '.');
		}

		/// <summary>
		///		Obtiene la cadena de opciones del gráfico
		/// </summary>
		private string GetOptions(ChartReport chart)
		{
			string options = "";

				// Crea la cadena de opciones
				options = options.AddWithSeparator(GetOption("title", chart.Title), ",");
				if (chart.Width != 0)
					options = options.AddWithSeparator(GetOption("width", chart.Width), ",");
				if (chart.Height != 0)
					options = options.AddWithSeparator(GetOption("height", chart.Height), ",");
				options = options.AddWithSeparator(GetOption("is3D", chart.Is3D), ",");
				if (chart.IsStacked)
					options = options.AddWithSeparator(GetOption("isStacked", chart.IsStacked), ",");
				options = options.AddWithSeparator(GetLegend(chart.WithLegend), ",");
				// Añade las opciones que dependen del tipo de gráfico
				switch (chart.IDType)
				{
					case ChartReport.ChartType.Donut:
							options = options.AddWithSeparator(GetOption("pieHole", "0.4"), ",");
						break;
					case ChartReport.ChartType.Spline:
							options = options.AddWithSeparator(GetOption("curveType", "function"), ",");
						break;
				}
				// Devuelve la cadena de opciones
				return $"{options}";
		}

		/// <summary>
		///		Obtiene la leyenda
		/// </summary>
		private string GetLegend(bool blnWithLegend)
		{
			if (blnWithLegend)
				return "'position': 'right'";
			else
				return "'position': 'none'";
		}

		/// <summary>
		///		Obtiene una opción formateada
		/// </summary>
		private string GetOption(string tag, string value)
		{
			if (!value.IsEmpty())
				return $"'{tag}': '{value}'";
			else
				return "";
		}

		/// <summary>
		///		Obtiene una opción formateada
		/// </summary>
		private string GetOption(string tag, int value)
		{
			return $"'{tag}': {value}";
		}

		/// <summary>
		///		Obtiene una opción formateada
		/// </summary>
		private string GetOption(string tag, bool value)
		{
			if (value)
				return $"'{tag}': true";
			else
				return $"'{tag}': false";
		}

		/// <summary>
		///		Añade una línea de texto
		/// </summary>
		private void Append(string html)
		{
			_builder.AppendLine(html);
		}

		/// <summary>
		///		Indica si se ha añadido el script de tratamiento de gráficos
		/// </summary>
		private bool IsAddChartScript { get; set; }

		/// <summary>
		///		Indice de gráfico añadido
		/// </summary>
		private int IndexChart { get; set; }
	}
}
