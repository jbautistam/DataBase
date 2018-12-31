using System;
using System.Data;

using Bau.Libraries.LibDBProvidersBase.Parameters;
using Bau.Libraries.LibDBProvidersBase.RepositoryData;

namespace Bau.Libraries.LibDBProviderSQLServerCE
{
	/// <summary>
	///		Clase para ayuda de los repository de bases de datos SQL Server Compact
	/// </summary>
	public class SQLServerCompactRepository : AbstractProviderRepository
	{	
		public SQLServerCompactRepository(SQLServerCompactProvider objProvider) : base(objProvider) 
		{	
		}

		/// <summary>
		///		Obtiene una instancia de este repositorio
		/// </summary>
		public override IProviderRepository GetInstance()
		{ return this;
		}
	}
}
