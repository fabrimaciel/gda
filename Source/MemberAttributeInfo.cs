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
using System.Reflection;
using System.Collections;

namespace GDA.Common.Configuration
{
	internal class MemberAttributeInfo
	{
		/// <summary>
		/// Membro relacionado (method, field, property, etc.).
		/// </summary>
		public readonly MemberInfo MemberInfo;

		/// <summary>
		/// Lista dos atributos relacionados.
		/// </summary>
		public readonly IList Attributes;

		/// <summary>
		/// Cria um nova instancia fazendo o link entre o MemberInfo e os atributos relacionados
		/// </summary>
		public MemberAttributeInfo(MemberInfo memberInfo, IList attributes)
		{
			this.MemberInfo = memberInfo;
			this.Attributes = attributes;
		}

		/// <summary>
		/// Obtem o atributo relacionados com o MemberInfo.
		/// </summary>
		public Attribute this[int index]
		{
			get
			{
				return Attributes[index] as Attribute;
			}
		}
	}
}
