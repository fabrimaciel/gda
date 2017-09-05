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
using GDA.Sql;
using System.Reflection;

namespace GDA
{
	/// <summary>
	/// Valida se o valor da propriedade é unico no banco de dados.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class UniqueAttribute : ValidatorAttribute
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public UniqueAttribute() : base()
		{
		}

		public UniqueAttribute(string message) : base()
		{
			MessageText = message;
		}

		/// <summary>
		/// Valida a unicidade da propriedade
		/// </summary>
		/// <param name="session"></param>
		/// <param name="propertyName"></param>
		/// <param name="propertyValue"></param>
		/// <param name="parent"></param>
		/// <returns></returns>
		public override bool Validate(GDASession session, ValidationMode mode, string propertyName, object propertyValue, object parent)
		{
			if(mode == ValidationMode.Delete)
				return true;
			return !GDAOperations.CheckExist(session, mode, propertyName, propertyValue, parent);
		}
	}
}
