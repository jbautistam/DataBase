using System;

namespace Bau.Libraries.LibSqLiteProvider.Builders
{
	/// <summary>
	///		Generador de base de datos
	/// </summary>
	public class SqLiteBuilder
	{
		public SqLiteBuilder(string fileName)
		{
			FileName = fileName;
		}

		/// <summary>
		///		Crea un archivo de base de datos con las tablas especificadas
		/// </summary>
		public void Create()
		{	
			// Genera el archivo
			if (!System.IO.File.Exists(FileName))
				System.Data.SQLite.SQLiteConnection.CreateFile(FileName);
			// Crea las tablas
			using (SqLiteProvider dbProvider = new SqLiteProvider(new SqLiteConnectionString(FileName)))
			{
				// Abre la conexión
				dbProvider.Open();
				// Crea las tablas
				foreach (SqLiteTableBuilder builder in TableBuilders)
				{
					string sql = builder.GetSqlCreate();

						if (!string.IsNullOrEmpty(sql))
							dbProvider.Execute(sql, null, System.Data.CommandType.Text);
				}
				// Cierra la conexión
				dbProvider.Close();
			}
		}

		/// <summary>
		///		Genera una tabla
		/// </summary>
		public SqLiteTableBuilder WithTable(string table)
		{
			// Añade el generador
			TableBuilders.Add(new SqLiteTableBuilder(this, table));
			// Devuelve el último generador
			return TableBuilders[TableBuilders.Count - 1];
		}

		/// <summary>
		///		Nombre de archivo
		/// </summary>
		public string FileName { get; }

		/// <summary>
		///		Generadores de tablas
		/// </summary>
		private System.Collections.Generic.List<SqLiteTableBuilder> TableBuilders { get; }= new System.Collections.Generic.List<SqLiteTableBuilder>();
	}
}
