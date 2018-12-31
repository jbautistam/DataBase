using System;

using Bau.Libraries.LibTokenizer;
using Bau.Libraries.LibTokenizer.Lexical.Rules;
using Bau.Libraries.LibTokenizer.Lexical.Tokens;

namespace Bau.Libraries.LibReports.Generator.Compiler.Lexical
{
	/// <summary>
	///		Intérprete de una cadena de fórmulas
	/// </summary>
	internal class Tokenizer
	{
		/// <summary>
		///		Interpreta una cadena
		/// </summary>
		internal TokenCollection Parse(string content)
		{
			TokenizerManager tokenizer = new TokenizerManager();

				// Añade las reglas de lectura
				tokenizer.Rules.Add(new RuleDelimited(Token.TokenType.Comment, null,
													  new string[] { "/*" }, new string[] { "*/" },
													  false, false, false, false)); // ... comentarios en bloque
				tokenizer.Rules.Add(new RuleDelimited(Token.TokenType.Comment, null,
													  new string[] { "//", "_" }, null,
													  true, false, false, false)); // ... comentarios
				tokenizer.Rules.Add(new RuleWord(Token.TokenType.ReservedWord, null,
												 new string[] 
													{ 
														CompilerConstants.IfSentence,
														CompilerConstants.ElseSentence,
														CompilerConstants.ForSentence,
														CompilerConstants.ToSentence,
														CompilerConstants.StepSentence,
														CompilerConstants.WhileSentence
													},
												 null, true)); // ... palabras reservadas
				tokenizer.Rules.Add(new RulePattern(Token.TokenType.Variable, null,
													"A", "A9_")); // ... definición de variables
				tokenizer.Rules.Add(new RuleDelimited(Token.TokenType.String, null,
													  new string[] { "\"" }, new string[] { "\"" },
													  false, false, false, false)); // ... cadenas
				tokenizer.Rules.Add(new RulePattern(Token.TokenType.Number, null,
													"9", "9.")); // ... definición de números
				tokenizer.Rules.Add(new RuleWordFixed(Token.TokenType.ArithmeticOperator, null,
													  new string[] { "+", "-", "*", "/", "%" }));
				tokenizer.Rules.Add(new RuleWordFixed(Token.TokenType.LogicalOperator, null,
													  new string[] { "<", ">", ">=", "<=", "==", "!=" }));
				tokenizer.Rules.Add(new RuleWordFixed(Token.TokenType.RelationalOperator, null,
													  new string[] 
														{ 
															CompilerConstants.OrOperator,
															CompilerConstants.AndOperator,
															CompilerConstants.NotOperator
														}
													  ));
				tokenizer.Rules.Add(new RuleWordFixed(Token.TokenType.Equal, null, new string[] { "=" }));
				tokenizer.Rules.Add(new RuleWordFixed(Token.TokenType.LeftParentesis, null, new string[] { "(" }));
				tokenizer.Rules.Add(new RuleWordFixed(Token.TokenType.RightParentesis, null, new string[] { ")" }));
				tokenizer.Rules.Add(new RuleWordFixed(Token.TokenType.EndInstruction, null, new string[] { ";" }));
				tokenizer.Rules.Add(new RuleWordFixed(Token.TokenType.StartBlock, null, new string[] { "{" }));
				tokenizer.Rules.Add(new RuleWordFixed(Token.TokenType.EndBlock, null, new string[] { "}" }));
				// Obtiene los tokens
				return tokenizer.Parse(content);
		}
	}
}
