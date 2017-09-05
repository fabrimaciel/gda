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

namespace GDA.Sql.InterpreterExpression
{
	class SpecialContainerExpression : Expression
	{
		/// <summary>
		/// Caracter container da expressão.
		/// </summary>
		private char containerChar;

		/// <summary>
		/// Caracter container da expressão.
		/// </summary>
		public char ContainerChar
		{
			get
			{
				return containerChar;
			}
		}

		private TokenID _containerToken;

		public TokenID ContainerToken
		{
			get
			{
				return _containerToken;
			}
			set
			{
				_containerToken = value;
			}
		}

		public SpecialContainerExpression(int beginPoint, int length, ExpressionLine line, string command, char containerChar) : base(beginPoint, length, line, command)
		{
			this.Token = TokenID.StringLiteral;
			this.containerChar = containerChar;
		}

		public override string ToString()
		{
			return containerChar + base.Text + containerChar;
		}
	}
}
