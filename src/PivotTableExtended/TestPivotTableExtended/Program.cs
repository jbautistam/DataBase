using System;
using System.Data;

using Bau.Libraries.PivotTableExtended;
using Bau.Libraries.PivotTableExtended.Results;

namespace TestPivotTableExtended
{
	class Program
	{
		static void Main(string[] args)
		{ DataTable objDataTable = FillDataTable();
			PivotTable objPivotTable = new PivotTable();
			CompiledPivotTable objCompiledTable;
			RawPivotTable objResult;

				// Añade las filas
					objPivotTable.AddGroupRow("País", "Country", true, 0);
					objPivotTable.AddGroupRow("Ciudad", "City", true, 0);
				// Añade las columnas
					objPivotTable.AddGroupColumn("Categoría", "Category", true, 0);
					objPivotTable.AddGroupColumn("Producto", "Product", true, 0);
				// Añade las celdas
					objPivotTable.AddGroupCell("Precio", "Price", Bau.Libraries.PivotTableExtended.Definitions.GroupModel.ComputeType.Sum);
					objPivotTable.AddGroupCell("Precio venta", "Sell", Bau.Libraries.PivotTableExtended.Definitions.GroupModel.ComputeType.Sum);
					objPivotTable.AddGroupCell("Beneficio", "Profit", Bau.Libraries.PivotTableExtended.Definitions.GroupModel.ComputeType.Sum);
				// Genera la tabla pivotal
					objResult = objPivotTable.Compute(objDataTable);
				// Muestra la información
					System.Diagnostics.Debug.WriteLine(objResult.GetDebugStructure());
				// Obtiene una tabla compilada
					objCompiledTable = objResult.GetCompiledTable();
				// Muestra la información
					System.Diagnostics.Debug.WriteLine(objCompiledTable.GetDebugInfo());
				// Graba el HTML
					Bau.Libraries.LibHelper.Files.HelperFiles.SaveTextFile(@"C:\Users\jbautistam\Downloads\Test.htm", 
																																 "<html><head></head><body>" + objCompiledTable.GetHtml() + "</body></html>");
		}

		/// <summary>
		///		Crea una tabla de datos
		/// </summary>
		private static DataTable FillDataTable()
		{ DataTable objDataTable = new DataTable();

				// Añade las columnas
					objDataTable.Columns.Add("Country", typeof(string));
					objDataTable.Columns.Add("City", typeof(string));
					objDataTable.Columns.Add("Category", typeof(string));
					objDataTable.Columns.Add("Product", typeof(string));
					objDataTable.Columns.Add("Price", typeof(double));
					objDataTable.Columns.Add("Sell", typeof(double));
					objDataTable.Columns.Add("Profit", typeof(double));
				// Añade las filas
					CreateRow(objDataTable, "Spain", 
																new string [] { "Madrid", "Barcelona" }, 
																new string [] { "Primera", "Segunda" },
																new string [] { "Pan", "Azúcar", "Vino" }
																);
					CreateRow(objDataTable, "UK", 
																new string [] { "London", "Bristol", "Birmingham" }, 
																new string [] { "Primera", "Segunda", "Tercera" },
																new string [] { "Pan", "Azúcar" }
																);
					CreateRow(objDataTable, "France", 
																new string [] { "Paris", "Lyon"}, 
																new string [] { "Primera", "Tercera" },
																new string [] { "Pan" }
																);
					CreateRow(objDataTable, "Italy", 
																new string [] { "Roma", "Venecia", "Milán", "Nápoles", "Genova" }, 
																new string [] { "Primera", "Segunda", "Tercera", "Cuarta" },
																new string [] { "Pan", "Azúcar", "Aceite" }
																);
					CreateRow(objDataTable, "Germany", 
																new string [] { "Berlin", "Bon" }, 
																new string [] { "Segunda", "Cuarta" },
																new string [] { "Pan", "Azúcar", "Aceite", "Vino" }
																);
					CreateRow(objDataTable, "Spain", 
																new string [] { "Madrid", "Barcelona" }, 
																new string [] { "Primera", "Segunda" },
																new string [] { "Azúcar" }
																);
				// Devuelve la tabla
					return objDataTable;
		}

		/// <summary>
		///		Crea una fila de prueba
		/// </summary>
		private static void CreateRow(DataTable objDataTable, string strCountry, string [] arrStrCities, string [] arrStrCategories, string [] arrStrProducts)
		{ foreach (string strCity in arrStrCities)
				foreach (string strCategory in arrStrCategories)
					foreach (string strProducts in arrStrProducts)
						objDataTable.Rows.Add(strCountry, strCity, strCategory, strProducts, 1, 2, 3);
		}
	}
}
