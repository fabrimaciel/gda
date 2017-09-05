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

namespace GDA.Sql.InterpreterExpression.Nodes
{
	class SelectPart
	{
		private uint? _top;

		/// <summary>
		/// Identifica se a valor do top está em porcentagem.
		/// </summary>
		private bool _topInPercent = false;

		/// <summary>
		/// Tipo da resultado que a clausula pode retornar.
		/// </summary>
		private Enums.SelectClauseResultTypes _resultType = GDA.Sql.InterpreterExpression.Enums.SelectClauseResultTypes.All;

		/// <summary>
		/// Expressões relacionadas a parte.
		/// </summary>
		private List<SelectExpression> _selectionExpressions = new List<SelectExpression>();

		public uint? Top
		{
			get
			{
				return _top;
			}
			set
			{
				_top = value;
			}
		}

		/// <summary>
		/// Identifica se a valor do top está em porcentagem.
		/// </summary>
		public bool TopInPercent
		{
			get
			{
				return _topInPercent;
			}
			set
			{
				_topInPercent = value;
			}
		}

		/// <summary>
		/// Tipo da resultado que a clausula pode retornar.
		/// </summary>
		public Enums.SelectClauseResultTypes ResultType
		{
			get
			{
				return _resultType;
			}
			set
			{
				_resultType = value;
			}
		}

		public List<SelectExpression> SelectionExpressions
		{
			get
			{
				return _selectionExpressions;
			}
			set
			{
				_selectionExpressions = value;
			}
		}
	}
}
