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

namespace GDA.Sql
{
	/// <summary>
	/// Tipos de join
	/// </summary>
	public enum JoinType
	{
		InnerJoin,
		LeftJoin,
		RightJoin,
		CrossJoin
	}
	/// <summary>
	/// Representa as informações de um JOIN.
	/// </summary>
	public class JoinInfo
	{
		private Type _classTypeMain;

		private Type _classTypeJoin;

		private JoinType _type;

		private string _classTypeMainAlias;

		private string _classTypeJoinAlias;

		private string _groupOfRelationship;

		private string _joinTableAlias;

		/// <summary>
		/// Tipo da classe principal do join.
		/// </summary>
		public Type ClassTypeMain
		{
			get
			{
				return _classTypeMain;
			}
			set
			{
				_classTypeMain = value;
			}
		}

		/// <summary>
		/// Tipo da classe representada pelo join.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		public Type ClassTypeJoin
		{
			get
			{
				return _classTypeJoin;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("TypeClassJoin");
				_classTypeJoin = value;
			}
		}

		/// <summary>
		/// Tipo do join.
		/// </summary>
		public JoinType Type
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
			}
		}

		/// <summary>
		/// Apelido da classe principal do join.
		/// </summary>
		public string ClassTypeMainAlias
		{
			get
			{
				return _classTypeMainAlias;
			}
			set
			{
				_classTypeMainAlias = value;
			}
		}

		/// <summary>
		/// Apelido da classe relacionada com o join.
		/// </summary>
		public string ClassTypeJoinAlias
		{
			get
			{
				return _classTypeJoinAlias;
			}
			set
			{
				_classTypeJoinAlias = value;
			}
		}

		/// <summary>
		/// Nome do grupo de relacionamento do join.
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
		/// Apelido da tabela do join.
		/// </summary>
		internal string JoinTableAlias
		{
			get
			{
				return _joinTableAlias;
			}
			set
			{
				_joinTableAlias = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="type">Tipo do join.</param>
		/// <param name="classTypeMain">Tipo da classe principal do join.</param>
		/// <param name="classTypeJoin">Tipo da classe relacionada com o join.</param>
		/// <param name="classTypeJoinAlias">Apelido da classe relacionada com o join.</param>
		/// <param name="groupOfRelationship">Nome do grupo de relacionamento do join.</param>
		/// <exception cref="ArgumentNullException"></exception>
		public JoinInfo(JoinType type, Type classTypeMain, Type classTypeJoin, string classTypeMainAlias, string classTypeJoinAlias, string groupOfRelationship)
		{
			if(classTypeJoin == null)
				throw new ArgumentNullException("typeClassJoin");
			_type = type;
			_classTypeJoin = classTypeJoin;
			_groupOfRelationship = groupOfRelationship;
			_classTypeMainAlias = classTypeMainAlias;
			_classTypeJoinAlias = classTypeJoinAlias;
			_classTypeMain = classTypeMain;
		}
	}
}
