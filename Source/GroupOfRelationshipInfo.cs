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
	/// Representa as informações do grupo de relacionamento.
	/// </summary>
	public class GroupOfRelationshipInfo : IEquatable<GroupOfRelationshipInfo>
	{
		/// <summary>
		/// Tipo da classe onde o relacionamento está inserido.
		/// </summary>
		private Type _typeOfClass;

		/// <summary>
		/// Tipo da classe relacionada.
		/// </summary>
		private Type _typeOfClassRelated;

		/// <summary>
		/// Nome do grupo de relacionamento.
		/// </summary>
		private string _groupOfRelationship;

		/// <summary>
		/// Chaves estrangeiras do grupo.
		/// </summary>
		private List<ForeignKeyMapper> _foreignKeys = new List<ForeignKeyMapper>();

		/// <summary>
		/// Tipo da classe onde o relacionamento está inserido.
		/// </summary>
		public Type TypeOfClass
		{
			get
			{
				return _typeOfClass;
			}
			set
			{
				_typeOfClass = value;
			}
		}

		/// <summary>
		/// Tipo da classe relacionada.
		/// </summary>
		public Type TypeOfClassRelated
		{
			get
			{
				return _typeOfClassRelated;
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

		public List<ForeignKeyMapper> ForeignKeys
		{
			get
			{
				return _foreignKeys;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="typeOfClasse">Tipo da classe onde o relacionamento está inserido.</param>
		/// <param name="typeOfClassRelated">Nome da classe relacionada.</param>
		/// <param name="groupOfRelationship">Nome do relacionamento.</param>
		/// <exception cref="ArgumentNullException"></exception>
		public GroupOfRelationshipInfo(Type typeOfClasse, Type typeOfClassRelated, string groupOfRelationship)
		{
			if(typeOfClasse == null)
				throw new ArgumentNullException("typeOfClasse");
			else if(typeOfClassRelated == null)
				throw new ArgumentNullException("typeOfClassRelated");
			_typeOfClass = typeOfClasse;
			_typeOfClassRelated = typeOfClassRelated;
			_groupOfRelationship = groupOfRelationship;
		}

		/// <summary>
		/// Adiciona uma chave estrangeira no grupo.
		/// </summary>
		/// <param name="fk"></param>
		public void AddForeignKey(ForeignKeyMapper fk)
		{
			_foreignKeys.Add(fk);
		}

		public override bool Equals(object obj)
		{
			if(obj is GroupOfRelationshipInfo)
				return Equals((GroupOfRelationshipInfo)obj);
			else
				return false;
		}

		public bool Equals(GroupOfRelationshipInfo other)
		{
			return _typeOfClassRelated == other.TypeOfClassRelated && _groupOfRelationship == other.GroupOfRelationship;
		}

		public static bool operator ==(GroupOfRelationshipInfo g1, GroupOfRelationshipInfo g2)
		{
			bool xnull, ynull;
			xnull = Object.ReferenceEquals(g1, null);
			ynull = Object.ReferenceEquals(g2, null);
			if(xnull && ynull)
				return true;
			if(!xnull && !ynull)
				return g1.Equals(g2);
			else
				return false;
		}

		public static bool operator !=(GroupOfRelationshipInfo g1, GroupOfRelationshipInfo g2)
		{
			bool xnull, ynull;
			xnull = Object.ReferenceEquals(g1, null);
			ynull = Object.ReferenceEquals(g2, null);
			if(xnull && ynull)
				return false;
			if(!xnull && !ynull)
				return !g1.Equals(g2);
			else
				return true;
		}
	}
}
