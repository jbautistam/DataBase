using System;

namespace Bau.Libraries.LibDBAggregator
{
	/// <summary>
	///		Manager de orígenes de datos
	/// </summary>
	public class DataSourcesManager
	{
		/// <summary>
		///		Limpia los datos
		/// </summary>
		public void Clear()
		{
			Connections.Clear();
		}

		/// <summary>
		///		Carga las conexiones de un archivo
		/// </summary>
		public void Load(string fileName)
		{
			Connections.AddRange(new Repository.DataSourcesRepository().Load(fileName));
		}

		/// <summary>
		///		Carga las conexiones de un nodo
		/// </summary>
		public void Load(LibMarkupLanguage.MLNode rootML)
		{
			Connections.AddRange(new Repository.DataSourcesRepository().Load(rootML));
		}

		/// <summary>
		///		Graba las conexiones en un archivo
		/// </summary>
		public void Save(string fileName)
		{
			new Repository.DataSourcesRepository().Save(fileName, Connections);
		}

		/// <summary>
		///		Obtiene un nodo con las conexiones
		/// </summary>
		public LibMarkupLanguage.MLNode GetMLNode(string tagRoot)
		{
			return new Repository.DataSourcesRepository().GetMLNode(tagRoot, Connections);
		}

		/// <summary>
		///		Conexiones
		/// </summary>
		public Models.ConnectionModelCollection Connections { get; } = new Models.ConnectionModelCollection();
	}
}
