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
using System.ComponentModel;

namespace GDA.Sql
{
	public class BindingResultList<Model> : ResultList<Model>, IBindingList where Model : new()
	{
		/// <summary>
		/// Construtor.
		/// </summary>
		/// <param name="pageSize">Tamanho da página da lista</param>
		public BindingResultList(int pageSize) : base(pageSize)
		{
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="query">Consulta.</param>
		/// <param name="pageSize">Tamanho da página da consulta.</param>
		public BindingResultList(Query query, int pageSize) : base(query, pageSize)
		{
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="query">Consulta.</param>
		/// <param name="session">Sessão usada nas consultas.</param>
		/// <param name="pageSize">Tamanho da página da consulta.</param>
		public BindingResultList(Query query, GDASession session, int pageSize) : base(query, session, pageSize)
		{
		}

		/// <summary>
		/// Propriedade que e lista está ordenada.
		/// </summary>
		private PropertyDescriptor _sortProperty;

		/// <summary>
		/// Direção da ordenação da lista.
		/// </summary>
		private ListSortDirection _direction;

		public void AddIndex(PropertyDescriptor property)
		{
			throw new NotSupportedException();
		}

		public object AddNew()
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Identifica se é permitida a edição da lista.
		/// </summary>
		public bool AllowEdit
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Identifica se é permitido a criação de novos itens na lista.
		/// </summary>
		public bool AllowNew
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Identifica se é permitido a remoção de itens na lista.
		/// </summary>
		public bool AllowRemove
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Aplica um ordenação na lista.
		/// </summary>
		/// <param name="property"></param>
		/// <param name="direction"></param>
		public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
		{
			if(_queryInstance == null)
				throw new GDAException("ResultList not support for Query {0}.", _myQuery.GetType().FullName);
			_sortProperty = property;
			_direction = direction;
			_queryInstance.SetOrder(property.Name + " " + (direction == ListSortDirection.Ascending ? "ASC" : "DESC"));
			Refresh();
		}

		/// <summary>
		/// Remove a ordenação da lista.
		/// </summary>
		public void RemoveSort()
		{
			if(_queryInstance == null)
				throw new GDAException("ResultList not support for Query {0}.", _myQuery.GetType().FullName);
			_sortProperty = null;
			_queryInstance.SetOrder(null);
			Refresh();
		}

		public int Find(PropertyDescriptor property, object key)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Identifica se a lista está ordenada.
		/// </summary>
		public bool IsSorted
		{
			get
			{
				return (_sortProperty != null);
			}
		}

		/// <summary>
		/// Evento acionado quando a lista for alterada.
		/// </summary>
		public event ListChangedEventHandler ListChanged;

		/// <summary>
		/// Remove o index do filtro.
		/// </summary>
		/// <param name="property"></param>
		public void RemoveIndex(PropertyDescriptor property)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Direção de ordenação da lista.
		/// </summary>
		public ListSortDirection SortDirection
		{
			get
			{
				return _direction;
			}
		}

		/// <summary>
		/// Propriedade que a lista está ordenada.
		/// </summary>
		public PropertyDescriptor SortProperty
		{
			get
			{
				return _sortProperty;
			}
		}

		/// <summary>
		/// Identifica se a lista da suporta a notificação de alteração.
		/// </summary>
		public bool SupportsChangeNotification
		{
			get
			{
				return true;
			}
		}

		public bool SupportsSearching
		{
			get
			{
				return false;
			}
		}

		public bool SupportsSorting
		{
			get
			{
				return true;
			}
		}

		public int Add(object value)
		{
			throw new NotImplementedException();
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		public bool Contains(object value)
		{
			throw new NotImplementedException();
		}

		public int IndexOf(object value)
		{
			throw new NotImplementedException();
		}

		public void Insert(int index, object value)
		{
			throw new NotImplementedException();
		}

		public bool IsFixedSize
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public void Remove(object value)
		{
			throw new NotImplementedException();
		}

		public void RemoveAt(int index)
		{
			throw new NotImplementedException();
		}

		public new object this[int index]
		{
			get
			{
				return base[index];
			}
			set
			{
				base[index] = (Model)value;
			}
		}
	}
}
