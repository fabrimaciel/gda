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

namespace GDA
{
	public abstract class ForeignMapper
	{
		/// <summary>
		/// Tipo da classe relacionada.
		/// </summary>
		private Type _typeOfClassRelated;

		/// <summary>
		/// Propriedade da classe relacionada.
		/// </summary>
		private PropertyInfo _propertyOfClassRelated;

		/// <summary>
		/// Propriedade da model.
		/// </summary>
		private PropertyInfo _propertyModel;

		/// <summary>
		/// Nome do grupo de relacionamento.
		/// </summary>
		private string _groupOfRelationship;

		/// <summary>
		/// Tipo da classe que representa a tabela relacionada.
		/// </summary>
		public Type TypeOfClassRelated
		{
			get
			{
				return _typeOfClassRelated;
			}
			set
			{
				_typeOfClassRelated = value;
			}
		}

		/// <summary>
		/// Propriedade da classe relacionada.
		/// </summary>
		public PropertyInfo PropertyOfClassRelated
		{
			get
			{
				return _propertyOfClassRelated;
			}
			set
			{
				_propertyOfClassRelated = value;
			}
		}

		/// <summary>
		/// Nome do grupo de relacionamento.
		/// </summary>
		public string GroupOfRelationship
		{
			get
			{
				return _groupOfRelationship;
			}
			set
			{
				_groupOfRelationship = value;
			}
		}

		/// <summary>
		/// Propriedade da model.
		/// </summary>
		public PropertyInfo PropertyModel
		{
			get
			{
				return _propertyModel;
			}
			set
			{
				_propertyModel = value;
			}
		}
	}
}
