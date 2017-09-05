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

namespace GDA
{
	/// <summary>
	/// Representa o resultado de uma validação executada.
	/// </summary>
	public class ValidationResult
	{
		/// <summary>
		/// Mensagens do resultado da validação.
		/// </summary>
		public List<ValidationMessage> Messages
		{
			get;
			private set;
		}

		/// <summary>
		/// Identifica se o resultado foi válido.
		/// </summary>
		public bool IsValid
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor usado somente no GDA.
		/// </summary>
		/// <param name="messages"></param>
		internal ValidationResult(List<ValidationMessage> messages, bool isValid)
		{
			Messages = messages;
			IsValid = isValid;
		}

		/// <summary>
		/// Construtor geral.
		/// </summary>
		public ValidationResult()
		{
			Messages = new List<ValidationMessage>();
		}
	}
}
