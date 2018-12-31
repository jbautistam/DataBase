using System;
using System.Collections.Generic;
using System.Data;

namespace Bau.Libraries.LibDBProvidersBase.RepositoryData
{   
	// Delegados públicos de este espacio de nombres
	/// <summary>
	///		Callback para asignar los datos de lectura a un registro
	/// </summary>
	public delegate object AssignDataCallBack(IDataReader reader);

	/// <summary>
	///		Interface para los objetos de repositorio
	/// </summary>
	public interface IProviderRepository : IDisposable
	{
		/// <summary>
		///		Obtiene una instancia nueva del objeto
		/// </summary>
		IProviderRepository GetInstance();

		/// <summary>
		///		Carga los datos de una colección
		/// </summary>
		void LoadCollection<TypeData>(IList<TypeData> data, string text, CommandType commandType, AssignDataCallBack callBackFunction)
					where TypeData : new();

		/// <summary>
		///		Carga una colección utilizando genéricos para un procedimiento con un único 
		/// parámetro de entrada alfanumérico
		/// </summary>
		void LoadCollection<TypeData>(IList<TypeData> data, string text, string parameterName,
									  string parameterValue, int parameterLength,
									  CommandType commandType, AssignDataCallBack callbackFunction)
					where TypeData : new();

		/// <summary>
		///		Carga una colección utilizando genéricos para un procedimiento con un único 
		/// parámetro de entrada numérico
		/// </summary>
		void LoadCollection<TypeData>(IList<TypeData> data, string text, string parameterName, int? parameterValue, 
									  CommandType commandType, AssignDataCallBack callbackFunction)
					where TypeData : new();

		/// <summary>
		///		Carga una colección utilizando genéricos
		/// </summary>
		void LoadCollection<TypeData>(IList<TypeData> data, string text, Parameters.ParametersDBCollection parameters,
									  CommandType commandType, AssignDataCallBack callbackFunction)
					where TypeData : new();

		/// <summary>
		///		Carga un enumerable
		/// </summary>
		IEnumerable<IDataReader> LoadEnumerable(string sql, Parameters.ParametersDBCollection parametersDB,
												CommandType commandType = CommandType.Text);

		/// <summary>
		///		Carga un objeto utilizando genéricos para un procedimiento con un único parámetro alfanumérico
		/// </summary>
		TypeData LoadObject<TypeData>(string text, string parameterName, string parameterValue, int parameterLength,
									  CommandType commandType, AssignDataCallBack callbackFunction) 
					where TypeData : new();

		/// <summary>
		///		Carga un objeto utilizando genéricos para un procedimiento con un único parámetro numérico
		/// </summary>
		TypeData LoadObject<TypeData>(string text, string parameterName, int? parameterValue,
									  CommandType commandType, AssignDataCallBack callbackFunction) 
					where TypeData : new();

		/// <summary>
		///		Carga un objeto utilizando genéricos
		/// </summary>
		TypeData LoadObject<TypeData>(string text, Parameters.ParametersDBCollection parametersDB,
									  CommandType commandType, AssignDataCallBack callbackFunction) 
					where TypeData : new();

		/// <summary>
		///		Ejecuta una sentencia sobre la conexión
		/// </summary>
		void Execute(string text, string parameterName, string parameterValue, int parameterLength, CommandType commandType);

		/// <summary>
		///		Ejecuta una sentencia sobre la conexión
		/// </summary>
		void Execute(string text, string parameterName, int? parameterValue, CommandType commandType);

		/// <summary>
		///		Ejecuta una sentencia sobre la conexión
		/// </summary>
		void Execute(string text, Parameters.ParametersDBCollection parametersDB, CommandType commandType);

		/// <summary>
		///		Ejecuta una sentencia sobre la conexión y devuelve un escalar
		/// </summary>
		object ExecuteScalar(string text, string parameterName, string parameterValue, int parameterLength, CommandType commandType);

		/// <summary>
		///		Ejecuta una sentencia sobre la conexión y devuelve un escalar
		/// </summary>
		object ExecuteScalar(string text, string parameterName, int? parameterValue, CommandType commandType);

		/// <summary>
		///		Ejecuta una sentencia sobre la conexión y devuelve un escalar
		/// </summary>
		object ExecuteScalar(string text, Parameters.ParametersDBCollection parametersDB, CommandType commandType);

		/// <summary>
		///		Conexión con la que trabaja el repository
		/// </summary>
		IDBProvider Provider { get; }
	}
}