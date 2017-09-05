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

namespace GDA.Sql.InterpreterExpression.Grammar
{
	enum ExpressionComparerType
	{
		Simple,
		Regex
	}
	class ExpressionComparer
	{
		/// <summary>
		/// Tipo de comparação a ser feita.
		/// </summary>
		private ExpressionComparerType _type = ExpressionComparerType.Simple;

		/// <summary>
		/// Texto usado na comparação.
		/// </summary>
		private string _textComparer;

		/// <summary>
		/// Identifica se a comparação é case sensitive.
		/// </summary>
		private bool _caseSensitive = false;

		/// <summary>
		/// Número de expressões anteriores que são usadas na comparação.
		/// </summary>
		private uint _numberPreviousExpressions;

		/// <summary>
		/// Número de expressões próximas que são usadas na comparação.
		/// </summary>
		private uint _numberNextExpressions;

		/// <summary>
		/// Tipo de comparação a ser feita.
		/// </summary>
		public ExpressionComparerType Type
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
		/// Texto usado na comparação.
		/// </summary>
		public string TextComparer
		{
			get
			{
				return _textComparer;
			}
			set
			{
				_textComparer = value;
			}
		}

		/// <summary>
		/// Identifica se a comparação é case sensitive.
		/// </summary>
		public bool CaseSensitive
		{
			get
			{
				return _caseSensitive;
			}
			set
			{
				_caseSensitive = value;
			}
		}

		/// <summary>
		/// Número de expressões anteriores que são usadas na comparação.
		/// </summary>
		public uint NumberPreviousExpressions
		{
			get
			{
				return _numberPreviousExpressions;
			}
			set
			{
				_numberPreviousExpressions = value;
			}
		}

		/// <summary>
		/// Número de expressões próximas que são usadas na comparação.
		/// </summary>
		public uint NumberNextExpressions
		{
			get
			{
				return _numberNextExpressions;
			}
			set
			{
				_numberNextExpressions = value;
			}
		}

		/// <summary>
		/// Número de expressões que o comparador saltou.
		/// </summary>
		public uint JumpNextExpressions
		{
			get
			{
				return 1 + _numberNextExpressions;
			}
		}

		public ExpressionComparer(string text) : this(text, ExpressionComparerType.Simple, false)
		{
		}

		public ExpressionComparer(string text, ExpressionComparerType type, bool caseSensitive)
		{
			_textComparer = text;
			_type = type;
			_caseSensitive = caseSensitive;
		}

		public bool Comparer(Expression expr)
		{
			return false;
		}
	}
}
