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
using System.Data;
using System.Collections;

namespace GDA
{
	/// <summary>
	/// Assinatura do listener da conexão do GDA.
	/// </summary>
	public abstract class GDAConnectionListener
	{
		/// <summary>
		/// Nome do listener.
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor protegido.
		/// </summary>
		protected GDAConnectionListener()
		{
		}

		/// <summary>
		/// Método acionado para notificar a criação da conexão.
		/// </summary>
		/// <param name="connection"></param>
		public abstract void NotifyConnectionCreated(IDbConnection connection);

		/// <summary>
		/// Método acionado para notificar que a conexão foi aberta.
		/// </summary>
		/// <param name="connection"></param>
		public abstract void NotifyConnectionOpened(IDbConnection connection);
	}
	/// <summary>
	/// Representa a coleção de listeners de conexão do GDA.
	/// </summary>
	public class GDAConnectionListenerCollection : IList, ICollection, IEnumerable
	{
		internal static readonly object _critSec = new object();

		private ArrayList _list = new ArrayList(1);

		/// <summary>
		/// Quantidade de itens na coleção.
		/// </summary>
		public int Count
		{
			get
			{
				return this._list.Count;
			}
		}

		/// <summary>
		/// Recupera o item na posição informada.
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public GDAConnectionListener this[int i]
		{
			get
			{
				return (GDAConnectionListener)_list[i];
			}
			set
			{
				this.InitializeListener(value);
				_list[i] = value;
			}
		}

		/// <summary>
		/// Recupera o item pelo nome informado.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public GDAConnectionListener this[string name]
		{
			get
			{
				foreach (GDAConnectionListener listener in this)
				{
					if(listener.Name == name)
						return listener;
				}
				return null;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		internal GDAConnectionListenerCollection()
		{
		}

		/// <summary>
		/// Adiciona o listener na coleção.
		/// </summary>
		/// <param name="listener"></param>
		/// <returns></returns>
		public int Add(GDAConnectionListener listener)
		{
			this.InitializeListener(listener);
			lock (_critSec)
				return _list.Add(listener);
		}

		/// <summary>
		/// Adiciona uma faixa de listeners na coleção.
		/// </summary>
		/// <param name="value"></param>
		public void AddRange(GDAConnectionListener[] value)
		{
			if(value == null)
				throw new ArgumentNullException("value");
			for(int i = 0; i < value.Length; i++)
				this.Add(value[i]);
		}

		/// <summary>
		/// Adiciona uma faixa de listeners na coleção.
		/// </summary>
		/// <param name="value"></param>
		public void AddRange(GDAConnectionListenerCollection value)
		{
			if(value == null)
				throw new ArgumentNullException("value");
			int count = value.Count;
			for(int i = 0; i < count; i++)
				this.Add(value[i]);
		}

		/// <summary>
		/// Limpa os itens da coleção.
		/// </summary>
		public void Clear()
		{
			_list = new ArrayList();
		}

		/// <summary>
		/// Verifica se a coleção contém o listener informado.
		/// </summary>
		/// <param name="listener"></param>
		/// <returns></returns>
		public bool Contains(GDAConnectionListener listener)
		{
			return ((IList)this).Contains(listener);
		}

		/// <summary>
		/// Copia os itens da coleção para o vetor informado.
		/// </summary>
		/// <param name="listeners"></param>
		/// <param name="index"></param>
		public void CopyTo(GDAConnectionListener[] listeners, int index)
		{
			((ICollection)this).CopyTo(listeners, index);
		}

		/// <summary>
		/// Recuper ao enumerador dos itens.
		/// </summary>
		/// <returns></returns>
		public IEnumerator GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		/// <summary>
		/// Recupera o indice do item na coleção.
		/// </summary>
		/// <param name="listener"></param>
		/// <returns></returns>
		public int IndexOf(GDAConnectionListener listener)
		{
			return ((IList)this).IndexOf(listener);
		}

		/// <summary>
		/// Inicializa o listener.
		/// </summary>
		/// <param name="listener"></param>
		internal void InitializeListener(GDAConnectionListener listener)
		{
			if(listener == null)
				throw new ArgumentNullException("listener");
		}

		/// <summary>
		/// Insere um item para a coleção na posição informada.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="listener"></param>
		public void Insert(int index, GDAConnectionListener listener)
		{
			this.InitializeListener(listener);
			lock (_critSec)
				_list.Insert(index, listener);
		}

		/// <summary>
		/// Remove o item da coleção.
		/// </summary>
		/// <param name="listener"></param>
		public void Remove(GDAConnectionListener listener)
		{
			((IList)this).Remove(listener);
		}

		/// <summary>
		/// Remove o item pelo nome informado.
		/// </summary>
		/// <param name="name"></param>
		public void Remove(string name)
		{
			GDAConnectionListener listener = this[name];
			if(listener != null)
				((IList)this).Remove(listener);
		}

		/// <summary>
		/// Remove o item na posição informada.
		/// </summary>
		/// <param name="index"></param>
		public void RemoveAt(int index)
		{
			lock (_critSec)
				_list.RemoveAt(index);
		}

		/// <summary>
		/// Copia os itens para o vetor informado.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		void ICollection.CopyTo(Array array, int index)
		{
			lock (_critSec)
				_list.CopyTo(array, index);
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
		/// objeto de sincronização.
		/// </summary>
		object ICollection.SyncRoot
		{
			get
			{
				return this;
			}
		}

		/// <summary>
		/// Adiciona um item na coleção.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		int IList.Add(object value)
		{
			GDAConnectionListener listener = value as GDAConnectionListener;
			if(listener == null)
				throw new ArgumentException("MustAddListener", "value");
			this.InitializeListener(listener);
			lock (_critSec)
				return _list.Add(value);
		}

		/// <summary>
		/// Verifica se o item informado está na coleção.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		bool IList.Contains(object value)
		{
			return this._list.Contains(value);
		}

		/// <summary>
		/// Recupera o indice do item informado.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		int IList.IndexOf(object value)
		{
			return this._list.IndexOf(value);
		}

		/// <summary>
		/// Insere o item na posição informada.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		void IList.Insert(int index, object value)
		{
			GDAConnectionListener listener = value as GDAConnectionListener;
			if(listener == null)
				throw new ArgumentException("MustAddListener", "value");
			this.InitializeListener(listener);
			lock (_critSec)
			{
				_list.Insert(index, value);
			}
		}

		/// <summary>
		/// Remove o item da coleção.
		/// </summary>
		/// <param name="value"></param>
		void IList.Remove(object value)
		{
			lock (_critSec)
			{
				_list.Remove(value);
			}
		}

		/// <summary>
		/// Identifica se a lista possui um tamanho fixo.
		/// </summary>
		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Identifica se a lista é somente leitura.
		/// </summary>
		bool IList.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Recupera o item pelo indice informado.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		object IList.this[int index]
		{
			get
			{
				return _list[index];
			}
			set
			{
				var listener = value as GDAConnectionListener;
				if(listener == null)
					throw new ArgumentException("MustAddListener", "value");
				this.InitializeListener(listener);
				_list[index] = listener;
			}
		}
	}
	/// <summary>
	/// Representa o gerenciador de conexão do GDA.
	/// </summary>
	public static class GDAConnectionManager
	{
		private static GDAConnectionListenerCollection _listeners;

		/// <summary>
		/// Notifica que a conexão foi criada.
		/// </summary>
		/// <param name="connection"></param>
		internal static void NotifyConnectionCreated(IDbConnection connection)
		{
			foreach (GDAConnectionListener listener in Listeners)
			{
				listener.NotifyConnectionCreated(connection);
			}
		}

		/// <summary>
		/// Notifica que a conexão foi aberta.
		/// </summary>
		/// <param name="connection"></param>
		internal static void NotifyConnectionOpened(IDbConnection connection)
		{
			foreach (GDAConnectionListener listener in Listeners)
			{
				listener.NotifyConnectionOpened(connection);
			}
		}

		/// <summary>
		/// Listener registrados.
		/// </summary>
		public static GDAConnectionListenerCollection Listeners
		{
			get
			{
				if(_listeners == null)
				{
					lock (GDAConnectionListenerCollection._critSec)
					{
						if(_listeners == null)
							_listeners = new GDAConnectionListenerCollection();
					}
				}
				return _listeners;
			}
		}
	}
}
