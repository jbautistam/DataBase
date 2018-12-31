﻿using System;

using Bau.Libraries.LibDbProviders.Base.RepositoryData;
using Bau.Libraries.LibDbProviders.Base.Parameters;

namespace Bau.Libraries.LibMySqlProvider
{
	/// <summary>
	///		Clase para ayuda de repository de MySql
	/// </summary>
	public class MySqlRepository<TypeData> : RepositoryDataBase<TypeData>
	{
		public MySqlRepository(MySqlProvider connection) : base(connection)
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