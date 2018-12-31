using System;

using Bau.Libraries.LibDbProviders.Base.RepositoryData;
using Bau.Libraries.LibDbProviders.Base.Parameters;

namespace Bau.Libraries.LibPostgreSqlProvider
{
	/// <summary>
	///		Clase para ayuda de repository de PostgreSql
	/// </summary>
	public class PostgreSqlRepository<TypeData> : RepositoryDataBase<TypeData>
	{
		public PostgreSqlRepository(PostgreSqlProvider connection) : base(connection)
		{
		}

		/// <summary>
		///		Devuelve el valor identidad
		/// </summary>
		protected override int? GetIdentityValue(ParametersDBCollection parametersDB)
		{ 
			return (int?) parametersDB["@return_code"].Value;
		}
	}
}
