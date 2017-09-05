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
	/// O ValidationMessage representa uma mensagem que será 
	/// exibida caso o valor não passe na validação.
	/// </summary>
	public class ValidationMessage
	{
		private string _Id;

		private string _message;

		private string _propertyName;

		/// <summary>
		/// Identificador da mensagem.
		/// </summary>
		public string Id
		{
			get
			{
				return _Id;
			}
			set
			{
				_Id = value;
			}
		}

		/// <summary>
		/// Mensagem relacionada.
		/// </summary>
		public string Message
		{
			get
			{
				return _message;
			}
			set
			{
				_message = value;
			}
		}

		/// <summary>
		/// Nome da propriedade que foi feita a validação.
		/// </summary>
		public string PropertyName
		{
			get
			{
				return _propertyName;
			}
			set
			{
				_propertyName = value;
			}
		}

		public virtual ValidationMessage Clone(string propertyName)
		{
			ValidationMessage vm = new ValidationMessage();
			vm.Message = this.Message;
			vm.Id = this.Id;
			vm.PropertyName = propertyName;
			return vm;
		}
	}
}
