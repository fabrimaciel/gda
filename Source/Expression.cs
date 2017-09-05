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
	class SpecialContainer
	{
		/// <summary>
		/// Caracter inicial do container aonde a expressão está contida.
		/// </summary>
		public readonly char BeginCharSpecialContainer;

		/// <summary>
		/// Caracter final do container aonde a expressão está contida.
		/// </summary>
		public readonly char EndCharSpecialContainer;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="beginChar"></param>
		/// <param name="endChar"></param>
		public SpecialContainer(char beginChar, char endChar)
		{
			BeginCharSpecialContainer = beginChar;
			EndCharSpecialContainer = endChar;
		}
	}
	/// <summary>
	/// Representa um expressao.
	/// </summary>
	class Expression
	{
		/// <summary>
		/// Ponto inicial da expressão.
		/// </summary>
		private int beginPoint;

		/// <summary>
		/// Ponto final da expressão.
		/// </summary>
		private int length;

		/// <summary>
		/// Texto da expressão.
		/// </summary>
		private string text;

		/// <summary>
		/// Linha que a expressão está contida.
		/// </summary>
		private ExpressionLine line;

		/// <summary>
		/// Container aonde a expressão está contida.
		/// </summary>
		private ContainerExpression container;

		/// <summary>
		/// Posição da expressão no container.
		/// </summary>
		private int containerPosition;

		/// <summary>
		/// Token relacionado a expressão.
		/// </summary>
		private TokenID _token;

		/// <summary>
		/// Container especial que a expressão está contida.
		/// </summary>
		private SpecialContainer _currentSpecialContainer = null;

		/// <summary>
		/// Ponto inicial da expressão.
		/// </summary>
		public int BeginPoint
		{
			get
			{
				return beginPoint;
			}
			set
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
			set
			{
				length = value;
			}
		}

		/// <summary>
		/// Texto da expressão.
		/// </summary>
		public string Text
		{
			get
			{
				return text;
			}
			set
			{
				text = value;
			}
		}

		/// <summary>
		/// Linha que a expressão está contida.
		/// </summary>
		public ExpressionLine Line
		{
			get
			{
				return line;
			}
			set
			{
				line = value;
			}
		}

		/// <summary>
		/// Container aonde a expressão está contida.
		/// </summary>
		public ContainerExpression Container
		{
			get
			{
				return container;
			}
			set
			{
				container = value;
			}
		}

		/// <summary>
		/// Posição da expressão no container.
		/// </summary>
		public int ContainerPosition
		{
			get
			{
				return containerPosition;
			}
			set
			{
				containerPosition = value;
			}
		}

		public TokenID Token
		{
			get
			{
				return _token;
			}
			set
			{
				_token = value;
			}
		}

		/// <summary>
		/// Container especial que a expressão está contida.
		/// </summary>
		public SpecialContainer CurrentSpecialContainer
		{
			get
			{
				return _currentSpecialContainer;
			}
			set
			{
				_currentSpecialContainer = value;
			}
		}

		/// <summary>
		/// Contrutor padrão.
		/// </summary>
		public Expression()
		{
		}

		/// <summary>
		/// Cria a instancia com o tipo do token informado.
		/// </summary>
		/// <param name="token"></param>
		public Expression(TokenID token)
		{
			_token = token;
		}

		/// <summary>
		/// Craia a expressão com o ponto do texto.
		/// </summary>
		/// <param name="beginPoint"></param>
		/// <param name="length"></param>
		public Expression(int beginPoint, int length)
		{
			this.beginPoint = beginPoint;
			this.length = length;
		}

		public Expression(int beginPoint, int length, ExpressionLine line)
		{
			this.beginPoint = beginPoint;
			this.length = length;
			this.line = line;
			this.line.Expressions.Add(this);
		}

		public Expression(int beginPoint, int length, ExpressionLine line, string command) : this(beginPoint, length, line, command, TokenID.Identifier)
		{
			this.text = command.Substring(beginPoint, length);
		}

		public Expression(int beginPoint, int length, ExpressionLine line, string command, TokenID token) : this(beginPoint, length, line)
		{
			this.text = command.Substring(beginPoint, length);
			_token = token;
		}

		public Expression(int beginPoint, ExpressionLine line, char command) : this(beginPoint, 1, line)
		{
			this.text = new string(command, 1);
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return text;
		}
	}
}
