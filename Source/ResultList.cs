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

namespace GDA.Sql
{
	/// <summary>
	/// Representa os argumentos para o evento de carga
	/// da página de resultados.
	/// </summary>
	public class LoadResultPageArgs<Model> : EventArgs where Model : new()
	{
		/// <summary>
		/// Tamanho da página solicitada
		/// </summary>
		public int PageSize
		{
			get;
			private set;
		}

		/// <summary>
		/// Linha inicial da recuperação da página.
		/// </summary>
		public int StartRow
		{
			get;
			private set;
		}

		/// <summary>
		/// Sessão de conexão do GDA que foi usada na recuperação da página.
		/// </summary>
		public GDASession Session
		{
			get;
			private set;
		}

		/// <summary>
		/// Elementos da página.
		/// </summary>
		public ReadOnlyCollection<Model> Page
		{
			get;
			private set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="pageSize"></param>
		/// <param name="startRow"></param>
		/// <param name="session"></param>
		/// <param name="page"></param>
		public LoadResultPageArgs(int pageSize, int startRow, GDASession session, ReadOnlyCollection<Model> page)
		{
			PageSize = pageSize;
			StartRow = startRow;
			Session = session;
			Page = page;
		}
	}
	/// <summary>
	/// Representa o evento acionado quando uma página de dados é carregada.
	/// </summary>
	/// <typeparam name="Model">Tipo do item da página de dados.</typeparam>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void LoadResultPageHandler<Model> (object sender, LoadResultPageArgs<Model> e) where Model : new();
	/// <summary>
	/// Lista virtual para resultados de uma consulta.
	/// </summary>
	/// <typeparam name="Model"></typeparam>
	public class ResultList<Model> : IEnumerable<Model>, IEnumerable, IList<Model>, IList, ICollection<Model>, ICollection where Model : new()
	{
		/// <summary>
		/// Instância da consulta usada na lista.
		/// </summary>
		internal protected BaseQuery _myQuery;

		/// <summary>
		/// Instancia da consulta associada.
		/// </summary>
		internal protected Query _queryInstance;

		/// <summary>
		/// Sessão do GDA usada para fazer as consultas.
		/// </summary>
		internal protected GDASession _myGDASession;

		/// <summary>
		/// Tamanho da página de cada sessão.
		/// </summary>
		private int _pageSize;

		/// <summary>
		/// Quantidade de elementos da lista.
		/// </summary>
		private int _count;

		/// <summary>
		/// Sessões onde são armazenados os elementos carregados na lista
		/// </summary>
		private IList<Model>[] _sessions;

		/// <summary>
		/// Versão da lista.
		/// </summary>
		internal int _version;

		/// <summary>
		/// Objeto usadao para garantir a sincronização da lista.
		/// </summary>
		private object _syncRoot;

		private Func<Model, Model> _processMethod;

		/// <summary>
		/// Evento acionado quando uma página de resultado é carregada pela lista.
		/// </summary>
		public event LoadResultPageHandler<Model> LoadResultPage;

		public int Count
		{
			get
			{
				return _count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Construtor.
		/// </summary>
		/// <param name="pageSize">Tamanho da página da lista</param>
		public ResultList(int pageSize) : this(new Query(), null, pageSize)
		{
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="query">Consulta.</param>
		/// <param name="pageSize">Tamanho da página da consulta.</param>
		public ResultList(BaseQuery query, int pageSize) : this(query, null, pageSize)
		{
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="query">Consulta.</param>
		/// <param name="session">Sessão usada nas consultas.</param>
		/// <param name="pageSize">Tamanho da página da consulta.</param>
		public ResultList(BaseQuery query, GDASession session, int pageSize)
		{
			if(query == null)
				throw new ArgumentNullException("query");
			else if(pageSize <= 0)
				throw new ArgumentException("Page size cannot be less or equal zero.", "pageSize");
			_queryInstance = query as Query;
			_pageSize = pageSize;
			_myQuery = query;
			_myGDASession = session;
			Refresh();
		}

		/// <summary>
		/// Recupera o item na posição informada.
		/// </summary>
		/// <param name="index">Posição do item.</param>
		/// <returns></returns>
		internal protected virtual Model GetItem(int index)
		{
			if(index >= this.Count)
				throw new ArgumentOutOfRangeException();
			int indexSession = (int)Math.Floor(index / (double)_pageSize);
			if(_sessions[indexSession] == null)
			{
				if(_processMethod != null)
				{
					var method = _processMethod;
					var items = new List<Model>();
					foreach (var item in _myQuery.BaseTake(_pageSize).BaseSkip(indexSession * _pageSize).ToCursor<Model>(_myGDASession))
						items.Add(method(item));
					_sessions[indexSession] = items;
				}
				else
					_sessions[indexSession] = new List<Model>(_myQuery.BaseTake(_pageSize).BaseSkip(indexSession * _pageSize).ToCursor<Model>(_myGDASession));
				if(LoadResultPage != null)
					LoadResultPage(this, new LoadResultPageArgs<Model>(_pageSize, indexSession * _pageSize, _myGDASession, new ReadOnlyCollection<Model>(_sessions[indexSession])));
			}
			return _sessions[indexSession][index - (indexSession * _pageSize)];
		}

		/// <summary>
		/// Define o item na posição informada.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		internal protected virtual void SetItem(int index, Model value)
		{
			if(index >= _count)
			{
				throw new ArgumentOutOfRangeException();
			}
			int indexSession = (int)Math.Floor(index / (double)_pageSize);
			OnSet(index, GetItem(index), value);
			_sessions[indexSession][index - (indexSession * _pageSize)] = value;
			this._version++;
			OnSetComplete(index, value);
		}

		/// <summary>
		/// Registra o método que será responsável por processar os itens carregados na coleção.
		/// </summary>
		/// <param name="method"></param>
		public void RegisterProcessMethod(Func<Model, Model> method)
		{
			_processMethod = method;
		}

		/// <summary>
		/// Atualiza os dados da lista.
		/// </summary>
		public void Refresh()
		{
			if(_myQuery is Query)
				_count = (int)((Query)_myQuery).Count<Model>(_myGDASession);
			else if(_myQuery is SelectStatement)
				_count = (int)((SelectStatement)_myQuery).Count(_myGDASession);
			else if(_myQuery is NativeQuery)
				_count = (int)((NativeQuery)_myQuery).Count(_myGDASession);
			int numberSessions = (int)Math.Ceiling(_count / (double)_pageSize);
			if(_sessions != null)
			{
				for(int i = 0; i < _sessions.Length; i++)
				{
					if(_sessions[i] != null)
					{
						_sessions[i].Clear();
						_sessions[i] = null;
					}
				}
			}
			_sessions = new IList<Model>[numberSessions];
			_version++;
			OnRefresh();
		}

		/// <summary>
		/// Recupera o item na posição informada.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public Model this[int index]
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

		/// <summary>
		/// Define a ordenação da lista.
		/// </summary>
		/// <param name="order"></param>
		public void SetOrder(string order)
		{
			if(_queryInstance == null)
				throw new GDAException("ResultList not support for Query {0}.", _myQuery.GetType().FullName);
			_queryInstance.SetOrder(order);
		}

		/// <summary>
		/// Aplica um filtro para carregar os dados da lista.
		/// </summary>
		/// <remarks>O filtro só será aplicado realmente após a chamada do método refresh.</remarks>
		/// <param name="filter"></param>
		public void ApplyFilter(string filter)
		{
			if(_queryInstance == null)
				throw new GDAException("ResultList not support for Query {0}.", _myQuery.GetType().FullName);
			_queryInstance.SetWhere(filter);
		}

		protected virtual void OnSet(int index, Model oldValue, Model newValue)
		{
		}

		protected virtual void OnSetComplete(int index, Model newValue)
		{
		}

		/// <summary>
		/// Método acionado quando se dar um refresh na lista.
		/// </summary>
		protected virtual void OnRefresh()
		{
		}

		/// <summary>
		/// Recupera o enumerator da lista.
		/// </summary>
		/// <returns></returns>
		IEnumerator<Model> IEnumerable<Model>.GetEnumerator()
		{
			return new Enumerator<Model>((ResultList<Model>)this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumerator<Model>((ResultList<Model>)this);
		}

		public void CopyTo(Array array, int arrayIndex)
		{
			var index = 0;
			for(int i = arrayIndex; i < array.Length && i < Count; i++)
				array.SetValue(this[i], index++);
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public object SyncRoot
		{
			get
			{
				if(_syncRoot == null)
				{
					System.Threading.Interlocked.CompareExchange(ref _syncRoot, new object(), null);
				}
				return _syncRoot;
			}
		}

		/// <summary>
		/// Enumerator usado na lista.
		/// </summary>
		/// <typeparam name="Model"></typeparam>
		public struct Enumerator<Model> : IEnumerator<Model>, IDisposable, IEnumerator where Model : new()
		{
			/// <summary>
			/// Lista em questão.
			/// </summary>
			private ResultList<Model> list;

			/// <summary>
			/// Atual index.
			/// </summary>
			private int index;

			/// <summary>
			/// Versão atual da lista
			/// </summary>
			private int version;

			/// <summary>
			/// Objeto atualmente selecionado.
			/// </summary>
			private Model current;

			internal Enumerator(ResultList<Model> list)
			{
				this.list = list;
				this.index = 0;
				this.version = list._version;
				this.current = default(Model);
			}

			public void Dispose()
			{
			}

			/// <summary>
			/// Movimenta para o proximo objeto.
			/// </summary>
			/// <returns></returns>
			public bool MoveNext()
			{
				if(this.version != this.list._version)
				{
					throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");
				}
				if(this.index < this.list.Count)
				{
					this.current = this.list[this.index];
					this.index++;
					return true;
				}
				this.index = this.list.Count + 1;
				this.current = default(Model);
				return false;
			}

			/// <summary>
			/// Atual objeto selecionado.
			/// </summary>
			public Model Current
			{
				get
				{
					return this.current;
				}
			}

			/// <summary>
			/// Atual objeto selecionado.
			/// </summary>
			object IEnumerator.Current
			{
				get
				{
					if((this.index == 0) || (this.index == (this.list.Count + 1)))
					{
						throw new InvalidOperationException("InvalidOperation_EnumOpCantHappen");
					}
					return this.Current;
				}
			}

			/// <summary>
			/// Reseta a lista.
			/// </summary>
			void IEnumerator.Reset()
			{
				if(this.version != this.list._version)
				{
					throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");
				}
				this.index = 0;
				this.current = default(Model);
			}
		}

		void ICollection<Model>.Add(Model item)
		{
			throw new NotSupportedException("Not supported readonly collection");
		}

		void ICollection<Model>.Clear()
		{
			throw new NotSupportedException("Not supported readonly collection");
		}

		bool ICollection<Model>.Contains(Model item)
		{
			throw new NotSupportedException("Not supported readonly collection");
		}

		void ICollection<Model>.CopyTo(Model[] array, int arrayIndex)
		{
			this.CopyTo(array, arrayIndex);
		}

		int ICollection<Model>.Count
		{
			get
			{
				return this.Count;
			}
		}

		bool ICollection<Model>.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		bool ICollection<Model>.Remove(Model item)
		{
			throw new NotSupportedException("Not supported readonly collection");
		}

		int IList<Model>.IndexOf(Model item)
		{
			throw new NotSupportedException("Not supported IndexOf");
		}

		void IList<Model>.Insert(int index, Model item)
		{
			throw new NotSupportedException("Not supported readonly collection");
		}

		void IList<Model>.RemoveAt(int index)
		{
			throw new NotSupportedException("Not supported readonly collection");
		}

		Model IList<Model>.this[int index]
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

		int IList.Add(object value)
		{
			throw new NotSupportedException("Not supported readonly collection");
		}

		void IList.Clear()
		{
			throw new NotSupportedException("Not supported readonly collection");
		}

		bool IList.Contains(object value)
		{
			throw new NotSupportedException("Not supported Contains");
		}

		int IList.IndexOf(object value)
		{
			throw new NotSupportedException("Not supported IndexOf");
		}

		void IList.Insert(int index, object value)
		{
			throw new NotSupportedException("Not supported readonly collection");
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
				return true;
			}
		}

		void IList.Remove(object value)
		{
			throw new NotSupportedException("Not supported readonly collection");
		}

		void IList.RemoveAt(int index)
		{
			throw new NotSupportedException("Not supported readonly collection");
		}

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				this[index] = (Model)value;
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			this.CopyTo(array, index);
		}

		int ICollection.Count
		{
			get
			{
				return this.Count;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return this.IsSynchronized;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this.SyncRoot;
			}
		}
	}
}
