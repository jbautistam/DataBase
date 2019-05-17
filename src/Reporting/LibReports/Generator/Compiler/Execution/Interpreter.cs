using System;

using Bau.Libraries.LibTokenizer.Interpreter;
using Bau.Libraries.LibTokenizer.Lexical.Tokens;
using Bau.Libraries.LibTokenizer.Interpreter.Evaluator;
using Bau.Libraries.LibTokenizer.Interpreter.Instructions;
using Bau.Libraries.LibTokenizer.Variables;

namespace Bau.Libraries.LibReports.Generator.Compiler.Execution
{
	/// <summary>
	///		Clase para interpretación de una fórmula
	/// </summary>
	internal class Interpreter
	{ 
		// Constantes privadas
		private const int ScopeSymbolsLocal = 300;
		// Variables privadas
		private Lexical.Tokenizer _tokenizer = new Lexical.Tokenizer();
		private VariablesCollection _variablesGlobal = new VariablesCollection();

		/// <summary>
		///		Ejecuta el código de una fórmula
		/// </summary>
		internal void Execute(VariablesCollection symbolsLocal, string code)
		{
			Program program = ParseProgram(code);

				// Depuración
				#if DEBUG
					System.Diagnostics.Debug.WriteLine("Depuración del programa");
					program.Debug();
				#endif
				// Añade la pila de datos a la colección de variables
				AddStackData(symbolsLocal);
				// Ejecuta el programa
				ExecuteBlock(program.Sentences);
				// Quita la pila de datos de la colección de variables
				RemoveStackData(symbolsLocal);
		}

		/// <summary>
		///		Ejecuta un bloque de instrucciones
		/// </summary>
		private void ExecuteBlock(InstructionsBaseCollection sentences)
		{
			foreach (InstructionBase sentence in sentences)
				if (sentence.IsError)
					System.Diagnostics.Debug.WriteLine("");
				else if (sentence is InstructionLet)
					ExecuteInstructionLet(sentence as InstructionLet);
				else if (sentence is InstructionIf)
					ExecuteInstructionIf(sentence as InstructionIf);
				else if (sentence is InstructionWhile)
					ExecuteInstructionWhile(sentence as InstructionWhile);
				else if (sentence is InstructionFor)
					ExecuteInstructionFor(sentence as InstructionFor);
		}

		/// <summary>
		///		Añade la pila de datos a la colección de variables
		/// </summary>
		private void AddStackData(VariablesCollection symbolsLocal)
		{
			foreach (Variable symbol in symbolsLocal)
				_variablesGlobal.Add(symbol.Name, symbol.Value, 0, ScopeSymbolsLocal);
		}

		/// <summary>
		///		Elimina la pila de datos de la colección de variables
		/// </summary>
		private void RemoveStackData(VariablesCollection symbolsLocal)
		{
			foreach (Variable symbol in symbolsLocal)
				_variablesGlobal.RemoveAtScope(symbol.Name, ScopeSymbolsLocal);
		}

		/// <summary>
		///		Obtiene una tabla de símbolos a partir de las variables
		/// </summary>
		internal VariablesCollection GetSymbols()
		{
			return _variablesGlobal;
		}

		/// <summary>
		///		Interpreta un programa
		/// </summary>
		private Program ParseProgram(string code)
		{
			TokenCollection tokens = _tokenizer.Parse(code);

				// Depuración
				#if DEBUG
				tokens.Debug();
				#endif
				// Devuelve el programa generado
				return new Sintactical.ProgramGenerator().Generate(tokens);
		}

		/// <summary>
		///		Evalúa una condición
		/// </summary>
		internal bool EvaluateCondition(VariablesCollection symbolsLocal, string code)
		{
			TokenCollection tokens = _tokenizer.Parse(code);
			ExpressionsCollection expressions;
			InstructionError error = null;
			bool result = false;

				// Evalúa la expresión
				expressions = new Sintactical.ProgramGenerator().EvaluateExpression(tokens, error);
				// Si no hay ningún error, evalúa la expresión
				if (error == null)
				{
					Variable variable = new Variable("Condition");

						// Añade los símbolos a las variables
						AddStackData(symbolsLocal);
						// Evalúa la expresión
						variable.Value = new ExpressionCompute(_variablesGlobal).Evaluate(expressions);
						// Quita los símbolos de la tabla de variables
						RemoveStackData(symbolsLocal);
						// Comprueba el resultado
						if (variable.Value is ValueBool)
							result = (variable.Value as ValueBool).Value;
				}
				// Devuelve el valor de la condición
				return result;
		}

		/// <summary>
		///		Ejecuta una instrucción de asignación
		/// </summary>
		private void ExecuteInstructionLet(InstructionLet instruction)
		{
			Variable variable = _variablesGlobal.Search(instruction.Variable.Name);

				// Evalúa la expresión y la asigna a la variable
				variable.Value = new ExpressionCompute(_variablesGlobal).Evaluate(instruction.Expressions);
		}

		/// <summary>
		///		Ejecuta una instrucción if
		/// </summary>
		private void ExecuteInstructionIf(InstructionIf instruction)
		{
			if (CheckCondition(instruction.Condition))
				ExecuteBlock(instruction.SentencesIf);
			else
				ExecuteBlock(instruction.SentencesElse);
		}

		/// <summary>
		///		Ejecuta una instrucción While
		/// </summary>
		private void ExecuteInstructionWhile(InstructionWhile instruction)
		{
			while (CheckCondition(instruction.Condition))
				ExecuteBlock(instruction.Sentences);
		}

		/// <summary>
		///		Ejecuta una instrucción for
		/// </summary>
		private void ExecuteInstructionFor(InstructionFor instruction)
		{ 
			// Ejecuta la instrucción que inicializa el contador
			ExecuteInstructionLet(instruction.StartInstruction);
			// Ejecuta el bucle
			while (CheckCondition(instruction.Condition))
			{ 
				// Ejecuta las sentencias
				ExecuteBlock(instruction.Sentences);
				// Ejecuta el incremento
				ExecuteInstructionLet(instruction.IncrementInstruction);
			}
		}

		/// <summary>
		///		Comprueba una condición
		/// </summary>
		private bool CheckCondition(ExpressionsCollection expressions)
		{
			ExpressionCompute expressionEvaluator = new ExpressionCompute(_variablesGlobal);
			ValueBase result = expressionEvaluator.Evaluate(expressions);

				// Evalúa la condición
				if (result is ValueBool)
					return (result as ValueBool).Value;
				else
					return false;
		}
	}
}
