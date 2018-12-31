using System;

using Bau.Libraries.LibCommonHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibDBAggregator.Models;
using Bau.Libraries.LibDBAggregator.Models.DataBase;

namespace Bau.Libraries.LibDBAggregator.Repository
{
	/// <summary>
	///		Repository para <see cref="AbstractConnectionModel"/>
	/// </summary>
	internal class DataSourcesRepository
	{
		// Constantes privadas
		private const string TagRoot = "Connections";
		private const string TagId = "Id";
		private const string TagName = "Name";
		private const string TagDescription = "Description";
		private const string TagSqlServer = "SqlServer";
		private const string TagSqLite = "SqLite";
		private const string TagServer = "Server";
		private const string TagPort = "Port";
		private const string TagUser = "User";
		private const string TagPassword = "Password";
		private const string TagDatabase = "Database";
		private const string TagUseIntegratedSecurity = "UseIntegratedSecurity";
		private const string TagFileName = "FileName";

		/// <summary>
		///		Carga las conexiones de un archivo
		/// </summary>
		internal ConnectionModelCollection Load(string fileName)
		{
			var connections = new ConnectionModelCollection();
			MLFile fileML = new LibMarkupLanguage.Services.XML.XMLParser().Load(fileName);

				// Carga las conexiones
				if (fileML != null)
					foreach (MLNode rootML in fileML.Nodes)
						if (rootML.Name == TagRoot)
							connections.AddRange(Load(rootML));
				// Devuelve las conexiones
				return connections;
		}

		/// <summary>
		///		Carga las conexiones de un nodo
		/// </summary>
		internal ConnectionModelCollection Load(MLNode rootML)
		{
			var connections = new ConnectionModelCollection();

				// Carga las conexiones
				foreach (MLNode nodeML in rootML.Nodes)
					switch (nodeML.Name)
					{
						case TagSqlServer:
								connections.Add(LoadSqlServerConnection(nodeML));
							break;
						case TagSqLite:
								connections.Add(LoadSqLiteConnection(nodeML));
							break;
					}
				// Devuelve la colección
				return connections;
		}

		/// <summary>
		///		Carga los datos de una conexión a SQL Server
		/// </summary>
		private SqlServerConnectionModel LoadSqlServerConnection(MLNode rootML)
		{
			var connection = new SqlServerConnectionModel();

				// Carga los datos básicos
				LoadBaseConnection(connection, rootML);
				// Carga el resto de datos
				connection.Server = rootML.Attributes[TagServer].Value;
				connection.Port = rootML.Attributes[TagPort].Value.GetInt(0);
				connection.DataBase = rootML.Attributes[TagDatabase].Value;
				connection.User = rootML.Attributes[TagUser].Value;
				connection.Password = rootML.Attributes[TagPassword].Value;
				connection.UseIntegratedSecurity = rootML.Attributes[TagUseIntegratedSecurity].Value.GetBool();
				// Devuelve la conexión
				return connection;
		}

		/// <summary>
		///		Carga una conexión a SqLite
		/// </summary>
		private SqLiteConnectionModel LoadSqLiteConnection(MLNode rootML)
		{
			var connection = new SqLiteConnectionModel();

				// Carga los datos básicos
				LoadBaseConnection(connection, rootML);
				// Carga las propiedades
				connection.FileName = rootML.Nodes[TagFileName].Value;
				connection.Password = rootML.Attributes[TagPassword].Value;
				// Devuelve la conexión
				return connection;
		}

		/// <summary>
		///		Obtiene los datos básicos de una conexión
		/// </summary>
		private void LoadBaseConnection(AbstractConnectionModel connection, MLNode rootML)
		{
			connection.GlobalId = rootML.Attributes[TagId].Value;
			connection.Name = rootML.Nodes[TagName].Value;
			connection.Description = rootML.Nodes[TagDescription].Value;
		}

		/// <summary>
		///		Graba los datos de las conexiones en un archivo
		/// </summary>
		internal void Save(string fileName, ConnectionModelCollection connections)
		{
			MLFile fileML = new MLFile();

				// Añade el nodo raíz
				fileML.Nodes.Add(GetMLNode(TagRoot, connections));
				// Graba el archivo
				new LibMarkupLanguage.Services.XML.XMLWriter().Save(fileName, fileML);
		}

		/// <summary>
		///		Obtiene un nodo con una serie de conexiones
		/// </summary>
		internal MLNode GetMLNode(string tagRoot, ConnectionModelCollection connections)
		{
			MLNode rootML = new MLNode(tagRoot);

				// Añade los nodos de las conexiones
				foreach (AbstractConnectionModel connection in connections)
					switch (connection)
					{
						case SqlServerConnectionModel cnn:
								rootML.Nodes.Add(GetMLNodeSqlServer(cnn));
							break;
						case SqLiteConnectionModel cnn:
								rootML.Nodes.Add(GetMLNodeSqLite(cnn));
							break;
					}
				// Devuelve el nodo raíz
				return rootML;
		}

		/// <summary>
		///		Obtiene un nodo de una conexión SQL Server
		/// </summary>
		private MLNode GetMLNodeSqlServer(SqlServerConnectionModel connection)
		{
			MLNode rootML = GetMLNodeBase(TagSqlServer, connection);

				// Añade los datos de la conexión
				rootML.Attributes.Add(TagServer, connection.Server);
				rootML.Attributes.Add(TagPort, connection.Port);
				rootML.Attributes.Add(TagDatabase, connection.DataBase);
				rootML.Attributes.Add(TagUser, connection.User);
				rootML.Attributes.Add(TagPassword, connection.Password);
				rootML.Attributes.Add(TagUseIntegratedSecurity, connection.UseIntegratedSecurity);
				// Devuelve el nodo
				return rootML;
		}

		/// <summary>
		///		Obtiene un nodo de una conexión SqLite
		/// </summary>
		private MLNode GetMLNodeSqLite(SqLiteConnectionModel connection)
		{
			MLNode rootML = GetMLNodeBase(TagSqLite, connection);

				// Añade los datos de la conexión 
				rootML.Attributes.Add(TagPassword, connection.Password);
				rootML.Nodes.Add(TagFileName, connection.FileName);
				// Devuelve el nodo
				return rootML;
		}

		/// <summary>
		///		Obtiene los datos básicos de un nodo de conexión
		/// </summary>
		private MLNode GetMLNodeBase(string tag, AbstractConnectionModel connection)
		{
			MLNode rootML = new MLNode(tag);

				// Añade los datos
				rootML.Attributes.Add(TagId, connection.GlobalId);
				rootML.Nodes.Add(TagName, connection.Name);
				rootML.Nodes.Add(TagDescription, connection.Description);
				// Devuelve el nodo
				return rootML;
		}
	}
}
