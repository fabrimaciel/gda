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
using GDA.Sql.InterpreterExpression.Enums;

namespace GDA.Sql.InterpreterExpression.Nodes
{
	/// <summary>
	/// Representa uma expressão Sql.
	/// </summary>
	class SqlExpression
	{
		private Expression _value;

		private SqlExpressionType _type;

		/// <summary>
		/// Valor da expressão.
		/// </summary>
		public Expression Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}

		/// <summary>
		/// Tipo da expressão.
		/// </summary>
		public SqlExpressionType Type
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="expression">Expressão relacionadas</param>
		internal SqlExpression(Expression expression)
		{
			switch(expression.Token)
			{
			case TokenID.Identifier:
				if(expression.Text[0] == '?' || expression.Text[0] == '@')
					_type = SqlExpressionType.Variable;
				else
					_type = SqlExpressionType.Column;
				break;
			case TokenID.IntLiteral:
			case TokenID.DecimalLiteral:
			case TokenID.RealLiteral:
				_type = SqlExpressionType.NumericLiteral;
				break;
			case TokenID.StringLiteral:
				_type = SqlExpressionType.StringLiteral;
				break;
			case TokenID.Star:
			case TokenID.Plus:
			case TokenID.Slash:
			case TokenID.Minus:
			case TokenID.kLike:
			case TokenID.kBetween:
				_type = SqlExpressionType.Operation;
				break;
			case TokenID.EqualEqual:
			case TokenID.Equal:
			case TokenID.Greater:
			case TokenID.GreaterEqual:
			case TokenID.Less:
			case TokenID.LessEqual:
			case TokenID.NotEqual:
			case TokenID.kNot:
			case TokenID.kIs:
			case TokenID.kIsNull:
				_type = SqlExpressionType.Comparation;
				break;
			case TokenID.kSelect:
				_type = SqlExpressionType.Select;
				break;
			case TokenID.kAnd:
			case TokenID.kOr:
				_type = SqlExpressionType.Boolean;
				break;
			case TokenID.kIn:
			case TokenID.kAny:
			case TokenID.kSome:
			case TokenID.kAll:
				_type = SqlExpressionType.ComparerScalar;
				break;
			default:
				_type = SqlExpressionType.Column;
				break;
			}
			_value = expression;
		}

		/// <summary>
		/// Construtor completo.
		/// </summary>
		/// <param name="value">Expressão.</param>
		/// <param name="type">Tipo da expressão.</param>
		public SqlExpression(Expression value, SqlExpressionType type)
		{
			_type = type;
			if(type == SqlExpressionType.Column && value.Length > 0 && (value.Text[0] == '?' || value.Text[0] == '@'))
				_type = SqlExpressionType.Variable;
			_value = value;
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return _value.Text;
		}
	}
}
