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
	/// Esse validador fornece a estrutura básica para a criação
	/// de validadores adaptados.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
	public abstract class ValidatorAttribute : Attribute
	{
		private ValidationMessage _message = new ValidationMessage();

		/// <summary>
		/// Mensagem de validação.
		/// </summary>
		public virtual ValidationMessage Message
		{
			get
			{
				return _message;
			}
			protected set
			{
				_message = value;
			}
		}

		public virtual string MessageText
		{
			get
			{
				return _message.Message;
			}
			set
			{
				_message.Message = value;
			}
		}

		/// <summary>
		/// Esse método deve ser sobreescrito para tratar a valicação da propridade.
		/// </summary>
		/// <param name="session">Sessão de conexão que pode ser usada na validação.</param>
		/// <param name="mode">Modo onde para qual está sendo feita a validação.</param>
		/// <param name="propertyName">Nome da propriedade.</param>
		/// <param name="propertyValue">Valor da propriedade.</param>
		/// <param name="parent">Objeto persistente onde a propriedade está inserida.</param>
		/// <returns>O método retorna true se a validação for bem sucedida.</returns>
		public virtual bool Validate(GDASession session, ValidationMode mode, string propertyName, object propertyValue, object parent)
		{
			return true;
		}
	}
}
