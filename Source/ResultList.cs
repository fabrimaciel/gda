using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Collections.ObjectModel;
namespace GDA.Sql
{
	public class LoadResultPageArgs<Model> : EventArgs where Model : new()
	{
		public int PageSize {
			get;
			private set;
		}
		public int StartRow {
			get;
			private set;
		}
		public GDASession Session {
			get;
			private set;
		}
		public ReadOnlyCollection<Model> Page {
			get;
			private set;
		}
		public LoadResultPageArgs (int a, int b, GDASession c, ReadOnlyCollection<Model> d)
		{
			PageSize = a;
			StartRow = b;
			Session = c;
			Page = d;
		}
	}
	public delegate void LoadResultPageHandler<Model> (object e, LoadResultPageArgs<Model> f) where Model : new();
	public class ResultList<Model> : IEnumerable<Model>, IEnumerable, IList<Model>, IList, ICollection<Model>, ICollection where Model : new()
	{
		internal protected BaseQuery _myQuery;
		internal protected Query _queryInstance;
		internal protected GDASession _myGDASession;
		private int _pageSize;
		private int _count;
		private IList<Model>[] _sessions;
		internal int _version;
		private object _syncRoot;
		private Func<Model, Model> _processMethod;
		public event LoadResultPageHandler<Model> LoadResultPage;
		public int Count {
			get {
				return _count;
			}
		}
		public bool IsReadOnly {
			get {
				return true;
			}
		}
		public ResultList (int a) : this (new Query (), null, a)
		{
		}
		public ResultList (BaseQuery a, int b) : this (a, null, b)
		{
		}
		public ResultList (BaseQuery a, GDASession b, int c)
		{
			if (a == null)
				throw new ArgumentNullException ("query");
			else if (c <= 0)
				throw new ArgumentException ("Page size cannot be less or equal zero.", "pageSize");
			_queryInstance = a as Query;
			_pageSize = c;
			_myQuery = a;
			_myGDASession = b;
			Refresh ();
		}
		internal protected virtual Model GetItem (int a)
		{
			if (a >= this.Count)
				throw new ArgumentOutOfRangeException ();
			int b = (int)Math.Floor (a / (double)_pageSize);
			if (_sessions [b] == null) {
				if (_processMethod != null) {
					var c = _processMethod;
					var d = new List<Model> ();
					foreach (var item in _myQuery.BaseTake (_pageSize).BaseSkip (b * _pageSize).ToCursor<Model> (_myGDASession))
						d.Add (c (item));
					_sessions [b] = d;
				}
				else
					_sessions [b] = new List<Model> (_myQuery.BaseTake (_pageSize).BaseSkip (b * _pageSize).ToCursor<Model> (_myGDASession));
				if (LoadResultPage != null)
					LoadResultPage (this, new LoadResultPageArgs<Model> (_pageSize, b * _pageSize, _myGDASession, new ReadOnlyCollection<Model> (_sessions [b])));
			}
			return _sessions [b] [a - (b * _pageSize)];
		}
		internal protected virtual void SetItem (int a, Model b)
		{
			if (a >= _count) {
				throw new ArgumentOutOfRangeException ();
			}
			int c = (int)Math.Floor (a / (double)_pageSize);
			OnSet (a, GetItem (a), b);
			_sessions [c] [a - (c * _pageSize)] = b;
			this._version++;
			OnSetComplete (a, b);
		}
		public void RegisterProcessMethod (Func<Model, Model> a)
		{
			_processMethod = a;
		}
		public void Refresh ()
		{
			if (_myQuery is Query)
				_count = (int)((Query)_myQuery).Count<Model> (_myGDASession);
			else if (_myQuery is SelectStatement)
				_count = (int)((SelectStatement)_myQuery).Count (_myGDASession);
			else if (_myQuery is NativeQuery)
				_count = (int)((NativeQuery)_myQuery).Count (_myGDASession);
			int a = (int)Math.Ceiling (_count / (double)_pageSize);
			if (_sessions != null) {
				for (int b = 0; b < _sessions.Length; b++) {
					if (_sessions [b] != null) {
						_sessions [b].Clear ();
						_sessions [b] = null;
					}
				}
			}
			_sessions = new IList<Model>[a];
			_version++;
			OnRefresh ();
		}
		public Model this [int a] {
			get {
				return GetItem (a);
			}
			set {
				SetItem (a, value);
			}
		}
		public void SetOrder (string a)
		{
			if (_queryInstance == null)
				throw new GDAException ("ResultList not support for Query {0}.", _myQuery.GetType ().FullName);
			_queryInstance.SetOrder (a);
		}
		public void ApplyFilter (string a)
		{
			if (_queryInstance == null)
				throw new GDAException ("ResultList not support for Query {0}.", _myQuery.GetType ().FullName);
			_queryInstance.SetWhere (a);
		}
		protected virtual void OnSet (int a, Model b, Model c)
		{
		}
		protected virtual void OnSetComplete (int a, Model b)
		{
		}
		protected virtual void OnRefresh ()
		{
		}
		IEnumerator<Model> IEnumerable<Model>.GetEnumerator ()
		{
			return new Enumerator<Model> ((ResultList<Model>)this);
		}
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return new Enumerator<Model> ((ResultList<Model>)this);
		}
		public void CopyTo (Array a, int b)
		{
			var c = 0;
			for (int d = b; d < a.Length && d < Count; d++)
				a.SetValue (this [d], c++);
		}
		public bool IsSynchronized {
			get {
				return false;
			}
		}
		public object SyncRoot {
			get {
				if (_syncRoot == null) {
					System.Threading.Interlocked.CompareExchange (ref _syncRoot, new object (), null);
				}
				return _syncRoot;
			}
		}
		public struct Enumerator<Model> : IEnumerator<Model>, IDisposable, IEnumerator where Model : new()
		{
			private ResultList<Model> list;
			private int index;
			private int version;
			private Model current;
			internal Enumerator (ResultList<Model> a)
			{
				this.list = a;
				this.index = 0;
				this.version = a._version;
				this.current = default(Model);
			}
			public void Dispose ()
			{
			}
			public bool MoveNext ()
			{
				if (this.version != this.list._version) {
					throw new InvalidOperationException ("InvalidOperation_EnumFailedVersion");
				}
				if (this.index < this.list.Count) {
					this.current = this.list [this.index];
					this.index++;
					return true;
				}
				this.index = this.list.Count + 1;
				this.current = default(Model);
				return false;
			}
			public Model Current {
				get {
					return this.current;
				}
			}
			object IEnumerator.Current {
				get {
					if ((this.index == 0) || (this.index == (this.list.Count + 1))) {
						throw new InvalidOperationException ("InvalidOperation_EnumOpCantHappen");
					}
					return this.Current;
				}
			}
			void IEnumerator.Reset ()
			{
				if (this.version != this.list._version) {
					throw new InvalidOperationException ("InvalidOperation_EnumFailedVersion");
				}
				this.index = 0;
				this.current = default(Model);
			}
		}
		void ICollection<Model>.Add (Model a)
		{
			throw new NotSupportedException ("Not supported readonly collection");
		}
		void ICollection<Model>.Clear ()
		{
			throw new NotSupportedException ("Not supported readonly collection");
		}
		bool ICollection<Model>.Contains (Model a)
		{
			throw new NotSupportedException ("Not supported readonly collection");
		}
		void ICollection<Model>.CopyTo (Model[] a, int b)
		{
			this.CopyTo (a, b);
		}
		int ICollection<Model>.Count {
			get {
				return this.Count;
			}
		}
		bool ICollection<Model>.IsReadOnly {
			get {
				return true;
			}
		}
		bool ICollection<Model>.Remove (Model a)
		{
			throw new NotSupportedException ("Not supported readonly collection");
		}
		int IList<Model>.IndexOf (Model a)
		{
			throw new NotSupportedException ("Not supported IndexOf");
		}
		void IList<Model>.Insert (int a, Model b)
		{
			throw new NotSupportedException ("Not supported readonly collection");
		}
		void IList<Model>.RemoveAt (int a)
		{
			throw new NotSupportedException ("Not supported readonly collection");
		}
		Model IList<Model>.this [int index] {
			get {
				return GetItem (index);
			}
			set {
				SetItem (index, value);
			}
		}
		int IList.Add (object a)
		{
			throw new NotSupportedException ("Not supported readonly collection");
		}
		void IList.Clear ()
		{
			throw new NotSupportedException ("Not supported readonly collection");
		}
		bool IList.Contains (object a)
		{
			throw new NotSupportedException ("Not supported Contains");
		}
		int IList.IndexOf (object a)
		{
			throw new NotSupportedException ("Not supported IndexOf");
		}
		void IList.Insert (int a, object b)
		{
			throw new NotSupportedException ("Not supported readonly collection");
		}
		bool IList.IsFixedSize {
			get {
				return false;
			}
		}
		bool IList.IsReadOnly {
			get {
				return true;
			}
		}
		void IList.Remove (object a)
		{
			throw new NotSupportedException ("Not supported readonly collection");
		}
		void IList.RemoveAt (int index)
		{
			throw new NotSupportedException ("Not supported readonly collection");
		}
		object IList.this [int index] {
			get {
				return this [index];
			}
			set {
				this [index] = (Model)value;
			}
		}
		void ICollection.CopyTo (Array array, int index)
		{
			this.CopyTo (array, index);
		}
		int ICollection.Count {
			get {
				return this.Count;
			}
		}
		bool ICollection.IsSynchronized {
			get {
				return this.IsSynchronized;
			}
		}
		object ICollection.SyncRoot {
			get {
				return this.SyncRoot;
			}
		}
	}
}
