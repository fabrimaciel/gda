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
using System.Collections;
using System.Collections.ObjectModel;

namespace GDA.Collections
{
	[System.Diagnostics.DebuggerDisplay("Count = {Count}")]
	public class GDAList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable
	{
		private static T[] _emptyArray;

		private T[] _items;

		private int _size;

		private object _syncRoot;

		private int _version;

		static GDAList()
		{
			GDAList<T>._emptyArray = new T[0];
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public GDAList()
		{
			this._items = GDAList<T>._emptyArray;
		}

		public GDAList(IEnumerable<T> collection)
		{
			if(collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			ICollection<T> is2 = collection as ICollection<T>;
			if(is2 != null)
			{
				int count = is2.Count;
				this._items = new T[count];
				is2.CopyTo(this._items, 0);
				this._size = count;
			}
			else
			{
				this._size = 0;
				this._items = new T[4];
				using (IEnumerator<T> enumerator = collection.GetEnumerator())
					while (enumerator.MoveNext())
						this.Add(enumerator.Current);
			}
		}

		public GDAList(int capacity)
		{
			if(capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity", "ArgumentOutOfRange_SmallCapacity");
			}
			this._items = new T[capacity];
		}

		public void Add(T item)
		{
			OnInsert(this._size, item);
			if(this._size == this._items.Length)
			{
				this.EnsureCapacity(this._size + 1);
			}
			this._items[this._size++] = item;
			this._version++;
			OnInsertComplete(this._size - 1, item);
		}

		public void AddRange(IEnumerable<T> collection)
		{
			this.InsertRange(this._size, collection);
		}

		public ReadOnlyCollection<T> AsReadOnly()
		{
			return new ReadOnlyCollection<T>(this);
		}

		public int BinarySearch(T item)
		{
			return this.BinarySearch(0, this.Count, item, null);
		}

		public int BinarySearch(T item, IComparer<T> comparer)
		{
			return this.BinarySearch(0, this.Count, item, comparer);
		}

		public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
		{
			if((index < 0) || (count < 0))
			{
				throw new ArgumentOutOfRangeException((index < 0) ? "index" : "count", "ArgumentOutOfRange_NeedNonNegNum");
			}
			if((this._size - index) < count)
			{
				throw new ArgumentException("Argument_InvalidOffLen");
			}
			return Array.BinarySearch<T>(this._items, index, count, item, comparer);
		}

		public void Clear()
		{
			OnClear();
			Array.Clear(this._items, 0, this._size);
			this._size = 0;
			this._version++;
			OnClearComplete();
		}

		public bool Contains(T item)
		{
			if(item == null)
			{
				for(int i = 0; i < this._size; i++)
				{
					if(this._items[i] == null)
					{
						return true;
					}
				}
				return false;
			}
			EqualityComparer<T> comparer = EqualityComparer<T>.Default;
			for(int j = 0; j < this._size; j++)
			{
				if(comparer.Equals(this._items[j], item))
				{
					return true;
				}
			}
			return false;
		}

		public GDAList<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
		{
			if(converter == null)
			{
				throw new ArgumentNullException("converter");
			}
			GDAList<TOutput> list = new GDAList<TOutput>(this._size);
			for(int i = 0; i < this._size; i++)
			{
				list._items[i] = converter(this._items[i]);
			}
			list._size = this._size;
			return list;
		}

		public void CopyTo(T[] array)
		{
			this.CopyTo(array, 0);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			try
			{
				Array.Copy(this._items, 0, array, arrayIndex, this._size);
			}
			catch(InvalidCastException)
			{
				throw new ArgumentException("Argument_InvalidArrayType");
			}
		}

		public void CopyTo(int index, T[] array, int arrayIndex, int count)
		{
			if((this._size - index) < count)
			{
				throw new ArgumentException("Argument_InvalidOffLen");
			}
			try
			{
				Array.Copy(this._items, index, array, arrayIndex, count);
			}
			catch(InvalidCastException)
			{
				throw new ArgumentException("Argument_InvalidArrayType");
			}
		}

		private void EnsureCapacity(int min)
		{
			if(this._items.Length < min)
			{
				int num = (this._items.Length == 0) ? 4 : (this._items.Length * 2);
				if(num < min)
				{
					num = min;
				}
				this.Capacity = num;
			}
		}

		public bool Exists(Predicate<T> match)
		{
			return (this.FindIndex(match) != -1);
		}

		public T Find(Predicate<T> match)
		{
			if(match == null)
			{
				throw new ArgumentNullException("match");
			}
			for(int i = 0; i < this._size; i++)
			{
				if(match(this._items[i]))
				{
					return this._items[i];
				}
			}
			return default(T);
		}

		public GDAList<T> FindAll(Predicate<T> match)
		{
			if(match == null)
			{
				throw new ArgumentNullException("match");
			}
			GDAList<T> list = new GDAList<T>();
			for(int i = 0; i < this._size; i++)
			{
				if(match(this._items[i]))
				{
					list.Add(this._items[i]);
				}
			}
			return list;
		}

		public int FindIndex(Predicate<T> match)
		{
			return this.FindIndex(0, this._size, match);
		}

		public int FindIndex(int startIndex, Predicate<T> match)
		{
			return this.FindIndex(startIndex, this._size - startIndex, match);
		}

		public int FindIndex(int startIndex, int count, Predicate<T> match)
		{
			if(startIndex > this._size)
			{
				throw new ArgumentOutOfRangeException("startIndex", "ArgumentOutOfRange_Index");
			}
			if((count < 0) || (startIndex > (this._size - count)))
			{
				throw new ArgumentOutOfRangeException("count", "ArgumentOutOfRange_Count");
			}
			if(match == null)
			{
				throw new ArgumentNullException("match");
			}
			int num = startIndex + count;
			for(int i = startIndex; i < num; i++)
			{
				if(match(this._items[i]))
				{
					return i;
				}
			}
			return -1;
		}

		public T FindLast(Predicate<T> match)
		{
			if(match == null)
			{
				throw new ArgumentNullException("match");
			}
			for(int i = this._size - 1; i >= 0; i--)
			{
				if(match(this._items[i]))
				{
					return this._items[i];
				}
			}
			return default(T);
		}

		public int FindLastIndex(Predicate<T> match)
		{
			return this.FindLastIndex(this._size - 1, this._size, match);
		}

		public int FindLastIndex(int startIndex, Predicate<T> match)
		{
			return this.FindLastIndex(startIndex, startIndex + 1, match);
		}

		public int FindLastIndex(int startIndex, int count, Predicate<T> match)
		{
			if(match == null)
			{
				throw new ArgumentNullException("match");
			}
			if(this._size == 0)
			{
				if(startIndex != -1)
				{
					throw new ArgumentOutOfRangeException("startIndex", "ArgumentOutOfRange_Index");
				}
			}
			else if(startIndex >= this._size)
			{
				throw new ArgumentOutOfRangeException("startIndex", "ArgumentOutOfRange_Index");
			}
			if((count < 0) || (((startIndex - count) + 1) < 0))
			{
				throw new ArgumentOutOfRangeException("count", "ArgumentOutOfRange_Count");
			}
			int num = startIndex - count;
			for(int i = startIndex; i > num; i--)
			{
				if(match(this._items[i]))
				{
					return i;
				}
			}
			return -1;
		}

		public void ForEach(Action<T> action)
		{
			if(action == null)
			{
				throw new ArgumentNullException("match");
			}
			for(int i = 0; i < this._size; i++)
			{
				action(this._items[i]);
			}
		}

		public Enumerator<T> GetEnumerator()
		{
			return new Enumerator<T>((GDAList<T>)this);
		}

		public GDAList<T> GetRange(int index, int count)
		{
			if((index < 0) || (count < 0))
			{
				throw new ArgumentOutOfRangeException((index < 0) ? "index" : "count", "ArgumentOutOfRange_NeedNonNegNum");
			}
			if((this._size - index) < count)
			{
				throw new ArgumentException("Argument_InvalidOffLen");
			}
			GDAList<T> list = new GDAList<T>(count);
			Array.Copy(this._items, index, list._items, 0, count);
			list._size = count;
			return list;
		}

		public int IndexOf(T item)
		{
			return Array.IndexOf<T>(this._items, item, 0, this._size);
		}

		public int IndexOf(T item, int index)
		{
			if(index > this._size)
			{
				throw new ArgumentOutOfRangeException("index", "ArgumentOutOfRange_Index");
			}
			return Array.IndexOf<T>(this._items, item, index, this._size - index);
		}

		public int IndexOf(T item, int index, int count)
		{
			if(index > this._size)
			{
				throw new ArgumentOutOfRangeException("index", "ArgumentOutOfRange_Index");
			}
			if((count < 0) || (index > (this._size - count)))
			{
				throw new ArgumentOutOfRangeException("count", "ArgumentOutOfRange_Count");
			}
			return Array.IndexOf<T>(this._items, item, index, count);
		}

		public void Insert(int index, T item)
		{
			OnInsert(index, item);
			if(index > this._size)
			{
				throw new ArgumentOutOfRangeException("index", "ArgumentOutOfRange_ListInsert");
			}
			if(this._size == this._items.Length)
			{
				this.EnsureCapacity(this._size + 1);
			}
			if(index < this._size)
			{
				Array.Copy(this._items, index, this._items, index + 1, this._size - index);
			}
			this._items[index] = item;
			this._size++;
			this._version++;
			OnInsertComplete(index, item);
		}

		public void InsertRange(int index, IEnumerable<T> collection)
		{
			if(collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			if(index > this._size)
			{
				throw new ArgumentOutOfRangeException("index", "ArgumentOutOfRange_Index");
			}
			ICollection<T> is2 = collection as ICollection<T>;
			if(is2 != null)
			{
				int count = is2.Count;
				if(count > 0)
				{
					this.EnsureCapacity(this._size + count);
					if(index < this._size)
					{
						Array.Copy(this._items, index, this._items, index + count, this._size - index);
					}
					if(this == is2)
					{
						Array.Copy(this._items, 0, this._items, index, index);
						Array.Copy(this._items, index + count, this._items, index * 2, this._size - index);
					}
					else
					{
						T[] array = new T[count];
						is2.CopyTo(array, 0);
						array.CopyTo(this._items, index);
					}
					this._size += count;
				}
			}
			else
			{
				using (IEnumerator<T> enumerator = collection.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						this.Insert(index++, enumerator.Current);
					}
				}
			}
			this._version++;
		}

		private static bool IsCompatibleObject(object value)
		{
			if(!(value is T) && ((value != null) || typeof(T).IsValueType))
			{
				return false;
			}
			return true;
		}

		public int LastIndexOf(T item)
		{
			return this.LastIndexOf(item, this._size - 1, this._size);
		}

		public int LastIndexOf(T item, int index)
		{
			if(index >= this._size)
			{
				throw new ArgumentOutOfRangeException("index", "ArgumentOutOfRange_Index");
			}
			return this.LastIndexOf(item, index, index + 1);
		}

		public int LastIndexOf(T item, int index, int count)
		{
			if(this._size == 0)
			{
				return -1;
			}
			if((index < 0) || (count < 0))
			{
				throw new ArgumentOutOfRangeException((index < 0) ? "index" : "count", "ArgumentOutOfRange_NeedNonNegNum");
			}
			if((index >= this._size) || (count > (index + 1)))
			{
				throw new ArgumentOutOfRangeException((index >= this._size) ? "index" : "count", "ArgumentOutOfRange_BiggerThanCollection");
			}
			return Array.LastIndexOf<T>(this._items, item, index, count);
		}

		public bool Remove(T item)
		{
			int index = this.IndexOf(item);
			if(index >= 0)
			{
				this.RemoveAt(index);
				return true;
			}
			return false;
		}

		public int RemoveAll(Predicate<T> match)
		{
			if(match == null)
			{
				throw new ArgumentNullException("match");
			}
			int index = 0;
			while ((index < this._size) && !match(this._items[index]))
			{
				index++;
			}
			if(index >= this._size)
			{
				return 0;
			}
			int num2 = index + 1;
			while (num2 < this._size)
			{
				while ((num2 < this._size) && match(this._items[num2]))
				{
					num2++;
				}
				if(num2 < this._size)
				{
					this._items[index++] = this._items[num2++];
				}
			}
			Array.Clear(this._items, index, this._size - index);
			int num3 = this._size - index;
			this._size = index;
			this._version++;
			OnRemoveAll();
			return num3;
		}

		public void RemoveAt(int index)
		{
			if(index >= this._size)
			{
				throw new ArgumentOutOfRangeException();
			}
			OnRemove(index, this._items[index]);
			this._size--;
			if(index < this._size)
			{
				Array.Copy(this._items, index + 1, this._items, index, this._size - index);
			}
			this._items[this._size] = default(T);
			this._version++;
			OnRemoveComplete(index, this._items[index]);
		}

		public void RemoveRange(int index, int count)
		{
			if((index < 0) || (count < 0))
			{
				throw new ArgumentOutOfRangeException((index < 0) ? "index" : "count", "ArgumentOutOfRange_NeedNonNegNum");
			}
			if((this._size - index) < count)
			{
				throw new ArgumentException("Argument_InvalidOffLen");
			}
			if(count > 0)
			{
				this._size -= count;
				if(index < this._size)
				{
					Array.Copy(this._items, index + count, this._items, index, this._size - index);
				}
				Array.Clear(this._items, this._size, count);
				this._version++;
			}
		}

		public void Reverse()
		{
			this.Reverse(0, this.Count);
		}

		public void Reverse(int index, int count)
		{
			if((index < 0) || (count < 0))
			{
				throw new ArgumentOutOfRangeException((index < 0) ? "index" : "count", "ArgumentOutOfRange_NeedNonNegNum");
			}
			if((this._size - index) < count)
			{
				throw new ArgumentException("Argument_InvalidOffLen");
			}
			Array.Reverse(this._items, index, count);
			this._version++;
		}

		public void Sort()
		{
			this.Sort(0, this.Count, null);
		}

		public void Sort(IComparer<T> comparer)
		{
			this.Sort(0, this.Count, comparer);
		}

		public void Sort(Comparison<T> comparison)
		{
			if(comparison == null)
			{
				throw new ArgumentNullException("match");
			}
			if(this._size > 0)
			{
				IComparer<T> comparer = new FunctorComparer<T>(comparison);
				Array.Sort<T>(this._items, 0, this._size, comparer);
			}
		}

		public void Sort(int index, int count, IComparer<T> comparer)
		{
			if((index < 0) || (count < 0))
			{
				throw new ArgumentOutOfRangeException((index < 0) ? "index" : "count", "ArgumentOutOfRange_NeedNonNegNum");
			}
			if((this._size - index) < count)
			{
				throw new ArgumentException("Argument_InvalidOffLen");
			}
			Array.Sort<T>(this._items, index, count, comparer);
			this._version++;
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return new Enumerator<T>((GDAList<T>)this);
		}

		void ICollection.CopyTo(Array array, int arrayIndex)
		{
			if((array != null) && (array.Rank != 1))
			{
				throw new ArgumentException("Arg_RankMultiDimNotSupported");
			}
			try
			{
				Array.Copy(this._items, 0, array, arrayIndex, this._size);
			}
			catch(ArrayTypeMismatchException)
			{
				throw new ArgumentException("Argument_InvalidArrayType");
			}
			catch(InvalidCastException)
			{
				throw new ArgumentException("Argument_InvalidArrayType");
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumerator<T>((GDAList<T>)this);
		}

		int IList.Add(object item)
		{
			GDAList<T>.VerifyValueType(item);
			this.Add((T)item);
			return (this.Count - 1);
		}

		bool IList.Contains(object item)
		{
			if(GDAList<T>.IsCompatibleObject(item))
			{
				return this.Contains((T)item);
			}
			return false;
		}

		int IList.IndexOf(object item)
		{
			if(GDAList<T>.IsCompatibleObject(item))
			{
				return this.IndexOf((T)item);
			}
			return -1;
		}

		void IList.Insert(int index, object item)
		{
			GDAList<T>.VerifyValueType(item);
			this.Insert(index, (T)item);
		}

		void IList.Remove(object item)
		{
			if(GDAList<T>.IsCompatibleObject(item))
			{
				this.Remove((T)item);
			}
		}

		public T[] ToArray()
		{
			T[] destinationArray = new T[this._size];
			Array.Copy(this._items, 0, destinationArray, 0, this._size);
			return destinationArray;
		}

		public void TrimExcess()
		{
			int num = (int)(this._items.Length * 0.9);
			if(this._size < num)
			{
				this.Capacity = this._size;
			}
		}

		public bool TrueForAll(Predicate<T> match)
		{
			if(match == null)
			{
				throw new ArgumentNullException("match");
			}
			for(int i = 0; i < this._size; i++)
			{
				if(!match(this._items[i]))
				{
					return false;
				}
			}
			return true;
		}

		private static void VerifyValueType(object value)
		{
			if(!GDAList<T>.IsCompatibleObject(value))
			{
				throw new ArgumentException();
			}
		}

		public int Capacity
		{
			get
			{
				return this._items.Length;
			}
			set
			{
				if(value != this._items.Length)
				{
					if(value < this._size)
					{
						throw new ArgumentOutOfRangeException("value", "ArgumentOutOfRange_SmallCapacity");
					}
					if(value > 0)
					{
						T[] destinationArray = new T[value];
						if(this._size > 0)
						{
							Array.Copy(this._items, 0, destinationArray, 0, this._size);
						}
						this._items = destinationArray;
					}
					else
					{
						this._items = GDAList<T>._emptyArray;
					}
				}
			}
		}

		public int Count
		{
			get
			{
				return GetCount();
			}
		}

		internal protected virtual int GetCount()
		{
			return this._size;
		}

		public T this[int index]
		{
			get
			{
				return GetItem(index);
			}
			set
			{
				SetItem(index, value);
			}
		}

		internal protected virtual T GetItem(int index)
		{
			if(index >= this._size)
			{
				throw new ArgumentOutOfRangeException();
			}
			return this._items[index];
		}

		internal protected virtual void SetItem(int index, T value)
		{
			if(index >= this._size)
			{
				throw new ArgumentOutOfRangeException();
			}
			OnSet(index, this._items[index], value);
			this._items[index] = value;
			this._version++;
			OnSetComplete(index, value);
		}

		bool ICollection<T>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				if(this._syncRoot == null)
				{
					System.Threading.Interlocked.CompareExchange(ref this._syncRoot, new object(), null);
				}
				return this._syncRoot;
			}
		}

		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				GDAList<T>.VerifyValueType(value);
				this[index] = (T)value;
			}
		}

		public virtual void OnClear()
		{
		}

		public virtual void OnClearComplete()
		{
		}

		public virtual void OnInsert(int index, T value)
		{
		}

		public virtual void OnInsertComplete(int index, T value)
		{
		}

		public virtual void OnRemove(int index, T value)
		{
		}

		public virtual void OnRemoveComplete(int index, T value)
		{
		}

		public virtual void OnRemoveAll()
		{
		}

		public virtual void OnSet(int index, T oldValue, T newValue)
		{
		}

		public virtual void OnSetComplete(int index, T newValue)
		{
		}

		public struct Enumerator<T> : IEnumerator<T>, IDisposable, IEnumerator
		{
			private GDAList<T> list;

			private int index;

			private int version;

			private T current;

			internal Enumerator(GDAList<T> list)
			{
				this.list = list;
				this.index = 0;
				this.version = list._version;
				this.current = default(T);
			}

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				if(this.version != this.list._version)
				{
					throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");
				}
				if(this.index < this.list._size)
				{
					this.current = this.list._items[this.index];
					this.index++;
					return true;
				}
				this.index = this.list._size + 1;
				this.current = default(T);
				return false;
			}

			public T Current
			{
				get
				{
					return this.current;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					if((this.index == 0) || (this.index == (this.list._size + 1)))
					{
						throw new InvalidOperationException("InvalidOperation_EnumOpCantHappen");
					}
					return this.Current;
				}
			}

			void IEnumerator.Reset()
			{
				if(this.version != this.list._version)
				{
					throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");
				}
				this.index = 0;
				this.current = default(T);
			}
		}

		/// <summary>
		/// Converte implicitamente para um lista tipada.
		/// </summary>
		/// <param name="collection"></param>
		/// <returns>Lista tipada.</returns>
		public static implicit operator List<T>(GDAList<T> collection)
		{
			return new List<T>(collection);
		}
	}
}
