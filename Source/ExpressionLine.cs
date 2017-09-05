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

namespace GDA.Sql.InterpreterExpression
{
	class ExpressionLine
	{
		private List<Expression> _expressions = new List<Expression>();

		/// <summary>
		/// Ponto inicial da expressão.
		/// </summary>
		private int beginPoint;

		/// <summary>
		/// Ponto final da expressão.
		/// </summary>
		private int length;

		/// <summary>
		/// Ponto inicial da expressão.
		/// </summary>
		public int BeginPoint
		{
			get
			{
				return beginPoint;
			}
			internal set
			{
				beginPoint = value;
			}
		}

		/// <summary>
		/// Ponto final da expressão.
		/// </summary>
		public int Length
		{
			get
			{
				return length;
			}
			internal set
			{
				length = value;
			}
		}

		/// <summary>
		/// Expressões da linha.
		/// </summary>
		public List<Expression> Expressions
		{
			get
			{
				return _expressions;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="beginPoint"></param>
		public ExpressionLine(int beginPoint)
		{
			this.beginPoint = beginPoint;
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			StringBuilder s = new StringBuilder();
			foreach (Expression ex in _expressions)
				s.Append(ex.ToString()).Append(" ");
			return s.ToString();
		}
	}
}
