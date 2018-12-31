using System;

namespace Bau.Libraries.LibDBProvidersBase.DBExceptions
{
	/// <summary>
	///		Excepción de base de datos
	/// </summary>
	public class DBException : Exception
	{
		public DBException(string message) : base(message) { }

		public DBException(string message, Exception innerException) : base(message, innerException) { }

		public DBException() : base()
		{
		}

		protected DBException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{
		}
	}
}
