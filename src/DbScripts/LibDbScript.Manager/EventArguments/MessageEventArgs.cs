using System;

namespace Bau.Libraries.LibDbScripts.Manager.EventArguments
{
	/// <summary>
	///		Argumentos de los eventos de un mensaje
	/// </summary>
	public class MessageEventArgs : EventArgs
	{
		/// <summary>
		///		Tipo de mensaje
		/// </summary>
		public enum MessageType
		{
			/// <summary>Depuración</summary>
			Debug,
			/// <summary>Informativo</summary>
			Info,
			/// <summary>Error</summary>
			Error
		}

		public MessageEventArgs(string message, MessageType type)
		{
			Message = message;
			Type = type;
		}

		/// <summary>
		///		Mensaje
		/// </summary>
		public string Message { get; }

		/// <summary>
		///		Tipo de mensaje
		/// </summary>
		public MessageType Type { get; }
	}
}
