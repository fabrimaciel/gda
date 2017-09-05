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
using System.Linq;
using System.Text;
using System.Collections;

namespace GDA.Diagnostics
{
	/// <summary>
	/// Implementação da coleção de listeners.
	/// </summary>
	public class GDATraceListenerCollection : IList, ICollection, IEnumerable
	{
		private ArrayList _list = new ArrayList(1);

		/// <summary>
		/// Recupera a quantidade de listener na coleção.
		/// </summary>
		public int Count
		{
			get
			{
				return this._list.Count;
			}
		}

		/// <summary>
		/// Recupera o listener na posição informada.
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public GDATraceListener this[int i]
		{
			get
			{
				return (GDATraceListener)this._list[i];
			}
			set
			{
				this.InitializeListener(value);
				this._list[i] = value;
			}
		}

		/// <summary>
		/// Recupera o listener pelo nome informado.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public GDATraceListener this[string name]
		{
			get
			{
				foreach (GDATraceListener listener in this)
				{
					if(listener.Name == name)
					{
						return listener;
					}
				}
				return null;
			}
		}

		/// <summary>
		/// Identifica se é uma coleção sincronizada.
		/// </summary>
		bool ICollection.IsSynchronized
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Objeto para sincronizar a coleção.
		/// </summary>
		object ICollection.SyncRoot
		{
			get
			{
				return this;
			}
		}

		/// <summary>
		/// Identifica se a coleção possui um tamanho fixo.
		/// </summary>
		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Identifica se a coleção é somente leitura.
		/// </summary>
		bool IList.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Recupera o listener na posição informada.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		object IList.this[int index]
		{
			get
			{
				return this._list[index];
			}
			set
			{
				GDATraceListener listener = value as GDATraceListener;
				if(listener == null)
				{
					throw new ArgumentException("MustAddListener", "value");
				}
				this.InitializeListener(listener);
				this._list[index] = listener;
			}
		}

		internal GDATraceListenerCollection()
		{
		}

		/// <summary>
		/// Adiciona um listener para a coleção.
		/// </summary>
		/// <param name="listener"></param>
		/// <returns></returns>
		public int Add(GDATraceListener listener)
		{
			this.InitializeListener(listener);
			lock (GDATraceInternal._critSec)
				return this._list.Add(listener);
		}

		/// <summary>
		/// Adiciona um faixa de listeners
		/// </summary>
		/// <param name="value"></param>
		public void AddRange(GDATraceListener[] value)
		{
			if(value == null)
				throw new ArgumentNullException("value");
			for(int i = 0; i < value.Length; i++)
				this.Add(value[i]);
		}

		/// <summary>
		/// Adiciona a faixa de valores contidos na coleção informada.
		/// </summary>
		/// <param name="value"></param>
		public void AddRange(GDATraceListenerCollection value)
		{
			if(value == null)
				throw new ArgumentNullException("value");
			int count = value.Count;
			for(int i = 0; i < count; i++)
				this.Add(value[i]);
		}

		/// <summary>
		/// Limpa a coleção.
		/// </summary>
		public void Clear()
		{
			this._list = new ArrayList();
		}

		/// <summary>
		/// Verifica se na coleção possui o listener informado.
		/// </summary>
		/// <param name="listener"></param>
		/// <returns></returns>
		public bool Contains(GDATraceListener listener)
		{
			return ((IList)this).Contains(listener);
		}

		/// <summary>
		/// Copia os listeners da coleção para o vetor informado.
		/// </summary>
		/// <param name="listeners"></param>
		/// <param name="index"></param>
		public void CopyTo(GDATraceListener[] listeners, int index)
		{
			((ICollection)this).CopyTo(listeners, index);
		}

		/// <summary>
		/// Recupera o enumerador dos listeners.
		/// </summary>
		/// <returns></returns>
		public IEnumerator GetEnumerator()
		{
			return this._list.GetEnumerator();
		}

		/// <summary>
		/// Recupera o indice do listener na coleção.
		/// </summary>
		/// <param name="listener"></param>
		/// <returns></returns>
		public int IndexOf(GDATraceListener listener)
		{
			return ((IList)this).IndexOf(listener);
		}

		/// <summary>
		/// Inicializa o listener.
		/// </summary>
		/// <param name="listener"></param>
		internal void InitializeListener(GDATraceListener listener)
		{
			if(listener == null)
				throw new ArgumentNullException("listener");
		}

		/// <summary>
		/// Adiciona o listener na posição informada.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="listener"></param>
		public void Insert(int index, GDATraceListener listener)
		{
			this.InitializeListener(listener);
			lock (GDATraceInternal._critSec)
			{
				this._list.Insert(index, listener);
			}
		}

		/// <summary>
		/// Remove o listener informado.
		/// </summary>
		/// <param name="listener"></param>
		public void Remove(GDATraceListener listener)
		{
			((IList)this).Remove(listener);
		}

		/// <summary>
		/// Remove o listener com o nome informado.
		/// </summary>
		/// <param name="name"></param>
		public void Remove(string name)
		{
			GDATraceListener listener = this[name];
			if(listener != null)
			{
				((IList)this).Remove(listener);
			}
		}

		/// <summary>
		/// Remove o listener a posição informada.
		/// </summary>
		/// <param name="index"></param>
		public void RemoveAt(int index)
		{
			lock (GDATraceInternal._critSec)
			{
				this._list.RemoveAt(index);
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			lock (GDATraceInternal._critSec)
			{
				this._list.CopyTo(array, index);
			}
		}

		int IList.Add(object value)
		{
			GDATraceListener listener = value as GDATraceListener;
			if(listener == null)
				throw new ArgumentException("MustAddListener", "value");
			this.InitializeListener(listener);
			lock (GDATraceInternal._critSec)
			{
				return this._list.Add(value);
			}
		}

		bool IList.Contains(object value)
		{
			return this._list.Contains(value);
		}

		int IList.IndexOf(object value)
		{
			return this._list.IndexOf(value);
		}

		void IList.Insert(int index, object value)
		{
			GDATraceListener listener = value as GDATraceListener;
			if(listener == null)
			{
				throw new ArgumentException("MustAddListener", "value");
			}
			this.InitializeListener(listener);
			lock (GDATraceInternal._critSec)
			{
				this._list.Insert(index, value);
			}
		}

		void IList.Remove(object value)
		{
			lock (GDATraceInternal._critSec)
			{
				this._list.Remove(value);
			}
		}
	}
}
