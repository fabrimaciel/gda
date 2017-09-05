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
	class ContainerExpression
	{
		/// <summary>
		/// Caracter container da expressão.
		/// </summary>
		private char containerChar;

		/// <summary>
		/// Caracter de fim do container
		/// </summary>
		private char endContainerChar;

		/// <summary>
		/// Expressões contidas.
		/// </summary>
		private ExpressionLine _line;

		private int _beginPos = 0;

		/// <summary>
		/// Caracter container da expressão.
		/// </summary>
		public char ContainerChar
		{
			get
			{
				return containerChar;
			}
			set
			{
				containerChar = value;
				switch(value)
				{
				case '(':
					endContainerChar = ')';
					break;
				case '[':
					endContainerChar = ']';
					break;
				case '{':
					endContainerChar = '}';
					break;
				default:
					throw new Exception("AD");
				}
			}
		}

		/// <summary>
		/// Caracter de fim do container
		/// </summary>
		public char EndContainerChar
		{
			get
			{
				return endContainerChar;
			}
		}

		public ExpressionLine Line
		{
			get
			{
				return _line;
			}
			set
			{
				_line = value;
			}
		}

		public int BeginPos
		{
			get
			{
				return _beginPos;
			}
			set
			{
				_beginPos = value;
			}
		}

		/// <summary>
		/// Expressões contidas.
		/// </summary>
		public ContainerExpression(int beginPos, char character, ExpressionLine line)
		{
			_beginPos = beginPos;
			ContainerChar = character;
			_line = line;
		}
	}
}
