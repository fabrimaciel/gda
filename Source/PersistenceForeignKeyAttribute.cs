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
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class PersistenceForeignKeyAttribute : Attribute
	{
		/// <summary>
		/// Tipo da classe relacionada.
		/// </summary>
		private Type _typeOfClassRelated;

		/// <summary>
		/// Nome da propriedade da classe do tipo especificado aonde o relacionamento será feito.
		/// </summary>
		private string _propertyName;

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
		/// Nome da propriedade da classe do tipo especificado aonde o relacionamento será feito.
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
		/// Constrói um instancia de PersistenceForeignKeyAttribute com o número do grupo de relacionamento informado.
		/// </summary>
		/// <param name="typeOfClassRelated">Classe na qual será feito o relacionamento.</param>
		/// <param name="propertyName">Nome da propriedade que será relacionada.</param>
		/// <param name="numberGroupOfRelationship">Número do grupo de relacionamento.</param>
		public PersistenceForeignKeyAttribute(Type typeOfClassRelated, string propertyName, int numberGroupOfRelationship) : this(typeOfClassRelated, propertyName, numberGroupOfRelationship.ToString())
		{
		}

		/// <summary>
		/// Constrói um instancia de PersistenceForeignKeyAttribute com o número do grupo de relacionamento informado.
		/// </summary>
		/// <param name="typeOfClassRelated">Classe na qual será feito o relacionamento.</param>
		/// <param name="propertyName">Nome da propriedade que será relacionada.</param>
		/// <param name="groupOfRelationship">Nome do grupo de relacionamento.</param>
		public PersistenceForeignKeyAttribute(Type typeOfClassRelated, string propertyName, string groupOfRelationship)
		{
			_typeOfClassRelated = typeOfClassRelated;
			_propertyName = propertyName;
			_groupOfRelationship = groupOfRelationship;
		}

		/// <summary>
		/// Constrói um instancia de PersistenceForeignKeyAttribute.
		/// </summary>
		/// <param name="typeOfClassRelated">Classe na qual será feito o relacionamento.</param>
		/// <param name="propertyName">Nome da propriedade que será relacionada.</param>
		public PersistenceForeignKeyAttribute(Type typeOfClassRelated, string propertyName) : this(typeOfClassRelated, propertyName, null)
		{
		}
	}
}
