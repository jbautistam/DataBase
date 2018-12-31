using System;
using System.Data;

using Bau.Libraries.LibDBProvidersBase;
using Bau.Libraries.LibDBProvidersBase.Parameters;
using Bau.Libraries.LibDBProvidersBase.Providers.SQLServer;
using Bau.Libraries.LibDBSchema;
using Bau.Libraries.LibDBSchema.DataSchema;

namespace Bau.Libraries.LibDBSchemaProvider.Providers
{
	/// <summary>
	///		Proveedor de esquema para SQL Server
	/// </summary>
	public class SchemaSqlServerProvider : Interfaces.ISchemaProvider<SQLServerConnectionString>
	{
		/// <summary>
		///		Clase para la carga de un esquema de una base de datos SQL Server
		/// </summary>
		public Schema LoadSchema(SQLServerConnectionString connectionString)
		{
			Schema schema = new Schema();

				// Carga los datos del esquema
				LoadSchemaTables(schema, connectionString);
				LoadSchemaTriggers(schema, connectionString);
				LoadSchemaViews(schema, connectionString);
				LoadSchemaRoutines(schema, connectionString);
				// Devuelve el esquema
				return schema;
		}

		/// <summary>
		///		Carga las tablas del esquema de una base de datos
		/// </summary>
		public void LoadSchemaTables(Schema schema, SQLServerConnectionString connectionString)
		{
			using (SQLServerProvider connection = new SQLServerProvider(connectionString))
			{ 
				// Abre la conexión
				connection.Open();
				// Limpia las tablas
				schema.Tables.Clear();
				// Obtiene los datos del esquema
				LoadTables(connection, schema);
				// Cierra la conexión
				connection.Close();
			}
		}

		/// <summary>
		///		Carga los desencadenadores del esquema de una base de datos
		/// </summary>
		public void LoadSchemaTriggers(Schema schema, SQLServerConnectionString connectionString)
		{
			using (SQLServerProvider connection = new SQLServerProvider(connectionString))
			{ 
				// Abre la conexión
				connection.Open();
				// Limpia los triggers
				schema.Triggers.Clear();
				// Obtiene los datos del esquema
				LoadTriggers(connection, schema);
				// Cierra la conexión
				connection.Close();
			}
		}

		/// <summary>
		///		Carga las vistas del esquema de una base de datos
		/// </summary>
		public void LoadSchemaViews(Schema schema, SQLServerConnectionString connectionString)
		{
			using (SQLServerProvider connection = new SQLServerProvider(connectionString))
			{ 
				// Abre la conexión
				connection.Open();
				// Limpia las vistas
				schema.Views.Clear();
				// Obtiene los datos del esquema
				LoadViews(connection, schema);
				// Cierra la conexión
				connection.Close();
			}
		}

		/// <summary>
		///		Carga las rutinas del esquema de una base de datos
		/// </summary>
		public void LoadSchemaRoutines(Schema schema, SQLServerConnectionString connectionString)
		{
			using (SQLServerProvider connection = new SQLServerProvider(connectionString))
			{ 
				// Abre la conexión
				connection.Open();
				// Limpia las rutinas
				schema.Routines.Clear();
				// Obtiene los datos del esquema
				LoadRoutines(connection, schema);
				// Cierra la conexión
				connection.Close();
			}
		}

		/// <summary>
		///		Carga las tablas de un esquema
		/// </summary>
		private void LoadTables(SQLServerProvider connection, Schema schema)
		{
			string sql;

				// Crea la cadena SQL
				sql = @"SELECT Tables.TABLE_CATALOG, Tables.TABLE_SCHEMA, Tables.TABLE_NAME,
							   Tables.TABLE_TYPE, Objects.Create_Date, Objects.Modify_Date, Properties.Value AS Description
						  FROM INFORMATION_SCHEMA.TABLES AS Tables INNER JOIN sys.all_objects AS Objects
								ON Tables.Table_Name = Objects.name
							LEFT JOIN sys.extended_properties AS Properties
								ON Objects.object_id = Properties.major_id
									AND Properties.minor_id = 0
									AND Properties.name = 'MS_Description'
						 ORDER BY Tables.TABLE_NAME";
				// Carga las tablas
				using (IDataReader tables = connection.ExecuteReader(sql, null, CommandType.Text))
				{ 
					// Recorre la colección de registros
					while (tables.Read())
					{
						SchemaTable table = new SchemaTable(schema);

							// Asigna los datos del registro al objeto
							table.Catalog = (string) tables.IisNull("TABLE_CATALOG");
							table.Schema = (string) tables.IisNull("TABLE_SCHEMA");
							table.Name = (string) tables.IisNull("TABLE_NAME");
							table.Type = GetTableType((string) tables.IisNull("TABLE_TYPE"));
							table.DateCreated = (DateTime) tables.IisNull("Create_Date");
							table.DateUpdated = (DateTime) tables.IisNull("Modify_Date");
							table.Description = (string) tables.IisNull("Description");
							// Añade el objeto a la colección (si es una tabla)
							if (table.Type == SchemaTable.TableType.Table)
								schema.Tables.Add(table);
					}
					// Cierra el recordset
					tables.Close();
				}
				// Carga los datos de las tablas
				foreach (SchemaTable table in schema.Tables)
				{
					LoadColumns(connection, table);
					LoadConstraints(connection, table);
				}
		}

		/// <summary>
		///		Obtiene el tipo de una tabla a partir de su descripción
		/// </summary>
		private SchemaTable.TableType GetTableType(string tableType)
		{
			if (tableType.Equals("BASE TABLE", StringComparison.CurrentCultureIgnoreCase))
				return SchemaTable.TableType.Table;
			else if (tableType.Equals("VIEW", StringComparison.CurrentCultureIgnoreCase))
				return SchemaTable.TableType.View;
			else
				return SchemaTable.TableType.Unknown;
		}

		/// <summary>
		///		Carga los triggers de un esquema
		/// </summary>
		private void LoadTriggers(SQLServerProvider connection, Schema schema)
		{
			string sql;

				// Crea la cadena SQL
				sql = @"SELECT tmpTables.name AS DS_Tabla, tmpTrigger.name AS DS_Trigger_Name,
							   USER_NAME(tmpTrigger.uid) AS DS_User_Name, tmpTrigger.category AS NU_Category,
							   CONVERT(bit, (CASE WHEN (OBJECTPROPERTY(tmpTrigger.id, N'IsExecuted') = 1) THEN 1 ELSE 0 END)) AS IsExecuted,
							   CONVERT(bit, (CASE WHEN (OBJECTPROPERTY(tmpTrigger.id, N'ExecIsAnsiNullsOn') = 1) THEN 1 ELSE 0 END)) AS ExecIsAnsiNullsOn,
							   CONVERT(bit, (CASE WHEN (OBJECTPROPERTY(tmpTrigger.id, N'ExecIsQuotedIdentOn') = 1) THEN 1 ELSE 0 END)) AS ExecIsQuotedIdentOn,
							   CONVERT(bit, (CASE WHEN (OBJECTPROPERTY(tmpTrigger.id, N'IsAnsiNullsOn') = 1) THEN 1 ELSE 0 END)) AS IsAnsiNullsOn,
							   CONVERT(bit, (CASE WHEN (OBJECTPROPERTY(tmpTrigger.id, N'IsQuotedIdentOn') = 1) THEN 1 ELSE 0 END)) AS IsQuotedIdentOn,
							   CONVERT(bit, (CASE WHEN (OBJECTPROPERTY(tmpTrigger.id, N'ExecIsAfterTrigger') = 1) THEN 1 ELSE 0 END)) AS ExecIsAfterTrigger,
							   CONVERT(bit, (CASE WHEN (OBJECTPROPERTY(tmpTrigger.id, N'ExecIsDeleteTrigger') = 1) THEN 1 ELSE 0 END)) AS ExecIsDeleteTrigger,
							   CONVERT(bit, (CASE WHEN (OBJECTPROPERTY(tmpTrigger.id, N'ExecIsFirstDeleteTrigger') = 1) THEN 1 ELSE 0 END)) AS ExecIsFirstDeleteTrigger,
							   CONVERT(bit, (CASE WHEN (OBJECTPROPERTY(tmpTrigger.id, N'ExecIsFirstInsertTrigger') = 1) THEN 1 ELSE 0 END)) AS ExecIsFirstInsertTrigger,
							   CONVERT(bit, (CASE WHEN (OBJECTPROPERTY(tmpTrigger.id, N'ExecIsFirstUpdateTrigger') = 1) THEN 1 ELSE 0 END)) AS ExecIsFirstUpdateTrigger,
							   CONVERT(bit, (CASE WHEN (OBJECTPROPERTY(tmpTrigger.id, N'ExecIsInsertTrigger') = 1) THEN 1 ELSE 0 END)) AS ExecIsInsertTrigger,
							   CONVERT(bit, (CASE WHEN (OBJECTPROPERTY(tmpTrigger.id, N'ExecIsInsteadOfTrigger') = 1) THEN 1 ELSE 0 END)) AS ExecIsInsteadOfTrigger,
							   CONVERT(bit, (CASE WHEN (OBJECTPROPERTY(tmpTrigger.id, N'ExecIsLastDeleteTrigger') = 1) THEN 1 ELSE 0 END)) AS ExecIsLastDeleteTrigger,
							   CONVERT(bit, (CASE WHEN (OBJECTPROPERTY(tmpTrigger.id, N'ExecIsLastInsertTrigger') = 1) THEN 1 ELSE 0 END)) AS ExecIsLastInsertTrigger,
							   CONVERT(bit, (CASE WHEN (OBJECTPROPERTY(tmpTrigger.id, N'ExecIsLastUpdateTrigger') = 1) THEN 1 ELSE 0 END)) AS ExecIsLastUpdateTrigger,
							   CONVERT(bit, (CASE WHEN (OBJECTPROPERTY(tmpTrigger.id, N'ExecIsTriggerDisabled') = 1) THEN 1 ELSE 0 END)) AS ExecIsTriggerDisabled,
							   CONVERT(bit, (CASE WHEN (OBJECTPROPERTY(tmpTrigger.id, N'ExecIsUpdateTrigger') = 1) THEN 1 ELSE 0 END)) AS ExecIsUpdateTrigger,
							   tmpTrigger.crdate AS FE_Create, tmpTrigger.refdate AS FE_Reference
						  FROM sys.sysobjects AS tmpTrigger INNER JOIN sys.sysobjects AS tmpTables
							ON tmpTrigger.parent_obj = tmpTables.id
						  WHERE OBJECTPROPERTY(tmpTrigger.id, N'IsTrigger') = 1
							AND OBJECTPROPERTY(tmpTrigger.id, N'IsMSShipped') = 0
						  ORDER BY tmpTables.Name, tmpTrigger.Name";
				// Carga los desencadenadores
				using (IDataReader triggers = connection.ExecuteReader(sql, null, CommandType.Text))
				{ 
					// Recorre la colección de registros
					while (triggers.Read())
					{
						SchemaTrigger trigger = new SchemaTrigger(schema);

							// Asigna los datos del registro al objeto
							trigger.Catalog = "TABLE_CATALOG"; // clsBaseDB.iisNull(rdoTables, "TABLE_CATALOG") as string;
							trigger.Schema = "TABLE_SCHEMA"; // clsBaseDB.iisNull(rdoTables, "TABLE_SCHEMA") as string;
							trigger.Table = (string) triggers.IisNull("DS_Tabla");
							trigger.Name = (string) triggers.IisNull("DS_Trigger_Name");
							trigger.UserName = (string) triggers.IisNull("DS_User_Name");
							trigger.Category = (int) triggers.IisNull("NU_Category");
							trigger.IsExecuted = (bool) triggers.IisNull("IsExecuted");
							trigger.IsExecutionAnsiNullsOn = (bool) triggers.IisNull("ExecIsAnsiNullsOn");
							trigger.IsExecutionQuotedIdentOn = (bool) triggers.IisNull("ExecIsQuotedIdentOn");
							trigger.IsAnsiNullsOn = (bool) triggers.IisNull("IsAnsiNullsOn");
							trigger.IsQuotedIdentOn = (bool) triggers.IisNull("IsQuotedIdentOn");
							trigger.IsExecutionAfterTrigger = (bool) triggers.IisNull("ExecIsAfterTrigger");
							trigger.IsExecutionDeleteTrigger = (bool) triggers.IisNull("ExecIsDeleteTrigger");
							trigger.IsExecutionFirstDeleteTrigger = (bool) triggers.IisNull("ExecIsFirstDeleteTrigger");
							trigger.IsExecutionFirstInsertTrigger = (bool) triggers.IisNull("ExecIsFirstInsertTrigger");
							trigger.IsExecutionFirstUpdateTrigger = (bool) triggers.IisNull("ExecIsFirstUpdateTrigger");
							trigger.IsExecutionInsertTrigger = (bool) triggers.IisNull("ExecIsInsertTrigger");
							trigger.IsExecutionInsteadOfTrigger = (bool) triggers.IisNull("ExecIsInsteadOfTrigger");
							trigger.IsExecutionLastDeleteTrigger = (bool) triggers.IisNull("ExecIsLastDeleteTrigger");
							trigger.IsExecutionLastInsertTrigger = (bool) triggers.IisNull("ExecIsLastInsertTrigger");
							trigger.IsExecutionLastUpdateTrigger = (bool) triggers.IisNull("ExecIsLastUpdateTrigger");
							trigger.IsExecutionTriggerDisabled = (bool) triggers.IisNull("ExecIsTriggerDisabled");
							trigger.IsExecutionUpdateTrigger = (bool) triggers.IisNull("ExecIsUpdateTrigger");
							trigger.DateCreated = ((DateTime?) triggers.IisNull("FE_Create")) ?? DateTime.Now;
							trigger.DateReference = (DateTime?) triggers.IisNull("FE_Reference");
							// Añade el objeto a la colección (si es una tabla)
							schema.Triggers.Add(trigger);
					}
					// Cierra el recordset
					triggers.Close();
				}
				// Carga el contenido de los triggers
				foreach (SchemaTrigger trigger in schema.Triggers)
					trigger.Content = LoadHelpText(connection, trigger.Name);
		}

		/// <summary>
		///		Carga el texto de una función, procedimiento, trigger ...
		/// </summary>
		private string LoadHelpText(SQLServerProvider connection, string name)
		{
			string text = "";

				// Obtiene el texto resultante de llamar a la rutina sp_helptext
				try
				{
					using (IDataReader textReader = connection.ExecuteReader($"EXEC sp_helptext '{name}'", null, CommandType.Text))
					{ 
						// Obtiene el texto
						while (textReader.Read())
							text += (string) textReader.IisNull("Text");
						// Cierra el recordset
						textReader.Close();
					}
				}
				catch { }
				// Devuelve el texto cargado
				return text;
		}

		/// <summary>
		///		Carga las rutinas de la base de datos
		/// </summary>
		private void LoadRoutines(SQLServerProvider connection, Schema schema)
		{
			string sql;

				// Crea la cadena SQL
				sql = @"SELECT Routine_Catalog AS Table_Catalog, Routine_Schema AS Table_Schema,
							   Routine_Name AS Table_Name, Routine_Type, Routine_Definition
						  FROM Information_Schema.Routines
						  ORDER BY Routine_Name";
				// Carga los datos
				using (IDataReader routinesReader = connection.ExecuteReader(sql, null, CommandType.Text))
				{ 
					// Lee los registros
					while (routinesReader.Read())
					{
						SchemaRoutine routine = new SchemaRoutine(schema);

							// Asigna los datos del recordset al objeto
							routine.Catalog = (string) routinesReader.IisNull("Table_Catalog");
							routine.Schema = (string) routinesReader.IisNull("Table_Schema");
							routine.Name = (string) routinesReader.IisNull("Table_Name");
							routine.Type = GetRoutineType((string) routinesReader.IisNull("Routine_Type"));
							routine.Definition = (string) routinesReader.IisNull("Routine_Definition");
							// Añade el objeto a la colección
							schema.Routines.Add(routine);
					}
					// Cierra el recordset
					routinesReader.Close();
				}
		}

		/// <summary>
		///		Obtiene el tipo de rutina
		/// </summary>
		private SchemaRoutine.RoutineType GetRoutineType(string type)
		{
			if (type.Equals("PROCEDURE", StringComparison.CurrentCultureIgnoreCase))
				return SchemaRoutine.RoutineType.Procedure;
			else if (type.Equals("FUNCTION", StringComparison.CurrentCultureIgnoreCase))
				return SchemaRoutine.RoutineType.Function;
			else
				return SchemaRoutine.RoutineType.Unknown;
		}

		/// <summary>
		///		Carga las restricciones de una tabla
		/// </summary>
		private void LoadConstraints(SQLServerProvider connection, SchemaTable table)
		{
			ParametersDBCollection parameters = new ParametersDBCollection();
			string sql;

				// Añade los parámetros
				parameters.Add("@Table_Catalog", table.Catalog);
				parameters.Add("@Table_Schema", table.Schema);
				parameters.Add("@Table_Name", table.Name);
				// Crea la cadena SQL
				sql = @"SELECT TableConstraints.Table_Catalog, TableConstraints.Table_Schema, TableConstraints.Table_Name,
							   ColumnConstraint.Column_Name, ColumnConstraint.Constraint_Name,
							   TableConstraints.Constraint_Type, Key_Column.Ordinal_Position
						  FROM Information_Schema.Table_Constraints AS TableConstraints
								INNER JOIN Information_Schema.Constraint_Column_Usage AS ColumnConstraint
									ON TableConstraints.Constraint_Catalog = ColumnConstraint.Constraint_Catalog
										AND TableConstraints.Constraint_Schema = ColumnConstraint.Constraint_Schema
										AND TableConstraints.Constraint_Name = ColumnConstraint.Constraint_Name
								INNER JOIN Information_Schema.Key_Column_Usage AS Key_Column
									ON ColumnConstraint.Constraint_Catalog = Key_Column.Constraint_Catalog
										AND ColumnConstraint.Constraint_Schema = Key_Column.Constraint_Schema
										AND ColumnConstraint.Constraint_Name = Key_Column.Constraint_Name
										AND ColumnConstraint.Column_Name = Key_Column.Column_Name
						  WHERE TableConstraints.Table_Catalog = @Table_Catalog
								AND TableConstraints.Table_Schema = @Table_Schema
								AND TableConstraints.Table_Name = @Table_Name
						  ORDER BY TableConstraints.Table_Name, TableConstraints.Constraint_Type, Key_Column.Ordinal_Position";
				// Carga los datos
				using (IDataReader constraintReader = connection.ExecuteReader(sql, parameters, CommandType.Text))
				{ 
					// Lee los datos
					while (constraintReader.Read())
					{
						SchemaConstraint constraint = new SchemaConstraint(table.Parent);

							// Asigna los datos del registro
							constraint.Catalog = (string) constraintReader.IisNull("Table_Catalog");
							constraint.Schema = (string) constraintReader.IisNull("Table_Schema");
							constraint.Table = (string) constraintReader.IisNull("Table_Name");
							constraint.Column = (string) constraintReader.IisNull("Column_Name");
							constraint.Name = (string) constraintReader.IisNull("Constraint_Name");
							constraint.Type = GetConstratype((string) constraintReader.IisNull("Constraint_Type"));
							constraint.Position = (int) constraintReader.IisNull("Ordinal_Position");
							// Añade la restricción a la colección
							table.Constraints.Add(constraint);
					}
					// Cierra el recordset
					constraintReader.Close();
				}
		}

		/// <summary>
		///		Obtiene el tipo de una restricción a partir de su nombre
		/// </summary>
		private SchemaConstraint.Constratype GetConstratype(string type)
		{
			if (type.Equals("UNIQUE", StringComparison.CurrentCultureIgnoreCase))
				return SchemaConstraint.Constratype.Unique;
			else if (type.Equals("PRIMARY KEY", StringComparison.CurrentCultureIgnoreCase))
				return SchemaConstraint.Constratype.PrimaryKey;
			else if (type.Equals("FOREIGN KEY", StringComparison.CurrentCultureIgnoreCase))
				return SchemaConstraint.Constratype.ForeignKey;
			else
				return SchemaConstraint.Constratype.Unknown;
		}

		/// <summary>
		///		Carga la definición de vistas
		/// </summary>
		private void LoadViews(SQLServerProvider connection, Schema schema)
		{
			string sql;

				// Crea la cadena SQL
				sql = @"SELECT Table_Catalog, Table_Schema, Table_Name, View_Definition, Check_Option, Is_Updatable
						  FROM Information_Schema.Views
						  ORDER BY Table_Name";
				// Carga las vistas
				using (IDataReader viewsReader = connection.ExecuteReader(sql, null, CommandType.Text))
				{ // Lee los registros
					while (viewsReader.Read())
					{
						SchemaView view = new SchemaView(schema);

							// Asigna los datos al objeto
							view.Catalog = (string) viewsReader.IisNull("Table_Catalog");
							view.Schema = (string) viewsReader.IisNull("Table_Schema");
							view.Name = (string) viewsReader.IisNull("Table_Name");
							view.Definition = (string) viewsReader.IisNull("View_Definition");
							view.CheckOption = (string) viewsReader.IisNull("Check_Option");
							view.IsUpdatable = !(((string) viewsReader.IisNull("Is_Updatable")).Equals("NO", StringComparison.CurrentCultureIgnoreCase));
							// Añade el objeto a la colección
							schema.Views.Add(view);
					}
					// Cierra el recordset
					viewsReader.Close();
				}
				// Carga las columnas de la vista
				foreach (SchemaView view in schema.Views)
					LoadColumns(connection, view);
		}

		/// <summary>
		///		Carga las columnas de una tabla
		/// </summary>
		private void LoadColumns(SQLServerProvider connection, SchemaTable table)
		{
			ParametersDBCollection parameters = new ParametersDBCollection();
			string sql;

				// Añade los parámetros
				parameters.Add("@Table_Catalog", table.Catalog);
				parameters.Add("@Table_Schema", table.Schema);
				parameters.Add("@Table_Name", table.Name);
				// Crea la cadena SQL
				sql = @"SELECT Columns.Column_Name, Columns.Ordinal_Position, Columns.Column_Default,
							   Columns.Is_Nullable, Columns.Data_Type, Columns.Character_Maximum_Length,
							   CONVERT(int, Columns.Numeric_Precision) AS Numeric_Precision,
							   CONVERT(int, Columns.Numeric_Precision_Radix) AS Numeric_Precision_Radix,
							   CONVERT(int, Columns.Numeric_Scale) AS Numeric_Scale,
							   CONVERT(int, Columns.DateTime_Precision) AS DateTime_Precision,
							   Columns.Character_Set_Name, Columns.Collation_Catalog, Columns.Collation_Schema, Columns.Collation_Name,
							   Objects.is_identity, Properties.value AS Description
						  FROM Information_Schema.Columns AS Columns INNER JOIN sys.all_objects AS Tables
								ON Columns.Table_Name = Tables.name
							INNER JOIN sys.columns AS Objects
								ON Columns.Column_Name = Objects.name
									AND Tables.object_id = Objects.object_id
							LEFT JOIN sys.extended_properties AS Properties
								ON Objects.object_id = Properties.major_id
									AND Properties.minor_id = Objects.column_id
									AND Properties.name = 'MS_Description'
						  WHERE Columns.Table_Catalog = @Table_Catalog
								AND Columns.Table_Schema = @Table_Schema
								AND Columns.Table_Name = @Table_Name
						  ORDER BY Ordinal_Position";
				// Carga los datos
				using (IDataReader columnsReader = connection.ExecuteReader(sql, parameters, CommandType.Text))
				{ 
					// Lee los datos
					while (columnsReader.Read())
					{
						SchemaColumn column = new SchemaColumn(table.Parent);

							// Asigna los datos del registro
							column.Name = (string) columnsReader.IisNull("Column_Name") as string;
							column.OrdinalPosition = columnsReader.IisNull("Ordinal_Position", 0);
							column.Default = (string) columnsReader.IisNull("Column_Default");
							column.IsNullable = !(((string) columnsReader.IisNull("Is_Nullable")).Equals("no", StringComparison.CurrentCultureIgnoreCase));
							column.DataType = (string) columnsReader.IisNull("Data_Type");
							column.CharacterMaximumLength = columnsReader.IisNull("Character_Maximum_Length", 0);
							column.NumericPrecision = columnsReader.IisNull("Numeric_Precision", 0);
							column.NumericPrecisionRadix = columnsReader.IisNull("Numeric_Precision_Radix", 0);
							column.NumericScale = columnsReader.IisNull("Numeric_Scale", 0);
							column.DateTimePrecision = columnsReader.IisNull("DateTime_Precision", 0);
							column.CharacterSetName = (string) columnsReader.IisNull("Character_Set_Name");
							column.CollationCatalog = (string) columnsReader.IisNull("Collation_Catalog");
							column.CollationSchema = (string) columnsReader.IisNull("Collation_Schema");
							column.CollationName = (string) columnsReader.IisNull("Collation_Name");
							column.IsIdentity = (bool) columnsReader.IisNull("is_identity");
							column.Description = (string) columnsReader.IisNull("Description") as string;
							// Añade la columna a la colección
							table.Columns.Add(column);
					}
					// Cierra el recordset
					columnsReader.Close();
				}
		}

		/// <summary>
		///		Carga las columnas de la vista
		/// </summary>
		private void LoadColumns(SQLServerProvider connection, SchemaView view)
		{
			ParametersDBCollection parameters = new ParametersDBCollection();
			string sql;

				// Asigna lo parámetros
				parameters.Add("@View_Catalog", view.Catalog);
				parameters.Add("@View_Schema", view.Schema);
				parameters.Add("@View_Name", view.Name);
				// Crea la cadena SQL
				sql = @"SELECT Table_Catalog, Table_Schema, Table_Name, Column_Name
						  FROM Information_Schema.View_Column_Usage
						  WHERE View_Catalog = @View_Catalog
								AND View_Schema = @View_Schema
								AND View_Name = @View_Name";
				// Carga las columnas
				using (IDataReader columnsReader = connection.ExecuteReader(sql, parameters, CommandType.Text))
				{ 
					// Lee los registros
					while (columnsReader.Read())
					{
						SchemaColumn column = new SchemaColumn(view.Parent);

							// Carga los datos de la columna
							column.Catalog = (string) columnsReader.IisNull("Table_Catalog");
							column.Schema = (string) columnsReader.IisNull("Table_Schema");
							column.Table = (string) columnsReader.IisNull("Table_Name");
							column.Name = (string) columnsReader.IisNull("Column_Name");
							// Añade la columna a la colección
							view.Columns.Add(column);
					}
					// Cierra el recordset
					columnsReader.Close();
				}
		}
	}
}