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
using System.Linq;
using System.Text;

namespace GDA.Diagnostics
{
	/// <summary>
	/// Representa um erro do trace.
	/// </summary>
	public class GDATraceException : GDAException
	{
		/// <summary>
		/// Cria a instancia tendo como referencia um erro interno.
		/// </summary>
		/// <param name="innerException"></param>
		public GDATraceException(Exception innerException) : base(innerException.Message, innerException)
		{
		}

		/// <summary>
		/// Cria a instancia com a mensagem do erro.
		/// </summary>
		/// <param name="message"></param>
		public GDATraceException(string message) : base(message)
		{
		}

		/// <summary>
		/// Cria a instancia com a mensagem do erro e com a referencia do erro interno.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		public GDATraceException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
