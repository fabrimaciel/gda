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
	/// Validador usado para membros que precisam ter um valor assinalado.
	/// Esse tipo de validador é normalmente usado para membros do tipo string.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
	public class RequiredValidatorAttribute : ValidatorAttribute
	{
		private bool allowNull = false;

		/// <summary>
		/// Identifica se o validador permitirá que o membro tenha valor null.
		/// </summary>
		public bool AllowNull
		{
			get
			{
				return allowNull;
			}
			set
			{
				allowNull = value;
			}
		}

		public RequiredValidatorAttribute() : base()
		{
		}

		public RequiredValidatorAttribute(string messageText) : base()
		{
			MessageText = messageText;
		}

		/// <summary>
		/// Esse método deve ser sobreescrito para tratar a valicação da propridade.
		/// </summary>
		/// <param name="propertyName">Nome da propriedade.</param>
		/// <param name="propertyValue">Valor da propriedade.</param>
		/// <param name="parent">Objeto persistente onde a propriedade está inserida.</param>
		/// <returns>O método retorna true se a validação for bem sucedida.</returns>
		public override bool Validate(GDASession session, ValidationMode mode, string propertyName, object propertyValue, object parent)
		{
			if(mode == ValidationMode.Delete)
				return true;
			if(propertyValue == null)
			{
				return allowNull;
			}
			return !(propertyValue is string && string.Empty == (string)propertyValue);
		}
	}
}
