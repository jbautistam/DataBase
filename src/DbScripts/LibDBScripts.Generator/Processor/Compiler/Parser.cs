using System;

using Bau.Libraries.LibCommonHelper.Extensors;
using Bau.Libraries.Compiler.LibTokenizer;
using Bau.Libraries.Compiler.LibTokenizer.Lexical.Tokens;
using Bau.Libraries.Compiler.LibTokenizer.Lexical.Rules;
using Bau.Libraries.Compiler.LibInterpreter.Expressions;

namespace Bau.Libraries.LibDbScripts.Generator.Processor.Compiler
{
	/// <summary>
	///		Clase del generador del programa
	/// </summary>
	internal class Parser
	{
		// Constantes para la definición de sentencias
		private const string AndOperator = "&&";
		private const string OrOperator = "||";
		private const string NotOperator = "!";
		/// <summary>
		///		Subtipo del token
		/// </summary>
		internal enum TokenSubType
		{
			/// <summary>Fecha</summary>
			Date,
			/// <summary>Nombre de variable (entre {{ y }})</summary>
			Variable
		}

		/// <summary>
		///		Obtiene las expresiones de un código
		/// </summary>
		internal ExpressionsCollection Parse(string code, out string error)
		{
			// Obtiene los tokens del código
			Tokens = Parse(code);
			// Añade un token de fin de instrucción
			Tokens.Add(new Token(Token.TokenType.EndInstruction, null, 0, 0, ";"));
			// Interpreta las expresiones
			return ParseExpression(new Token.TokenType[] { Token.TokenType.EndInstruction }, out error);
		}

		/// <summary>
		///		Interpreta una cadena
		/// </summary>
		private TokenCollection Parse(string content)
		{
			TokenizerManager tokenizer = new TokenizerManager();

				// Añade las reglas de lectura
				tokenizer.Rules.Add(new RulePattern(Token.TokenType.Variable, null,
													"A", "A9_")); // ... definición de variables
				tokenizer.Rules.Add(new RuleDelimited(Token.TokenType.String, null,
													  new string[] { "\"" }, new string[] { "\"" },
													  false, false, false, false)); // ... cadenas
				tokenizer.Rules.Add(new RuleDelimited(Token.TokenType.UserDefined, (int) TokenSubType.Date,
													  new string[] { "#" }, new string[] { "#" },
													  false, false, false, false)); // ... fechas
				tokenizer.Rules.Add(new RuleDelimited(Token.TokenType.UserDefined, (int) TokenSubType.Variable,
													  new string[] { "{{" }, new string[] { "}}" },
													  false, false, false, false)); // ... fechas
				tokenizer.Rules.Add(new RulePattern(Token.TokenType.Number, null,
													"9", "9.")); // ... definición de números
				tokenizer.Rules.Add(new RuleWordFixed(Token.TokenType.ArithmeticOperator, null,
													  new string[] { "+", "-", "*", "/", "%" }));
				tokenizer.Rules.Add(new RuleWordFixed(Token.TokenType.LogicalOperator, null,
													  new string[] { "<", ">", ">=", "<=", "==", "!=" }));
				tokenizer.Rules.Add(new RuleWordFixed(Token.TokenType.RelationalOperator, null,
													  new string[] 
														{ 
															OrOperator,
															AndOperator,
															NotOperator
														}
													  ));
				// tokenizer.Rules.Add(new RuleWordFixed(Token.TokenType.Equal, null, new string[] { "=" }));
				tokenizer.Rules.Add(new RuleWordFixed(Token.TokenType.LeftParentesis, null, new string[] { "(" }));
				tokenizer.Rules.Add(new RuleWordFixed(Token.TokenType.RightParentesis, null, new string[] { ")" }));
				// tokenizer.Rules.Add(new RuleWordFixed(Token.TokenType.EndInstruction, null, new string[] { ";" }));
				// tokenizer.Rules.Add(new RuleWordFixed(Token.TokenType.StartBlock, null, new string[] { "{" }));
				// tokenizer.Rules.Add(new RuleWordFixed(Token.TokenType.EndBlock, null, new string[] { "}" }));
				// Obtiene los tokens
				return tokenizer.Parse(content);
		}

		/// <summary>
		///		Interpreta una expresión hasta encontrar un token de cierre
		/// </summary>
		private ExpressionsCollection ParseExpression(Token.TokenType[] tokensTypeEnd, out string error)
		{
			ExpressionsCollection expressions = new ExpressionsCollection();
			int parenthesisOpen = 0;
			bool isEnd = false;

				// Inicializa los argumentos de salida
				error = string.Empty;
				// Interpreta las expresiones
				while (!isEnd && string.IsNullOrEmpty(error))
				{ 
					// Añade el token actual a la colección de expresiones
					switch (ActualToken.Type)
					{
						case Token.TokenType.ArithmeticOperator:
								expressions.Add(GetExpressionMath(ActualToken.Value));
							break;
						case Token.TokenType.LeftParentesis:
						case Token.TokenType.RightParentesis:
								// Añade la expresión del paréntesis
								expressions.Add(new ExpressionParenthesis(ActualToken.Type == Token.TokenType.LeftParentesis));
								// Añade o quita el paréntesis del contador
								if (ActualToken.Type == Token.TokenType.LeftParentesis)
									parenthesisOpen++;
								else if (ActualToken.Type == Token.TokenType.RightParentesis)
									parenthesisOpen--;
							break;
						case Token.TokenType.LogicalOperator:
								expressions.Add(GetExpressionLogical(ActualToken.Value));
							break;
						case Token.TokenType.RelationalOperator:
								expressions.Add(GetExpressionRelation(ActualToken.Value));
							break;
						case Token.TokenType.Number:
								expressions.Add(new ExpressionConstant(Libraries.Compiler.LibInterpreter.Context.Variables.VariableModel.VariableType.Numeric, 
																	   ActualToken.Value.GetDouble(0)));
							break;
						case Token.TokenType.String:
								expressions.Add(new ExpressionConstant(Libraries.Compiler.LibInterpreter.Context.Variables.VariableModel.VariableType.String,
																	   ActualToken.Value));
							break;
						case Token.TokenType.UserDefined:
								switch ((TokenSubType) (ActualToken.SubType ?? 2000))
								{
									case TokenSubType.Date:
											expressions.Add(new ExpressionConstant(Libraries.Compiler.LibInterpreter.Context.Variables.VariableModel.VariableType.Date,
																				   ActualToken.Value.GetDateTime()));
										break;
									case TokenSubType.Variable:
											expressions.Add(new ExpressionVariableIdentifier(ActualToken.Value));
										break;
									default:
											expressions.Add(new ExpressionError("Tipo de token desconocido"));
										break;
								}
							break;
						case Token.TokenType.Variable:
								expressions.Add(new ExpressionVariableIdentifier(ActualToken.Value));
							break;
						default:
								expressions.Add(new ExpressionError("Tipo de expresión desconocido"));
							break;
					}
					// Si la última expresión es un error, se detiene
					if (expressions.Count > 0 && expressions[0] is ExpressionError expression)
						error = expression.Message;
					else
					{
						// Pasa al siguiente token
						NextToken();
						// Comprueba si es el final de la expresión
						isEnd = IsEndExpression(tokensTypeEnd, parenthesisOpen);
					}
				}
				// Devuelve la colección de expresiones
				return expressions;
		}

		/// <summary>
		///		Obtiene una expresión matemática
		/// </summary>
		private ExpressionBase GetExpressionMath(string operation)
		{
			switch (operation)
			{
				case "+":
					return new ExpressionOperatorMath(ExpressionOperatorMath.MathType.Sum);
				case "-":
					return new ExpressionOperatorMath(ExpressionOperatorMath.MathType.Substract);
				case "*":
					return new ExpressionOperatorMath(ExpressionOperatorMath.MathType.Multiply);
				case "/": 
					return new ExpressionOperatorMath(ExpressionOperatorMath.MathType.Divide);
				case "%":
					return new ExpressionOperatorMath(ExpressionOperatorMath.MathType.Modulus);
				default:
					return new ExpressionError($"No se reconoce el operador {operation}");
			}
		}

		/// <summary>
		///		Obtiene una expresión lógica
		/// </summary>
		private ExpressionBase GetExpressionLogical(string operation)
		{
			switch (operation)
			{
				case "<":
					return new ExpressionOperatorLogical(ExpressionOperatorLogical.LogicalType.Less);
				case ">":
					return new ExpressionOperatorLogical(ExpressionOperatorLogical.LogicalType.Greater);
				case ">=":
					return new ExpressionOperatorLogical(ExpressionOperatorLogical.LogicalType.GreaterOrEqual);
				case "<=":
					return new ExpressionOperatorLogical(ExpressionOperatorLogical.LogicalType.LessOrEqual);
				case "==":
					return new ExpressionOperatorLogical(ExpressionOperatorLogical.LogicalType.Equal);
				case "!=":
					return new ExpressionOperatorLogical(ExpressionOperatorLogical.LogicalType.Distinct);
				default:
					return new ExpressionError($"No se reconoce el operador {operation}");
			}
		}

		/// <summary>
		///		Obtiene una expresión de relación
		/// </summary>
		private ExpressionBase GetExpressionRelation(string operation)
		{
			switch (operation)
			{
				case OrOperator:
					return new ExpressionOperatorRelational(ExpressionOperatorRelational.RelationalType.Or);
				case AndOperator:
					return new ExpressionOperatorRelational(ExpressionOperatorRelational.RelationalType.And);
				case NotOperator:
					return new ExpressionOperatorRelational(ExpressionOperatorRelational.RelationalType.Not);
				default:
					return new ExpressionError($"No se reconoce el operador {operation}");
			}
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