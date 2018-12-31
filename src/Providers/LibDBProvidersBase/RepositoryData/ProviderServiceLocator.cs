using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibDBProvidersBase.RepositoryData
{
	/// <summary>
	///		Controlador de repositorios
	/// </summary>
	public class ProviderServiceLocator
	{   
		// Variables privadas
		private static ProviderServiceLocator _instance = null;

		public ProviderServiceLocator()
		{
			Repositories = new Dictionary<string, IProviderRepository>();
		}

		/// <summary>
		///		Obtiene una instancia del controlador
		/// </summary>
		public static ProviderServiceLocator GetInstance()
		{   
			// Crea una instancia global si no existía
			if (_instance == null)
				_instance = new ProviderServiceLocator();
			// Devuelve la instancia
			return _instance;
		}

		/// <summary>
		///		Registra un proveedor de un repositorio para un tipo
		/// </summary>
		public void Register(string key, IProviderRepository repository)
		{
			if (!Repositories.ContainsKey(key))
				Repositories.Add(key, repository);
		}

		/// <summary>
		///		Obtiene un nuevo proveedor de repositorio para la clave definida
		/// </summary>
		public IProviderRepository GetInstance(string key)
		{
			if (Repositories.ContainsKey(key))
				return Repositories[key].GetInstance();
			else
				return null;
		}

		/// <summary>
		///		Diccionario con los repositorios
		/// </summary>
		private Dictionary<string, IProviderRepository> Repositories { get; set; }
	}
}
