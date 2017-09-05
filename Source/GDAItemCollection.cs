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
using System.Collections;
using System.Globalization;

namespace GDA.Collections
{
	public class GDAItemCollection<T> : GDAList<T>, IList<T>, IBindingList, IBindingListView
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public GDAItemCollection()
		{
		}

		/// <summary>
		/// Construtor que inicializa a collection como uma collection.
		/// </summary>
		/// <param name="collection"></param>
		public GDAItemCollection(IEnumerable<T> collection) : base(collection)
		{
		}

		public event ListChangedEventHandler ListChanged;

		private ListChangedEventArgs resetEvent = new ListChangedEventArgs(ListChangedType.Reset, -1);

		/// <summary>
		/// Método acionado quando a lista é alterada.
		/// </summary>
		/// <param name="ev"></param>
		protected virtual void OnListChanged(ListChangedEventArgs ev)
		{
			if(ListChanged != null)
			{
				ListChanged(this, ev);
			}
		}

		public bool AllowEdit
		{
			get
			{
				return true;
			}
		}

		public bool AllowNew
		{
			get
			{
				return true;
			}
		}

		public bool AllowRemove
		{
			get
			{
				return true;
			}
		}

		public bool SupportsChangeNotification
		{
			get
			{
				return false;
			}
		}

		public bool SupportsSearching
		{
			get
			{
				return true;
			}
		}

		public bool SupportsSorting
		{
			get
			{
				return true;
			}
		}

		public object AddNew()
		{
			T item = (T)Activator.CreateInstance(typeof(T));
			this.Add(item);
			return item;
		}

		private bool isSorted = false;

		public bool IsSorted
		{
			get
			{
				return isSorted;
			}
		}

		private ListSortDirection listSortDirection = ListSortDirection.Ascending;

		public ListSortDirection SortDirection
		{
			get
			{
				return listSortDirection;
			}
		}

		PropertyDescriptor sortProperty = null;

		string sortPropertyName = null;

		public PropertyDescriptor SortProperty
		{
			get
			{
				return sortProperty;
			}
		}

		public void AddIndex(PropertyDescriptor property)
		{
			isSorted = true;
			sortProperty = property;
			sortPropertyName = property.Name;
		}

		public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
		{
			sortProperty = property;
			ApplySort(property.Name, direction);
		}

		public void ApplySort(string property, ListSortDirection direction)
		{
			isSorted = true;
			sortPropertyName = property;
			listSortDirection = direction;
			ArrayList a = new ArrayList();
			this.Sort(new ObjectPropertyComparer<T>(sortPropertyName));
			if(direction == ListSortDirection.Descending)
				this.Reverse();
			OnListChanged(resetEvent);
		}

		private int[] Find(string propertyName, object key, bool getFirst)
		{
			GDAList<int> itens = new GDAList<int>();
			Type type = typeof(T);
			int count = 0;
			foreach (T o in this)
			{
				if(Match(type.GetProperty(propertyName).GetValue(o, null), key))
				{
					if(getFirst)
						return new int[] {
							count
						};
					else
						itens.Add(count);
				}
				count++;
			}
			if(getFirst)
				return null;
			else
				return itens.ToArray();
		}

		public int Find(string propertyName, object key)
		{
			int[] value = Find(propertyName, key, true);
			if(value == null)
			{
				return -1;
			}
			else
			{
				OnListChanged(new ListChangedEventArgs(ListChangedType.ItemMoved, value[0], 0));
				return value[0];
			}
		}

		public int Find(PropertyDescriptor property, object key)
		{
			return Find(property.Name, key);
		}

		public void RemoveIndex(PropertyDescriptor property)
		{
			sortProperty = null;
			sortPropertyName = null;
		}

		public void RemoveSort()
		{
			isSorted = false;
			sortProperty = null;
			sortPropertyName = null;
			OnListChanged(resetEvent);
		}

		private bool applyFilter = false;

		private int[] itensFilter;

		public void ApplyFilter(string propertyName, object key)
		{
			itensFilter = Find(propertyName, key, false);
			applyFilter = true;
			OnListChanged(resetEvent);
		}

		public void RemoveFilter()
		{
			applyFilter = false;
			OnListChanged(resetEvent);
		}

		protected internal override T GetItem(int index)
		{
			if(applyFilter)
				return base.GetItem(itensFilter[index]);
			else
				return base.GetItem(index);
		}

		protected internal override void SetItem(int index, T value)
		{
			if(applyFilter)
				base.SetItem(itensFilter[index], value);
			else
				base.SetItem(index, value);
		}

		protected internal override int GetCount()
		{
			if(applyFilter)
				return itensFilter.Length;
			else
				return base.GetCount();
		}

		public override void OnInsertComplete(int index, T value)
		{
			base.OnInsert(index, value);
			OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, index));
		}

		public override void OnRemoveComplete(int index, T value)
		{
			base.OnRemove(index, value);
			OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
		}

		public override void OnRemoveAll()
		{
			base.OnRemoveAll();
			OnListChanged(resetEvent);
		}

		public override void OnClearComplete()
		{
			base.OnClear();
			OnListChanged(resetEvent);
		}

		public override void OnSetComplete(int index, T newValue)
		{
			base.OnSetComplete(index, newValue);
			OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
		}

		protected bool Match(object data, object searchValue)
		{
			if(data == null || searchValue == null)
			{
				return (bool)(data == searchValue);
			}
			bool IsString = (bool)(data is string);
			if(data.GetType() != searchValue.GetType())
				throw new ArgumentException("Objects must be of the same type");
			if(!(data.GetType().IsValueType || data is string))
				throw new ArgumentException("Objects must be a value type");
			if(IsString)
			{
				string stringData = ((string)data).ToLower(CultureInfo.CurrentCulture);
				string stringMatch = ((string)searchValue).ToLower(CultureInfo.CurrentCulture);
				return (bool)(stringData.IndexOf(stringMatch) >= 0);
			}
			else
			{
				return (bool)(Comparer.Default.Compare(data, searchValue) == 0);
			}
		}

		private string m_FilterString = null;

		void IBindingListView.ApplySort(ListSortDescriptionCollection sorts)
		{
			throw new NotSupportedException();
		}

		string IBindingListView.Filter
		{
			get
			{
				return m_FilterString;
			}
			set
			{
				m_FilterString = value;
				UpdateFilter();
			}
		}

		void IBindingListView.RemoveFilter()
		{
			RemoveFilter();
		}

		ListSortDescriptionCollection IBindingListView.SortDescriptions
		{
			get
			{
				return null;
			}
		}

		bool IBindingListView.SupportsAdvancedSorting
		{
			get
			{
				return false;
			}
		}

		bool IBindingListView.SupportsFiltering
		{
			get
			{
				return true;
			}
		}

		protected virtual void UpdateFilter()
		{
			int equalsPos = m_FilterString.IndexOf('=');
			string propName = m_FilterString.Substring(0, equalsPos).Trim();
			string criteria = m_FilterString.Substring(equalsPos + 1, m_FilterString.Length - equalsPos - 1).Trim();
			criteria = criteria.Substring(1, criteria.Length - 2);
			ApplyFilter(propName, criteria);
		}

		/// <summary>
		/// Converte implicitamente para um lista tipada.
		/// </summary>
		/// <param name="collection"></param>
		/// <returns>Lista tipada.</returns>
		public static implicit operator List<T>(GDAItemCollection<T> collection)
		{
			return new List<T>(collection);
		}
	}
}
