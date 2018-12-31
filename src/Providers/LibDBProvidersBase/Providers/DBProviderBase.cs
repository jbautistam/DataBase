using System;
using System.Data;

using Bau.Libraries.LibDBProvidersBase.Parameters;

namespace Bau.Libraries.LibDBProvidersBase.Providers
{
	/// <summary>
	///		Clase base para los proveedores de base de datos
	/// </summary>
	public abstract class DBProviderBase : IDBProvider
	{ 
		// Variables privadas
		private object _lock = new object();
		private int _open = 0;

		public DBProviderBase(IDbConnection connection)
		{ 
			Connection = connection;
			Transaction = null;
		}

		/// <summary>
		///		Abre la conexión a la base de datos
		/// </summary>
		public void Open()
		{
			lock (_lock)
			{
				if (Connection.State == ConnectionState.Closed)
					Connection.Open();
				_open++;
			}
		}

		/// <summary>
		///		Obtiene un comando
		/// </summary>
		protected abstract IDbCommand GetCommand(string text);

		/// <summary>
		///		Cierra la conexión a la base de datos
		/// </summary>
		public virtual void Close()
		{
			lock (_lock)
			{
				_open--;
				if (Connection != null && Connection.State == ConnectionState.Open && _open <= 0)
					Connection.Close();
			}
		}

		/// <summary>
		///		Ejecuta una sentencia o un procedimiento sobre la base de datos
		/// </summary>
		public void Execute(string sql, ParametersDBCollection parameters, CommandType commandType)
		{
			lock (_lock)
			{
				using (IDbCommand command = GetCommand(sql))
				{ 
					// Indica el tipo del comando
					command.CommandType = commandType;
					// Añade los parámetros al comando
					AddParameters(command, parameters);
					// Ejecuta la consulta
					command.ExecuteNonQuery();
					// Pasa los valores de salida de los parámetros del comando a la colección de parámetros de entrada
					parameters = ReadOutputParameters(command.Parameters);
				}
			}
		}

		/// <summary>
		///		Obtiene un DataReader
		/// </summary>
		public IDataReader ExecuteReader(string sql, ParametersDBCollection parametersDB, CommandType commandType)
		{
			IDataReader reader = null; // ... supone que no se puede abrir el dataReader			

				// Ejecuta el lector
				lock (_lock)
				{
					using (IDbCommand command = GetCommand(sql))
					{ 
						// Indica el tipo de comando
						command.CommandType = commandType;
						// Añade los parámetros
						AddParameters(command, parametersDB);
						// Obtiene el dataReader
						reader = command.ExecuteReader();
					}
				}
				// Devuelve el dataReader
				return reader;
		}

		/// <summary>
		///		Ejecuta una sentencia o procedimiento sobre la base de datos y devuelve un escalar
		/// </summary>
		public object ExecuteScalar(string sql, ParametersDBCollection parameters, CommandType commandType)
		{
			object result = null;

				// Ejecuta el comando
				lock (_lock)
				{
					using (IDbCommand command = GetCommand(sql))
					{ 
						// Indica el tipo de comando
						command.CommandType = commandType;
						// Añade los parámetros al comando
						AddParameters(command, parameters);
						// Ejecuta la consulta
						result = command.ExecuteScalar();
					}
				}
				// Devuelve el resultado
				return result;
		}


		/// <summary>
		///		Ejecuta un INSERT sobre la base de datos y obtiene el valor de identidad
		/// </summary>
		public abstract int? ExecuteGetIdentity(string text, ParametersDBCollection parametersDB, CommandType commandType);

		/// <summary>
		///		Obtiene un dataTable a partir de un nombre de una sentencia o procedimiento y sus parámetros
		/// </summary>
		public DataTable GetDataTable(string sql, ParametersDBCollection parameters, CommandType commandType)
		{
			DataTable table = new DataTable();

				// Crea el comando SQL Server
				lock (_lock)
				{
					using (IDbCommand command = GetCommand(sql))
					{ 
						// Inicializa el tipo de comando
						command.CommandType = commandType;
						// Pasa los parámetros al comando
						AddParameters(command, parameters);
						// Rellena la tabla con los datos
						table = FillDataTable(command);
					}
				}
				// Devuelve la tabla
				return table;
		}

		/// <summary>
		///		Obtiene un adaptador de datos
		/// </summary>
		protected abstract DataTable FillDataTable(IDbCommand command);

		/// <summary>
		///		Añade a un comando los parámetros de una clase <see cref="ParametersDBCollection"/>
		/// </summary>
		protected void AddParameters(IDbCommand command, ParametersDBCollection parameters)
		{ 
			// Limpia los parámetros antiguos
			command.Parameters.Clear();
			// Añade los parámetros nuevos
			if (parameters != null)
				for (int index = 0; index < parameters.Count; index++)
					command.Parameters.Add(GetSQLParameter(parameters[index]));
		}

		/// <summary>
		///		Obtiene un parámetro SQLServer a partir de un parámetro genérico
		/// </summary>
		private IDataParameter GetSQLParameter(ParameterDB parameterDB)
		{
			IDataParameter parameterDataDB = ConvertParameter(parameterDB);

				// Asigna el valor
				parameterDataDB.Value = parameterDB.GetDBValue();
				// Asigna la dirección
				parameterDataDB.Direction = parameterDB.Direction;
				// Devuelve el parámetro
				return parameterDataDB;
		}

		/// <summary>
		///		Método abstracto para convertir un parámetro
		/// </summary>
		protected abstract IDataParameter ConvertParameter(ParameterDB parameter);

		/// <summary>
		///		Lee los parámetros de salida
		/// </summary>
		private ParametersDBCollection ReadOutputParameters(IDataParameterCollection parametersData)
		{
			ParametersDBCollection parametersDB = new ParametersDBCollection();

				// Recupera los parámetros
				foreach (IDataParameter parameterData in parametersData)
				{
					ParameterDB parameterDB = new ParameterDB();

						// Asigna los datos
						parameterDB.Name = parameterData.ParameterName;
						parameterDB.Direction = parameterData.Direction;
						if (parameterData.Value == DBNull.Value)
							parameterDB.Value = null;
						else
							parameterDB.Value = parameterData.Value;
						// Añade el parámetro a la colección
						parametersDB.Add(parameterDB);
				}
				// Devuelve la colección de parámetros
				return parametersDB;
		}

		/// <summary>
		///		Inicia una transacción
		/// </summary>
		public void BeginTransaction()
		{
			Transaction = Connection.BeginTransaction();
		}

		/// <summary>
		///		Confirma una transacción
		/// </summary>
		public void Commit()
		{
			if (Transaction != null)
				Transaction.Commit();
			Transaction = null;
		}

		/// <summary>
		///		Deshace una transacción
		/// </summary>
		public void RollBack()
		{
			if (Transaction != null)
				Transaction.Rollback();
			Transaction = null;
		}

		/// <summary>
		///		Desconecta la conexión
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		///		Desconecta la conexión
		/// </summary>
		private void Dispose(bool disposing)
		{
			if (disposing && Connection != null)
			{
				Close();
				Connection.Dispose();
				Connection = null;
			}
		}

		/// <summary>
		///		Destruye el objeto
		/// </summary>
		~DBProviderBase()
		{
			Dispose(false);
		}

		/// <summary>
		///		Conexión
		/// </summary>
		protected IDbConnection Connection { get; private set; }

		/// <summary>
		///		Transacción
		/// </summary>
		protected IDbTransaction Transaction { get; set; }
	}
}