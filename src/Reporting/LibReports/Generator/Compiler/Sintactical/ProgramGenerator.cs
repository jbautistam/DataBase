using System;

using Bau.Libraries.LibTokenizer.Lexical.Tokens;
using Bau.Libraries.LibTokenizer.Interpreter;
using Bau.Libraries.LibTokenizer.Interpreter.Instructions;

namespace Bau.Libraries.LibReports.Generator.Compiler.Sintactical
{
	/// <summary>
	///		Clase del generador del programa
	/// </summary>
	internal class ProgramGenerator
	{
		/// <summary>
		///		Genera las instrucciones del programa a partir de una colección de tokens
		/// </summary>
		internal Program Generate(TokenCollection tokens)
		{
			Program program = new Program("main");
			bool isError = false;

				// Guarda la colección de tokens
				Tokens = tokens;
				// Interpreta la colección de tokens
				while (!IsEndProgram && !isError)
				{
					InstructionBase instruction;

						// Interpreta la instrucción						
						instruction = ParseInstruction();
						// Si la instrucción es un error, finaliza el programa
						if (instruction is InstructionError || instruction is InstructionEndProgram)
							isError = true;
						// Añade la instrucción en cualquier caso
						program.Sentences.Add(instruction);
				}
				// Devuelve el programa generado
				return program;
		}

		/// <summary>
		///		Evalúa una expresión
		/// </summary>
		internal ExpressionsCollection EvaluateExpression(TokenCollection tokens, InstructionError error)
		{ 
			// Añade un token de final de instrucción
			tokens.Add(new Token(Token.TokenType.EndInstruction, null, 0, 0, ";"));
			Tokens = tokens;
			// Evalúa la expresión
			return ParseExpression(error);
		}

		/// <summary>
		///		Interpreta una instrucción
		/// </summary>
		private InstructionBase ParseInstruction()
		{
			switch (ActualToken.Type)
			{
				case Token.TokenType.Comment:
					return ParseInstructionComment();
				case Token.TokenType.Variable:
					return ParseAssignInstruction();
				case Token.TokenType.ReservedWord:
					return ParseInstructionReserved();
				case Token.TokenType.Unknown:
				case Token.TokenType.EOF:
					return new InstructionEndProgram(ActualToken);
				case Token.TokenType.Error:
					return ParseError(ActualToken.GetDebugInfo());
				default:
					return new InstructionError(ActualToken, "Token desconocido");
			}
		}

		/// <summary>
		///		Interpreta un comentario
		/// </summary>
		private InstructionBase ParseInstructionComment()
		{
			InstructionComment instruction = new InstructionComment(ActualToken);

				// Pasa al siguiente token
				NextToken();
				// Devuelve la instrucción
				return instruction;
		}

		/// <summary>
		///		Interpreta una instrucción de asignación
		/// </summary>
		private InstructionLet ParseAssignInstruction(bool parseCondition = false)
		{
			InstructionLet instruction = new InstructionLet(ActualToken);
			InstructionError error = null;

				// Añade el nombre de la variable
				instruction.Variable = new ExpressionVariableIdentifier(ActualToken);
				instruction.Variable.Name = ActualToken.Value;
				// Pasa al siguiente token
				NextToken();
				// Añade la asignación de la variable
				if (ActualToken.Type == Token.TokenType.Equal)
				{ 
					// Pasa al siguiente token
					NextToken();
					// Interpreta la expresión
					if (parseCondition)
					{   
						// Inicializa la condición interpretando hasta el paréntesis derecho, no utiliza parseCondition porque no hay paréntesis de apertura
						instruction.Expressions = ParseExpression(error, new Token.TokenType[] { Token.TokenType.RightParentesis });
						// Comprueba que haya un paréntesis derecho
						if (ActualToken.Type == Token.TokenType.RightParentesis)
							NextToken();
						else
							error = ParseError("No se encuentra el paréntesis derecho");
					}
					else
						instruction.Expressions = ParseExpression(error);
				}
				else
					error = ParseError("Falta el signo igual");
				// Asigna el error
				instruction.Error = error;
				// Devuelve la instrucción
				return instruction;
		}

		/// <summary>
		///		Interpreta una instrucción de palabra reservada
		/// </summary>
		private InstructionBase ParseInstructionReserved()
		{
			switch (ActualToken.Value)
			{
				case CompilerConstants.IfSentence:
					return ParseInstructionIf();
				case CompilerConstants.ForSentence:
					return ParseInstructionFor();
				case CompilerConstants.WhileSentence:
					return ParseIntructionWhile();
				default:
					return ParseError("Palabra reservada desconocida");
			}
		}

		/// <summary>
		///		Interpreta la instrucción if
		/// </summary>
		private InstructionIf ParseInstructionIf()
		{
			InstructionIf instruction = new InstructionIf(ActualToken);
			InstructionError error = null;

				// Pasa al siguiente token
				NextToken();
				// Interpreta la condición
				instruction.Condition = ParseCondition(error);
				// Si no hay error ...
				if (error == null)
				{ 
					// Interpreta el bloque if
					instruction.SentencesIf = ParseBlock(error);
					// Interpreta la condición else
					if (error == null)
						if (ActualToken.Type == Token.TokenType.ReservedWord && ActualToken.Value == CompilerConstants.ElseSentence)
						{ 
							// Pasa al siguiente token
							NextToken();
							// Interpreta las sentencias el bloque else
							instruction.SentencesElse = ParseBlock(error);
						}
				}
				// Asigna el error
				instruction.Error = error;
				// Devuelve la instrucción
				return instruction;
		}

		/// <summary>
		///		Interpreta una instrucción for
		/// </summary>
		private InstructionFor ParseInstructionFor()
		{
			InstructionFor instruction = new InstructionFor(ActualToken);
			InstructionError error = null;

				// Pasa al siguiente token
				NextToken();
				// Si es un paréntesis de apertura ...
				if (ActualToken.Type == Token.TokenType.LeftParentesis)
				{ 
					// Pasa al siguiente token
					NextToken();
					// Obtiene el inicializador de la variable
					if (ActualToken.Type == Token.TokenType.Variable)
					{ 
						// Asigna la instrucción de inicio
						instruction.StartInstruction = ParseAssignInstruction(false);
						// Si no hay error ...
						if (instruction.StartInstruction.IsError)
							error = instruction.StartInstruction.Error;
						else // ... no se comprueba el punto y coma del fin de inicialización porque el ParseAssignInstruction ya lo ha quitado
						{ 
							// Obtiene la condición
							instruction.Condition = ParseExpression(error);
							// Si no hay error, se recoge el incremento y las sentencias
							if (error == null)
							{
								if (instruction.Condition.Count == 0)
									error = ParseError("No se encuentran las condiciones de cierre del bucle for");
								else if (ActualToken.Type == Token.TokenType.Variable)
								{ 
									// Obtiene la expresión de incremento
									instruction.IncrementInstruction = ParseAssignInstruction(true);
									// Si no hay error, comprueba el paréntesis de cierre
									if (instruction.IncrementInstruction.IsError)
										error = instruction.IncrementInstruction.Error;
									else // ... no se comprueba el paréntesis de cierre por el ParseAssignInstruction ya lo ha quitado y no tiene errores
										instruction.Sentences = ParseBlock(error);
								}
								else
									error = ParseError("No se encuentra el incremento de variable en el bucle for");
							}
						}
					}
					else
						error = ParseError("Falta la expresión de inicio");
				}
				else
					error = ParseError("Falta el paréntesis de apertura de la sentencia for");
				// Asigna el error
				instruction.Error = error;
				// Devuelve la instrucción
				return instruction;
		}

		/// <summary>
		///		Interpreta una instrucción While
		/// </summary>
		private InstructionWhile ParseIntructionWhile()
		{
			InstructionWhile instruction = new InstructionWhile(ActualToken);
			InstructionError error = null;

				// Pasa al siguiente token	
				NextToken();
				// Interpreta la condición
				instruction.Condition = ParseCondition(error);
				// Si no hay error
				if (error == null)
					instruction.Sentences = ParseBlock(error);
				// Asigna el error
				instruction.Error = error;
				// Devuelve la instrucción
				return instruction;
		}

		/// <summary>
		///		Interpreta una serie de instrucciones
		/// </summary>
		private InstructionsBaseCollection ParseBlock(InstructionError error)
		{
			InstructionsBaseCollection instructions = new InstructionsBaseCollection();

				// Interpreta el bloque
				if (ActualToken.Type == Token.TokenType.StartBlock)
				{
					InstructionBase instruction;

						// Siguiente token
						NextToken();
						// Interpreta la instrucción
						while (!IsEndProgram && error == null && ActualToken.Type != Token.TokenType.EndBlock)
						{   
							// Interpreta la instrucción
							instruction = ParseInstruction();
							// Si es un error, termina, si no añade la instrucción
							if (instruction is InstructionError)
							{
								error = ParseError((instruction as InstructionError).ErrorDescription);
								instructions.Add(instruction);
							}
							else
								instructions.Add(instruction);
						}
						// Si es un fin de bloque ...
						if (ActualToken.Type == Token.TokenType.EndBlock)
							NextToken();
						else
							error = ParseError("No se encuentra el token de fin de bloque");
				}
				else
					instructions.Add(ParseInstruction());
				// Devuelve la colección de instrucciones
				return instructions;
		}

		/// <summary>
		///		Interpreta un error
		/// </summary>
		private InstructionError ParseError(string error)
		{
			return new InstructionError(ActualToken, error);
		}

		/// <summary>
		///		Interpreta una expresión
		/// </summary>
		private ExpressionsCollection ParseExpression(InstructionError error)
		{
			ExpressionsCollection expressions = new ExpressionsCollection();

				// Inicializa la instrucción de error
				error = null;
				// Interpreta los tokens
				expressions = ParseExpression(error, new Token.TokenType[] { Token.TokenType.EndInstruction } );
				// Comprueba si se ha leído alguna expresión
				if (expressions == null)
					error = ParseError("No se ha leído ninguna expresión");
				else if (IsEndInstruction())
					NextToken();
				// Devuelve la colección de expresiones
				return expressions;
		}

		/// <summary>
		///		Interpreta una condición
		/// </summary>
		private ExpressionsCollection ParseCondition(InstructionError error)
		{
			ExpressionsCollection expressions = new ExpressionsCollection();

				// Interpeta la condición
				if (ActualToken.Type == Token.TokenType.LeftParentesis)
				{ 
					// Pasa al siguiente token
					NextToken();
					// Interpreta la colección de expresiones hasta el cierre del paréntesis
					expressions = ParseExpression(error, new Token.TokenType[] { Token.TokenType.RightParentesis } );
					// Comprueba el paréntesis derecho
					if (ActualToken.Type == Token.TokenType.RightParentesis)
						NextToken();
					else
						error = ParseError("Falta el paréntesis de cierre de la condición");
				}
				else
					error = ParseError("Falta el paréntesis de apertura de la condición");
				// Devuelve la colección de expresiones
				return expressions;
		}

		/// <summary>
		///		Interpreta una expresión hasta encontrar un token de cierre
		/// </summary>
		private ExpressionsCollection ParseExpression(InstructionError error, Token.TokenType[] tokensTypeEnd)
		{
			ExpressionsCollection expressions = new ExpressionsCollection();
			int parenthesisOpen = 0;
			bool isEnd = false;

				// Interpreta las expresiones
				while (!isEnd && error == null)
				{ 
					// Añade el token actual a la colección de expresiones
					switch (ActualToken.Type)
					{
						case Token.TokenType.ArithmeticOperator:
						case Token.TokenType.Variable:
						case Token.TokenType.LeftParentesis:
						case Token.TokenType.RightParentesis:
						case Token.TokenType.LogicalOperator:
						case Token.TokenType.RelationalOperator:
						case Token.TokenType.Number:
						case Token.TokenType.String:
								expressions.Add(new ExpressionBase(ActualToken));
								if (ActualToken.Type == Token.TokenType.LeftParentesis)
									parenthesisOpen++;
								else if (ActualToken.Type == Token.TokenType.RightParentesis)
									parenthesisOpen--;
							break;
						default:
								error = ParseError("Tipo de expresión desconocido");
							break;
					}
					// Pasa al siguiente token
					NextToken();
					// Comprueba si es el final de la expresión
					isEnd = IsEndExpression(tokensTypeEnd, parenthesisOpen);
				}
				// Devuelve la colección de expresiones
				return expressions;
		}

		/// <summary>
		///		Interpreta el fin de una expresión
		/// </summary>
		private bool IsEndExpression(Token.TokenType[] tokensTypeEnd, int intParenthesisOpen)
		{
			bool isEnd = false;

				// Comprueba si es el final de la expresión
				if (IsEndProgram)
					isEnd = true;
				else
					foreach (Token.TokenType type in tokensTypeEnd)
						if (type == Token.TokenType.RightParentesis)
						{
							if (ActualToken.Type == Token.TokenType.RightParentesis && intParenthesisOpen == 0)
								isEnd = true;
						}
						else if (type == ActualToken.Type)
							isEnd = true;
				// Si ha llegado hasta aquí es porque no es el final de la expresión
				return isEnd;
		}

		/// <summary>
		///		Pasa al siguiente token
		/// </summary>
		private void NextToken()
		{
			ActualTokenIndex++;
		}

		/// <summary>
		///		Obtiene el siguiente token
		/// </summary>
		private Token GetNextToken()
		{
			if (ActualTokenIndex < Tokens.Count)
				return Tokens[ActualTokenIndex + 1];
			else
				return new Token(Token.TokenType.EOF, null, 0, 0, "EOF");
		}

		/// <summary>
		///		Comprueba si el token actual finaliza una instrucción
		/// </summary>
		private bool IsEndInstruction()
		{
			return ActualToken.Type == Token.TokenType.EndInstruction;
		}

		/// <summary>
		///		Tokens a analizar
		/// </summary>
		private TokenCollection Tokens { get; set; }

		/// <summary>
		///		Token que se está analizando
		/// </summary>
		private int ActualTokenIndex { get; set; } = 0;

		/// <summary>
		///		Token actual
		/// </summary>
		private Token ActualToken
		{
			get
			{
				if (ActualTokenIndex < Tokens.Count)
					return Tokens[ActualTokenIndex];
				else
					return new Token(Token.TokenType.EOF, null, 0, 0, "Fin de programa");
			}
		}

		/// <summary>
		///		Indica si es el final del programa
		/// </summary>
		private bool IsEndProgram
		{
			get { return ActualTokenIndex >= Tokens.Count - 1; }
		}
	}
}