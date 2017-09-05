/* 
 * GDA - Generics Data Access, is framework to object-relational mapping 
 * (a programming technique for converting data between incompatible 
 * type systems in databases and Object-oriented programming languages) using c#.
 * 
 * Copyright (C) 2010  <http://www.colosoft.com.br/gda> - support@colosoft.com.br
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Text;
using GDA.Sql.InterpreterExpression;
using System.Reflection;

namespace GDA.Sql
{
	/// <summary>
	/// Classe responsável por construir o comando sql.
	/// </summary>
	public static class SqlBuilder
	{
		internal static string SimbolOperator(Operator op)
		{
			switch(op)
			{
			case Operator.Equals:
				return "=";
			case Operator.GreaterThan:
				return ">";
			case Operator.GreaterThanOrEquals:
				return ">=";
			case Operator.In:
				return " IN ";
			case Operator.LessThan:
				return "<";
			case Operator.LessThanOrEquals:
				return "<=";
			case Operator.Like:
				return " LIKE ";
			case Operator.NotEquals:
				return "<>";
			case Operator.NotIn:
				return " NOT IN ";
			case Operator.NotLike:
				return " NOT LIKE ";
			default:
				return "";
			}
		}

		/// <summary>
		/// Importa um namespace para ser trabalhado pelo SqlBuilder.
		/// </summary>
		/// <param name="assembly">Assembly onde o namespace está introduzido.</param>
		/// <param name="namesSpace">Namespace importado.</param>
		public static void ImportNamespace(Assembly assembly, string namesSpace)
		{
			GDASettings.AddModelsNamespace(assembly, namesSpace);
		}

		/// <summary>
		/// Recupera uma consulta para a tradução do comando passsado.
		/// </summary>
		/// <param name="command">Comando que sera traduzido.</param>
		/// <returns></returns>
		public static SelectStatement P(ISelectStatementReferences references, string command)
		{
			if(references == null)
				throw new ArgumentNullException("references");
			Lexer lex = new Lexer(command);
			Parser parser = new Parser(lex);
			parser.Execute();
			return new SelectStatement(references, parser);
		}

		/// <summary>
		/// Recupera uma consulta para a tradução do comando passsado.
		/// </summary>
		/// <param name="command">Comando que sera traduzido.</param>
		/// <returns></returns>
		public static SelectStatement P(string command)
		{
			Lexer lex = new Lexer(command);
			Parser parser = new Parser(lex);
			parser.Execute();
			return new SelectStatement(parser);
		}
	}
}
