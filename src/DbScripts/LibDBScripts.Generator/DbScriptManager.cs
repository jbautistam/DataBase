using System;
using System.Collections.Generic;

using Bau.Libraries.LibCommonHelper.Extensors;
using Bau.Libraries.Aggregator.Providers.Base;

namespace Bau.Libraries.LibDbScripts.Generator
{
	/// <summary>
	///		Manager para  la ejecución de scripts
	/// </summary>
	public class DbScriptManager
	{
		// Eventos públicos
		public event EventHandler<EventArguments.MessageEventArgs> Log;

		public DbScriptManager(DataProviderCollection dataProviders, Dictionary<string, object> parameters, string pathBase)
		{
			DataProviders = dataProviders;
			Parameters = parameters;
			PathBase = pathBase;
		}

		/// <summary>
		///		Ejecuta el script desde un archivo
		/// </summary>
		public bool ProcessByFile(string fileName)
		{
			return Process(new Processor.Repository.DbScriptRepository().LoadByFile(fileName));
		}

		/// <summary>
		///		Ejecuta el script desde una cadena XML
		/// </summary>
		public bool ProcessByXml(string xml)
		{
			return Process(new Processor.Repository.DbScriptRepository().LoadByText(xml));
		}

		/// <summary>
		///		Ejecuta el programa
		/// </summary>
		private bool Process(Processor.Sentences.ProgramModel program)
		{
			Processor.DbScriptProcessor processor = new Processor.DbScriptProcessor(this, program);

				// Ejecuta el script
				processor.Process();
				// Devuelve el valor que indica si ha habido errores
				return !CheckError();
		}

		/// <summary>
		///		Comprueba si ha habido algún error
		/// </summary>
		private bool CheckError()
		{
			// Recorre los eventos comprobando si ha habido algún error
			foreach (EventArguments.MessageEventArgs message in Events)
				if (message.Type == EventArguments.MessageEventArgs.MessageType.Error)
					return true;
			// Si ha llegado hasta aquí es porque no ha habido ningún error
			return false;
		}

		/// <summary>
		///		Lanza un mensaje
		/// </summary>
		internal void RaiseMessage(string message, EventArguments.MessageEventArgs.MessageType type)
		{
			EventArguments.MessageEventArgs args = new EventArguments.MessageEventArgs(message.TrimIgnoreNull(), type);

				// Añade el mensaje a la lista de eventos
				Events.Add(args);
				// Lanza el evento
				Log?.Invoke(this, args);
		}

		/// <summary>
		///		Proveedores de datos
		/// </summary>
		internal DataProviderCollection DataProviders { get; }

		/// <summary>
		///		Parámetros iniciales
		/// </summary>
		internal Dictionary<string, object> Parameters { get; }

		/// <summary>
		///		Directorio base
		/// </summary>
		internal string PathBase { get; }

		/// <summary>
		///		Mensajes de eventos
		/// </summary>
		public List<EventArguments.MessageEventArgs> Events { get; } = new List<EventArguments.MessageEventArgs>();
	}
}
