using System;

using Bau.Libraries.LibCommonHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibReports.Renderer.Models.Contents;

namespace Bau.Libraries.LibReports.Renderer.Parser.Tools
{
	/// <summary>
	///		Intérprete de <see cref="ChartReport"/>
	/// </summary>
	internal class ChartParser : ParserBase<ChartReport>
	{
		internal ChartParser(ReportParser reportParser) : base(reportParser) { }

		//<Chart Type = "Bar">
		//	<Title>Título del gráfico</Title>
		//	<Subtitle>Subtítulo del gráfico</Subtitle>
		//	<Serie Type = "Bar" FieldName = "ProductName" FieldValue = "UnitPrice">
		//		<DataTableProvider Key ="NorthWind">
		//			<Sentence>
		//				SELECT [Order Details].UnitPrice, [Order Details].Quantity, [Order Details].Discount, 
		//					   Products.ProductName
		//				FROM [Order Details] INNER JOIN Products 
		//					ON [Order Details].ProductID = Products.ProductID
		//				WHERE [Order Details].OrderID = @OrderID
		//			</Sentence>
		//			<Parameter Type="Record" ParameterName="@OrderID" FieldName = "OrderID" />
		//		</DataTableProvider>
		//		<Title>Título de la serie 1</Title>
		//		<Subtitle>Subtítulo de la serie 1</Subtitle>
		//	</Serie>
		//	<Serie Type = "Bar" FieldName = "ProductName" FieldValue = "Quantity">
		//		<DataTableProvider Key ="NorthWind">
		//			<Sentence>
		//				SELECT [Order Details].UnitPrice, [Order Details].Quantity, [Order Details].Discount, 
		//					   Products.ProductName
		//				FROM [Order Details] INNER JOIN Products 
		//					ON [Order Details].ProductID = Products.ProductID
		//				WHERE [Order Details].OrderID = @OrderID
		//			</Sentence>
		//			<Parameter Type="Record" ParameterName="@OrderID" FieldName = "OrderID" />
		//		</DataTableProvider>
		//		<Title>Título de la serie 2</Title>
		//		<Subtitle>Subtítulo de la serie 2</Subtitle>
		//		<Code>
		//				Código asociado a la serie
		//		</Code>
		//	</Serie>
		//</Chart>


		//<Chart Type = "Bar" Is3D = "true" WithLegend = "true" Width = "800" Height = "600">
		//	<Title>Título del gráfico 2</Title>
		//	<Serie Type = "Bar" FieldColumnTitle = "ProductName">
		//		<DataTableProvider Key ="NorthWind">
		//			<Sentence>
		//				SELECT Products.ProductName, [Order Details].Quantity,
		//					   [Order Details].UnitPrice * [Order Details].Quantity - [Order Details].Discount AS Price
		//				FROM Orders INNER JOIN [Order Details]
		//					ON Orders.OrderID = [Order Details].OrderID
		//						AND Orders.CustomerID = @CustomerID
		//				INNER JOIN Products 
		//					ON [Order Details].ProductID = Products.ProductID
		//			</Sentence>
		//			<Parameter Type="Record" ParameterName="@CustomerID" FieldName = "CustomerID" />
		//		</DataTableProvider>
		//		<Title>Título de la serie 2</Title>
		//		<Subtitle>Subtítulo de la serie 2</Subtitle>
		//		<DataDefinition FieldData = "Quantity">
		//			<Title>Cantidad</Title>
		//		</DataDefinition>
		//		<DataDefinition FieldData = "Price">
		//			<Title>Importe</Title>
		//		</DataDefinition>
		//	</Serie>
		//</Chart>

		/// <summary>
		///		Interpreta el contenido del nodo
		/// </summary>
		protected override ChartReport ParseInner(MLNode nodeML)
		{
			ChartReport chart = new ChartReport(Parent);

				// Asigna el tipo
				chart.IDType = ConvertType(nodeML.Attributes["Type"].Value);
				// Asigna las propiedades
				chart.Title = nodeML.Nodes["Title"].Value;
				chart.SubTitle = nodeML.Nodes["Subtitle"].Value;
				chart.Is3D = nodeML.Attributes["Is3D"].Value.GetBool();
				chart.IsStacked = nodeML.Attributes["IsStacked"].Value.GetBool();
				chart.WithLegend = nodeML.Attributes["WithLegend"].Value.GetBool();
				chart.Width = nodeML.Attributes["Width"].Value.GetInt(0);
				chart.Height = nodeML.Attributes["Height"].Value.GetInt(0);
				// Carga las series
				foreach (MLNode childML in nodeML.Nodes)
					if (childML.Name == "Serie")
						chart.Series.Add(ParseSerie(chart, childML));
				// Devuelve el informe
				return chart;
		}

		/// <summary>
		///		Obtiene el tipo de gráfico
		/// </summary>
		private ChartReport.ChartType ConvertType(string type)
		{
			return type.GetEnum(ChartReport.ChartType.ColumnBar);
		}

		/// <summary>
		///		Interpreta los datos de una serie
		/// </summary>
		private ChartSerieReport ParseSerie(ChartReport chart, MLNode nodeML)
		{
			ChartSerieReport serie = new ChartSerieReport(chart);

				// Recoge los datos básicos de la serie
				serie.Title = nodeML.Nodes["Title"].Value;
				serie.SubTitle = nodeML.Nodes["Subtitle"].Value;
				serie.IDType = ConvertType(nodeML.Attributes["Type"].Value);
				serie.FieldColumnTitle = nodeML.Attributes["FieldColumnTitle"].Value;
				// Recoge los datos de la serie
				foreach (MLNode childML in nodeML.Nodes)
					switch (childML.Name)
					{
						//case "DataTableProvider":
						//		serie.Command = ReportParser.ParserLocator.ParseDataComand(serie, childML);
						//	break;
						case "Code":
								serie.Code = ReportParser.ParserLocator.Parse<CodeReport>(serie, childML);
							break;
						case "DataDefinition":
								serie.DataDefinitions.Add(ParseDataDefinition(childML));
							break;
					}
				// Devuelve la serie
				return serie;
		}

		/// <summary>
		///		Interpreta la definición de datos
		/// </summary>
		private ChartSerieDataDefinition ParseDataDefinition(MLNode nodeML)
		{
			ChartSerieDataDefinition definition = new ChartSerieDataDefinition();

				// Asigna las propiedades
				definition.FieldTitle = nodeML.Attributes["FieldTitle"].Value;
				definition.FieldValue = nodeML.Attributes["FieldValue"].Value;
				definition.FieldMaximum = nodeML.Attributes["FieldMaximum"].Value;
				definition.FieldMinimum = nodeML.Attributes["FieldMinimum"].Value;
				definition.Title = nodeML.Nodes["Title"].Value;
				// Devuelve la definición
				return definition;
		}
	}
}
