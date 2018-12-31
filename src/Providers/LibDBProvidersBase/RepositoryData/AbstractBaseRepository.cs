using System;
using System.Collections.Generic;
using System.Data;

using Bau.Libraries.LibDBProvidersBase.Parameters;

namespace Bau.Libraries.LibDBProvidersBase.RepositoryData
{
	/// <summary>
	///		Base de los repository
	/// </summary>
	public abstract class AbstractBaseRepository<TypeData> where TypeData : new()
	{ 
		// Variables privadas
		private object _lock = new object();

		public AbstractBaseRepository(IDBProvider provider)
		{
			Provider = provider;
		}

		/// <summary>
		///		Carga los datos de un elemento
		/// </summary>
		public virtual TypeData Load(int? id)
		{
			return LoadObject(GetSQLSelectByID(), NameParameterID, id, CommandType.Text);
		}

		/// <summary>
		///		Carga los datos de un objeto
		/// </summary>
		public virtual TypeData LoadObject(string sql, string parameterName, int? parameterValue, CommandType commandType = CommandType.Text)
		{
			ParametersDBCollection parametersDB = new ParametersDBCollection();

				// Asigna los parámetros
				parametersDB.Add(parameterName, parameterValue);
				// Carga los datos
				return LoadObject(sql, parametersDB, commandType);
		}

		/// <summary>
		///		Carga los datos de un objeto
		/// </summary>
		public virtual TypeData LoadObject(string sql, string parameterName, string parameterValue, 
										   int length, CommandType commandType = CommandType.Text)
		{
			ParametersDBCollection parametersDB = new ParametersDBCollection();

				// Asigna los parámetros
				parametersDB.Add(parameterName, parameterValue, length);
				// Carga los datos
				return LoadObject(sql, parametersDB, commandType);
		}

		/// <summary>
		///		Carga los datos de un elemento
		/// </summary>
		public virtual TypeData LoadObject(string sql, ParametersDBCollection parametersDB,
										   CommandType commandType = CommandType.Text)
		{
			TypeData data = default(TypeData);

				lock (_lock)
				{   
					// Abre la conexión
					Provider.Open();
					// Lee los datos
					using (IDataReader reader = Provider.ExecuteReader(sql, parametersDB, commandType))
					{ 
						// Lee los datos
						if (reader.Read())
							data = (TypeData) AssignData(reader);
						else
							data = (TypeData) AssignData(null);
						// Cierra el recordset
						reader.Close();
					}
					// Cierra la conexión
					Provider.Close();
				}
				// Devuelve el objeto
				return data;
		}

		/// <summary>
		///		Carga una colección
		/// </summary>
		public void LoadCollection(IList<TypeData> data, string sql, string fieldName, int? fieldValue,
								   CommandType commandType = CommandType.Text, bool checkNullsBefore = false)
		{
			if (!checkNullsBefore || (checkNullsBefore && fieldValue != null))
			{
				ParametersDBCollection parametersDB = new ParametersDBCollection();

					// Asigna los parámetros
					parametersDB.Add(fieldName, fieldValue);
					// Carga los datos
					LoadCollection(data, sql, parametersDB, commandType);
			}
		}

		/// <summary>
		///		Carga una colección
		/// </summary>
		public void LoadCollection(IList<TypeData> data, string sql, string fieldName,
								   string fieldValue, int length, CommandType commandType = CommandType.Text)
		{
			ParametersDBCollection parametersDB = new ParametersDBCollection();

				// Asigna los parámetros
				parametersDB.Add(fieldName, fieldValue, length);
				// Carga los datos
				LoadCollection(data, sql, parametersDB, commandType);
		}

		/// <summary>
		///		Carga una colección
		/// </summary>
		public void LoadCollection(IList<TypeData> data, string sql, ParametersDBCollection parametersDB,
								   CommandType commandType = CommandType.Text)
		{
			lock (_lock)
			{   
				// Abre la conexión
				Provider.Open();
				// Carga los datos
				using (IDataReader reader = Provider.ExecuteReader(sql, parametersDB, commandType))
				{ 
					// Lee los datos
					while (reader.Read())
						data.Add((TypeData) AssignData(reader));
					// Cierra el recordset
					reader.Close();
				}
				// Cierra la conexión
				Provider.Close();
			}
		}

		/// <summary>
		///		Carga una colección
		/// </summary>
		public IEnumerable<IDataReader> LoadEnumerable(string sql, ParametersDBCollection parametersDB,
													   CommandType commandType = CommandType.Text)
		{
			lock (_lock)
			{   
				// Abre la conexión
				Provider.Open();
				// Carga los datos
				using (IDataReader reader = Provider.ExecuteReader(sql, parametersDB, commandType))
				{ 
					// Lee los datos
					while (reader.Read())
						yield return reader;
					// Cierra el recordset
					reader.Close();
				}
				// Cierra la conexión
				Provider.Close();
			}
		}

		/// <summary>
		///		Asigna los datos de un recordset al objeto
		/// </summary>
		protected abstract object AssignData(IDataReader reader);

		/// <summary>
		///		Graba los datos de un registro
		/// </summary>
		public virtual void Save(object data, ref int? intdataId)
		{
			Save((TypeData) data, ref intdataId);
		}

		/// <summary>
		///		Graba los datos de un registro
		/// </summary>
		public virtual void Save(TypeData data, ref int? dataId)
		{
			lock (_lock)
			{   
				// Abre el proveedor
				Provider.Open();
				// Graba los datos
				Save(data, FillParametersSave(data), ref dataId);
				// Cierra proveedor
				Provider.Close();
			}
		}

		/// <summary>
		///		Graba los datos de un registro
		/// </summary>
		public virtual void Save(TypeData data, ParametersDBCollection parametersDB, ref int? intdataId)
		{
			lock (_lock)
			{
				string sqlUpdate = GetSQLUpdate();

					// Graba los datos
					if (intdataId == null || string.IsNullOrWhiteSpace(sqlUpdate))
						intdataId = Provider.ExecuteGetIdentity(GetSQLInsert(), parametersDB, CommandType.Text);
					else
						Provider.Execute(sqlUpdate, parametersDB, CommandType.Text);
			}
		}

		/// <summary>
		///		Obtiene los parámetros de grabación
		/// </summary>
		protected abstract ParametersDBCollection FillParametersSave(TypeData data);

		/// <summary>
		///		Ejecuta una cadena SQL sobre la base de datos
		/// </summary>
		protected void Execute(string sql, string parameterName, int? parameterValue,
							   bool checkNullsBefore = true, CommandType commandType = CommandType.Text)
		{
			if (!checkNullsBefore || (checkNullsBefore && parameterValue != null))
			{
				ParametersDBCollection parametersDB = new ParametersDBCollection();

					// Asigna los parámetros
					parametersDB.Add(parameterName, parameterValue);
					// Ejecuta la cadena SQL
					Execute(sql, parametersDB, commandType);
			}
		}

		/// <summary>
		///		Ejecuta una cadena SQL sobre la base de datos
		/// </summary>
		protected void Execute(string sql, ParametersDBCollection parametersDB, CommandType commandType = CommandType.Text)
		{
			lock (_lock)
			{ 
				// Abre el proveedor
				Provider.Open();
				// Ejecuta la cadena SQL
				Provider.Execute(sql, parametersDB, commandType);
				// Cierra el proveedor
				Provider.Close();
			}
		}

		/// <summary>
		///		Ejecuta una cadena SQL sobre la base de datos y devuelve un escalar
		/// </summary>
		protected object ExecuteScalar(string sql, string parameterName, int? parameterValue, CommandType commandType = CommandType.Text)
		{
			ParametersDBCollection parametersDB = new ParametersDBCollection();

				// Asigna los parámetros
				parametersDB.Add(parameterName, parameterValue);
				// Ejecuta la cadena SQL
				return ExecuteScalar(sql, parametersDB, commandType);
		}

		/// <summary>
		///		Ejecuta una cadena SQL sobre la base de datos y devuelve un escalar
		/// </summary>
		protected object ExecuteScalar(string sql, ParametersDBCollection parametersDB, CommandType commandType = CommandType.Text)
		{
			object scalar = null;

				// Bloquea antes de ejecutar
				lock (_lock)
				{ 
					// Abre el proveedor
					Provider.Open();
					// Ejecuta
					scalar = Provider.ExecuteScalar(sql, parametersDB, commandType);
					// Cierra el proveedor
					Provider.Close();
				}
				// Devuelve el escalar
				return scalar;
		}

		/// <summary>
		///		Comprueba si puede borrar un registro (el método base siempre devuelve true)
		/// </summary>
		public virtual bool CanDelete(int? id)
		{
			return true;
		}

		/// <summary>
		///		Borra los datos de un registro
		/// </summary>
		public virtual void Delete(int? id)
		{
			if (id != null)
				Execute(GetSQLDeleteByID(), NameParameterID, id);
		}

		/// <summary>
		///		Obtiene la cadena SQL de selección por ID del registro
		/// </summary>
		protected abstract string GetSQLSelectByID();

		/// <summary>
		///		Obtiene la cadena SQL de inserción de un registro
		/// </summary>
		protected abstract string GetSQLInsert();

		/// <summary>
		///		Obtiene la cadena SQL de modificación de un registro
		/// </summary>
		protected abstract string GetSQLUpdate();

		/// <summary>
		///		Obtiene la cadena SQL de borrado de un registro
		/// </summary>
		protected abstract string GetSQLDeleteByID();

		/// <summary>
		///		Normaliza las fechas
		/// </summary>
		protected void NormalizeDates(ref DateTime start, ref DateTime end)
		{
			DateTime dtmInter = start;

				// Intercambia las fechas
				start = end;
				end = dtmInter;
				// Deja la fecha de inicio a 0 y la fecha de fin al final del día
				start = start.Date;
				end = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59);
		}

		/// <summary>
		///		Obtiene una cadena con una condición de filtros de fechas
		/// </summary>
		/// <remarks>
		///		Dependiendo de las fechas pasadas al filtro devuelve una cadena del estilo:
		///			* field BETWEEN strParameterStart AND strParameterEnd
		///			* field &gt;= strParameterStart
		///			* field &lt;= strParameterEnd
		/// </remarks>
		protected string GetSQLFilterDates(string field, string parameterStart, string parameterEnd, DateTime? start, DateTime? end)
		{
			string sql = "";

				// Asigna la cadena SQL
				if (start != null && end != null)
					sql += $" {field} BETWEEN {parameterStart} AND {parameterEnd}";
				else if (start != null && end == null)
					sql += $" {field} >= {parameterStart}";
				else if (start == null && end != null)
					sql += $" {field} <= {parameterEnd}";
				// Devuelve la cadena SQL
				return sql;
		}

		/// <summary>
		///		Obtiene el SQL para una condición IN
		/// </summary>
		protected string GetSQLIn(List<int?> ids)
		{
			string sql = "";

				// Añade los IDs a la cadena
				foreach (int? id in ids)
					if (id != null)
					{ 
						// Añade una coma si es necesario
						if (!string.IsNullOrEmpty(sql))
							sql += ",";
						// Añade el ID
						sql += id.ToString();
					}
				// Devuelve la condición SQL	
				return sql;
		}

		/// <summary>
		///		Nombre del parámetro que se utiliza como ID
		/// </summary>
		protected abstract string NameParameterID { get; }

		/// <summary>
		///		Proveedor de datos
		/// </summary>
		protected IDBProvider Provider { get; set; }
	}
}