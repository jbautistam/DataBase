using System;
using System.Collections.Generic;

using Bau.Libraries.DbAggregator.Models;

namespace Bau.Libraries.DbAggregator.Builders
{
	/// <summary>
	///		Generador para comandos
	/// </summary>
    public class DataProviderCommandBuilder
    {
		/// <summary>
		///		Tipo de parámetro
		/// </summary>
		public enum ParameterType
		{
			/// <summary>Cadena</summary>
			String,
			/// <summary>Entero</summary>
			Integer,
			/// <summary>Decimal</summary>
			Decimal,
			/// <summary>Fecha</summary>
			DateTime,
			/// <summary>Lógico</summary>
			Boolean
		}

		public DataProviderCommandBuilder(string sql)
		{
			Command = new CommandModel(sql);
		}

		/// <summary>
		///		Añade el parámetro
		/// </summary>
		public DataProviderCommandBuilder WithParameter(string key, object value)
		{
			// Añade el parámetro
			Add(Command.Parameters, key, value);
			// Devuelve el generador
			return this;
		}

		/// <summary>
		///		Añade el parámetro
		/// </summary>
		public DataProviderCommandBuilder WithParameter(string key, ParameterType type, string value, string defaultValue)
		{
			// Añade el parámetro
			Add(Command.Parameters, key, ConvertObject(type, value, defaultValue));
			// Devuelve el generador
			return this;
		}

		/// <summary>
		///		Genera el comando
		/// </summary>
		public CommandModel Build()
		{
			return Command;
		}

		/// <summary>
		///		Añade un elemento al diccionario
		/// </summary>
		private void Add<TypeData>(Dictionary<string, TypeData> dictionary, string key, TypeData value)
		{
			// Normaliza la clave
			key = NormalizeKey(key);
			// Añade / modifica el valor
			if (dictionary.ContainsKey(key))
				dictionary[key] = value;
			else
				dictionary.Add(key, value); 
		}

		/// <summary>
		///		Normaliza la clave
		/// </summary>
		private string NormalizeKey(string key)
		{
			return key.ToUpperInvariant();
		}

		/// <summary>
		///		Convierte una cadena a un objeto
		/// </summary>
		private object ConvertObject(ParameterType type, string value, string defaultValue)
		{
			if (string.IsNullOrEmpty(value))
				return defaultValue;
			else
				return value;
		}

		/// <summary>
		///		Comando
		/// </summary>
		private CommandModel Command { get; }
    }
}
