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

namespace GDA.Sql
{
	/// <summary>
	/// Enumeration dos operadores de comparação usados no comando sql.
	/// </summary>
	public enum Operator
	{
		/// <summary>
		/// Operador que faz uma comparação restrita.
		/// </summary>
		Equals,
		/// <summary>
		/// Operador que faz uma comparação de diferença.
		/// </summary>
		NotEquals,
		/// <summary>
		/// Operador que faz uma comparação se uma string está contida
		/// no campo referenciado.
		/// </summary>
		Like,
		/// <summary>
		/// Operador que faz uma comparação se uma string não está contida
		/// no campo referenciado.
		/// </summary>
		NotLike,
		/// <summary>
		/// Operador menor que.
		/// </summary>
		LessThan,
		/// <summary>
		/// Operador menor que ou igual.
		/// </summary>
		LessThanOrEquals,
		/// <summary>
		/// Operador maior que.
		/// </summary>
		GreaterThan,
		/// <summary>
		/// Operador maior que ou igual.
		/// </summary>
		GreaterThanOrEquals,
		/// <summary>
		/// Operador indica que o campo referenciado tem que ter o seus valores
		/// contidos no em um conjunto de dados separados por virgula, ou em um
		/// outro comando sql.
		/// </summary>
		In,
		NotIn
	}
	/// <summary>
	/// Enumeration dos operadores lógicos usados no comando sql.
	/// </summary>
	public enum LogicalOperator
	{
		And,
		Or
	}
	/// <summary>
	/// Enumeration dos tipos de ordenação.
	/// </summary>
	public enum Order
	{
		Ascending,
		Descending
	}
}
