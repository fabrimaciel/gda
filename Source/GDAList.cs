using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Collections.ObjectModel;
namespace GDA.Collections
{
	[System.Diagnostics.DebuggerDisplay ("Count = {Count}")]
	public class GDAList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable
	{
		private static T[] _emptyArray;
		private T[] _items;
		private int _size;
		private object _syncRoot;
		private int _version;
		static GDAList ()
		{
			GDAList<T>._emptyArray = new T[0];
		}
		public GDAList ()
		{
			this._items = GDAList<T>._emptyArray;
		}
		public GDAList (IEnumerable<T> a)
		{
			if (a == null) {
				throw new ArgumentNullException ("collection");
			}
			ICollection<T> b = a as ICollection<T>;
			if (b != null) {
				int c = b.Count;
				this._items = new T[c];
				b.CopyTo (this._items, 0);
				this._size = c;
			}
			else {
				this._size = 0;
				this._items = new T[4];
				using (IEnumerator<T> d = a.GetEnumerator ())
					while (d.MoveNext ())
						this.Add (d.Current);
			}
		}
		public GDAList (int a)
		{
			if (a < 0) {
				throw new ArgumentOutOfRangeException ("capacity", "ArgumentOutOfRange_SmallCapacity");
			}
			this._items = new T[a];
		}
		public void Add (T a)
		{
			OnInsert (this._size, a);
			if (this._size == this._items.Length) {
				this.EnsureCapacity (this._size + 1);
			}
			this._items [this._size++] = a;
			this._version++;
			OnInsertComplete (this._size - 1, a);
		}
		public void AddRange (IEnumerable<T> a)
		{
			this.InsertRange (this._size, a);
		}
		public ReadOnlyCollection<T> AsReadOnly ()
		{
			return new ReadOnlyCollection<T> (this);
		}
		public int BinarySearch (T a)
		{
			return this.BinarySearch (0, this.Count, a, null);
		}
		public int BinarySearch (T a, IComparer<T> b)
		{
			return this.BinarySearch (0, this.Count, a, b);
		}
		public int BinarySearch (int a, int b, T c, IComparer<T> d)
		{
			if ((a < 0) || (b < 0)) {
				throw new ArgumentOutOfRangeException ((a < 0) ? "index" : "count", "ArgumentOutOfRange_NeedNonNegNum");
			}
			if ((this._size - a) < b) {
				throw new ArgumentException ("Argument_InvalidOffLen");
			}
			return Array.BinarySearch<T> (this._items, a, b, c, d);
		}
		public void Clear ()
		{
			OnClear ();
			Array.Clear (this._items, 0, this._size);
			this._size = 0;
			this._version++;
			OnClearComplete ();
		}
		public bool Contains (T a)
		{
			if (a == null) {
				for (int b = 0; b < this._size; b++) {
					if (this._items [b] == null) {
						return true;
					}
				}
				return false;
			}
			EqualityComparer<T> c = EqualityComparer<T>.Default;
			for (int d = 0; d < this._size; d++) {
				if (c.Equals (this._items [d], a)) {
					return true;
				}
			}
			return false;
		}
		public GDAList<TOutput> ConvertAll<TOutput> (Converter<T, TOutput> a)
		{
			if (a == null) {
				throw new ArgumentNullException ("converter");
			}
			GDAList<TOutput> b = new GDAList<TOutput> (this._size);
			for (int c = 0; c < this._size; c++) {
				b._items [c] = a (this._items [c]);
			}
			b._size = this._size;
			return b;
		}
		public void CopyTo (T[] a)
		{
			this.CopyTo (a, 0);
		}
		public void CopyTo (T[] a, int b)
		{
			try {
				Array.Copy (this._items, 0, a, b, this._size);
			}
			catch (InvalidCastException) {
				throw new ArgumentException ("Argument_InvalidArrayType");
			}
		}
		public void CopyTo (int a, T[] b, int c, int d)
		{
			if ((this._size - a) < d) {
				throw new ArgumentException ("Argument_InvalidOffLen");
			}
			try {
				Array.Copy (this._items, a, b, c, d);
			}
			catch (InvalidCastException) {
				throw new ArgumentException ("Argument_InvalidArrayType");
			}
		}
		private void EnsureCapacity (int a)
		{
			if (this._items.Length < a) {
				int b = (this._items.Length == 0) ? 4 : (this._items.Length * 2);
				if (b < a) {
					b = a;
				}
				this.Capacity = b;
			}
		}
		public bool Exists (Predicate<T> a)
		{
			return (this.FindIndex (a) != -1);
		}
		public T Find (Predicate<T> a)
		{
			if (a == null) {
				throw new ArgumentNullException ("match");
			}
			for (int b = 0; b < this._size; b++) {
				if (a (this._items [b])) {
					return this._items [b];
				}
			}
			return default(T);
		}
		public GDAList<T> FindAll (Predicate<T> a)
		{
			if (a == null) {
				throw new ArgumentNullException ("match");
			}
			GDAList<T> b = new GDAList<T> ();
			for (int c = 0; c < this._size; c++) {
				if (a (this._items [c])) {
					b.Add (this._items [c]);
				}
			}
			return b;
		}
		public int FindIndex (Predicate<T> a)
		{
			return this.FindIndex (0, this._size, a);
		}
		public int FindIndex (int a, Predicate<T> b)
		{
			return this.FindIndex (a, this._size - a, b);
		}
		public int FindIndex (int a, int b, Predicate<T> c)
		{
			if (a > this._size) {
				throw new ArgumentOutOfRangeException ("startIndex", "ArgumentOutOfRange_Index");
			}
			if ((b < 0) || (a > (this._size - b))) {
				throw new ArgumentOutOfRangeException ("count", "ArgumentOutOfRange_Count");
			}
			if (c == null) {
				throw new ArgumentNullException ("match");
			}
			int d = a + b;
			for (int e = a; e < d; e++) {
				if (c (this._items [e])) {
					return e;
				}
			}
			return -1;
		}
		public T FindLast (Predicate<T> a)
		{
			if (a == null) {
				throw new ArgumentNullException ("match");
			}
			for (int b = this._size - 1; b >= 0; b--) {
				if (a (this._items [b])) {
					return this._items [b];
				}
			}
			return default(T);
		}
		public int FindLastIndex (Predicate<T> a)
		{
			return this.FindLastIndex (this._size - 1, this._size, a);
		}
		public int FindLastIndex (int a, Predicate<T> b)
		{
			return this.FindLastIndex (a, a + 1, b);
		}
		public int FindLastIndex (int a, int b, Predicate<T> c)
		{
			if (c == null) {
				throw new ArgumentNullException ("match");
			}
			if (this._size == 0) {
				if (a != -1) {
					throw new ArgumentOutOfRangeException ("startIndex", "ArgumentOutOfRange_Index");
				}
			}
			else if (a >= this._size) {
				throw new ArgumentOutOfRangeException ("startIndex", "ArgumentOutOfRange_Index");
			}
			if ((b < 0) || (((a - b) + 1) < 0)) {
				throw new ArgumentOutOfRangeException ("count", "ArgumentOutOfRange_Count");
			}
			int d = a - b;
			for (int e = a; e > d; e--) {
				if (c (this._items [e])) {
					return e;
				}
			}
			return -1;
		}
		public void ForEach (Action<T> a)
		{
			if (a == null) {
				throw new ArgumentNullException ("match");
			}
			for (int b = 0; b < this._size; b++) {
				a (this._items [b]);
			}
		}
		public Enumerator<T> GetEnumerator ()
		{
			return new Enumerator<T> ((GDAList<T>)this);
		}
		public GDAList<T> GetRange (int a, int b)
		{
			if ((a < 0) || (b < 0)) {
				throw new ArgumentOutOfRangeException ((a < 0) ? "index" : "count", "ArgumentOutOfRange_NeedNonNegNum");
			}
			if ((this._size - a) < b) {
				throw new ArgumentException ("Argument_InvalidOffLen");
			}
			GDAList<T> c = new GDAList<T> (b);
			Array.Copy (this._items, a, c._items, 0, b);
			c._size = b;
			return c;
		}
		public int IndexOf (T a)
		{
			return Array.IndexOf<T> (this._items, a, 0, this._size);
		}
		public int IndexOf (T a, int b)
		{
			if (b > this._size) {
				throw new ArgumentOutOfRangeException ("index", "ArgumentOutOfRange_Index");
			}
			return Array.IndexOf<T> (this._items, a, b, this._size - b);
		}
		public int IndexOf (T a, int b, int c)
		{
			if (b > this._size) {
				throw new ArgumentOutOfRangeException ("index", "ArgumentOutOfRange_Index");
			}
			if ((c < 0) || (b > (this._size - c))) {
				throw new ArgumentOutOfRangeException ("count", "ArgumentOutOfRange_Count");
			}
			return Array.IndexOf<T> (this._items, a, b, c);
		}
		public void Insert (int a, T b)
		{
			OnInsert (a, b);
			if (a > this._size) {
				throw new ArgumentOutOfRangeException ("index", "ArgumentOutOfRange_ListInsert");
			}
			if (this._size == this._items.Length) {
				this.EnsureCapacity (this._size + 1);
			}
			if (a < this._size) {
				Array.Copy (this._items, a, this._items, a + 1, this._size - a);
			}
			this._items [a] = b;
			this._size++;
			this._version++;
			OnInsertComplete (a, b);
		}
		public void InsertRange (int a, IEnumerable<T> b)
		{
			if (b == null) {
				throw new ArgumentNullException ("collection");
			}
			if (a > this._size) {
				throw new ArgumentOutOfRangeException ("index", "ArgumentOutOfRange_Index");
			}
			ICollection<T> c = b as ICollection<T>;
			if (c != null) {
				int d = c.Count;
				if (d > 0) {
					this.EnsureCapacity (this._size + d);
					if (a < this._size) {
						Array.Copy (this._items, a, this._items, a + d, this._size - a);
					}
					if (this == c) {
						Array.Copy (this._items, 0, this._items, a, a);
						Array.Copy (this._items, a + d, this._items, a * 2, this._size - a);
					}
					else {
						T[] e = new T[d];
						c.CopyTo (e, 0);
						e.CopyTo (this._items, a);
					}
					this._size += d;
				}
			}
			else {
				using (IEnumerator<T> f = b.GetEnumerator ()) {
					while (f.MoveNext ()) {
						this.Insert (a++, f.Current);
					}
				}
			}
			this._version++;
		}
		private static bool IsCompatibleObject (object a)
		{
			if (!(a is T) && ((a != null) || typeof(T).IsValueType)) {
				return false;
			}
			return true;
		}
		public int LastIndexOf (T a)
		{
			return this.LastIndexOf (a, this._size - 1, this._size);
		}
		public int LastIndexOf (T a, int b)
		{
			if (b >= this._size) {
				throw new ArgumentOutOfRangeException ("index", "ArgumentOutOfRange_Index");
			}
			return this.LastIndexOf (a, b, b + 1);
		}
		public int LastIndexOf (T a, int b, int c)
		{
			if (this._size == 0) {
				return -1;
			}
			if ((b < 0) || (c < 0)) {
				throw new ArgumentOutOfRangeException ((b < 0) ? "index" : "count", "ArgumentOutOfRange_NeedNonNegNum");
			}
			if ((b >= this._size) || (c > (b + 1))) {
				throw new ArgumentOutOfRangeException ((b >= this._size) ? "index" : "count", "ArgumentOutOfRange_BiggerThanCollection");
			}
			return Array.LastIndexOf<T> (this._items, a, b, c);
		}
		public bool Remove (T a)
		{
			int b = this.IndexOf (a);
			if (b >= 0) {
				this.RemoveAt (b);
				return true;
			}
			return false;
		}
		public int RemoveAll (Predicate<T> a)
		{
			if (a == null) {
				throw new ArgumentNullException ("match");
			}
			int b = 0;
			while ((b < this._size) && !a (this._items [b])) {
				b++;
			}
			if (b >= this._size) {
				return 0;
			}
			int c = b + 1;
			while (c < this._size) {
				while ((c < this._size) && a (this._items [c])) {
					c++;
				}
				if (c < this._size) {
					this._items [b++] = this._items [c++];
				}
			}
			Array.Clear (this._items, b, this._size - b);
			int d = this._size - b;
			this._size = b;
			this._version++;
			OnRemoveAll ();
			return d;
		}
		public void RemoveAt (int a)
		{
			if (a >= this._size) {
				throw new ArgumentOutOfRangeException ();
			}
			OnRemove (a, this._items [a]);
			this._size--;
			if (a < this._size) {
				Array.Copy (this._items, a + 1, this._items, a, this._size - a);
			}
			this._items [this._size] = default(T);
			this._version++;
			OnRemoveComplete (a, this._items [a]);
		}
		public void RemoveRange (int a, int b)
		{
			if ((a < 0) || (b < 0)) {
				throw new ArgumentOutOfRangeException ((a < 0) ? "index" : "count", "ArgumentOutOfRange_NeedNonNegNum");
			}
			if ((this._size - a) < b) {
				throw new ArgumentException ("Argument_InvalidOffLen");
			}
			if (b > 0) {
				this._size -= b;
				if (a < this._size) {
					Array.Copy (this._items, a + b, this._items, a, this._size - a);
				}
				Array.Clear (this._items, this._size, b);
				this._version++;
			}
		}
		public void Reverse ()
		{
			this.Reverse (0, this.Count);
		}
		public void Reverse (int a, int b)
		{
			if ((a < 0) || (b < 0)) {
				throw new ArgumentOutOfRangeException ((a < 0) ? "index" : "count", "ArgumentOutOfRange_NeedNonNegNum");
			}
			if ((this._size - a) < b) {
				throw new ArgumentException ("Argument_InvalidOffLen");
			}
			Array.Reverse (this._items, a, b);
			this._version++;
		}
		public void Sort ()
		{
			this.Sort (0, this.Count, null);
		}
		public void Sort (IComparer<T> a)
		{
			this.Sort (0, this.Count, a);
		}
		public void Sort (Comparison<T> a)
		{
			if (a == null) {
				throw new ArgumentNullException ("match");
			}
			if (this._size > 0) {
				IComparer<T> b = new FunctorComparer<T> (a);
				Array.Sort<T> (this._items, 0, this._size, b);
			}
		}
		public void Sort (int a, int b, IComparer<T> c)
		{
			if ((a < 0) || (b < 0)) {
				throw new ArgumentOutOfRangeException ((a < 0) ? "index" : "count", "ArgumentOutOfRange_NeedNonNegNum");
			}
			if ((this._size - a) < b) {
				throw new ArgumentException ("Argument_InvalidOffLen");
			}
			Array.Sort<T> (this._items, a, b, c);
			this._version++;
		}
		IEnumerator<T> IEnumerable<T>.GetEnumerator ()
		{
			return new Enumerator<T> ((GDAList<T>)this);
		}
		void ICollection.CopyTo (Array a, int b)
		{
			if ((a != null) && (a.Rank != 1)) {
				throw new ArgumentException ("Arg_RankMultiDimNotSupported");
			}
			try {
				Array.Copy (this._items, 0, a, b, this._size);
			}
			catch (ArrayTypeMismatchException) {
				throw new ArgumentException ("Argument_InvalidArrayType");
			}
			catch (InvalidCastException) {
				throw new ArgumentException ("Argument_InvalidArrayType");
			}
		}
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return new Enumerator<T> ((GDAList<T>)this);
		}
		int IList.Add (object a)
		{
			GDAList<T>.VerifyValueType (a);
			this.Add ((T)a);
			return (this.Count - 1);
		}
		bool IList.Contains (object a)
		{
			if (GDAList<T>.IsCompatibleObject (a)) {
				return this.Contains ((T)a);
			}
			return false;
		}
		int IList.IndexOf (object a)
		{
			if (GDAList<T>.IsCompatibleObject (a)) {
				return this.IndexOf ((T)a);
			}
			return -1;
		}
		void IList.Insert (int a, object b)
		{
			GDAList<T>.VerifyValueType (b);
			this.Insert (a, (T)b);
		}
		void IList.Remove (object a)
		{
			if (GDAList<T>.IsCompatibleObject (a)) {
				this.Remove ((T)a);
			}
		}
		public T[] ToArray ()
		{
			T[] a = new T[this._size];
			Array.Copy (this._items, 0, a, 0, this._size);
			return a;
		}
		public void TrimExcess ()
		{
			int a = (int)(this._items.Length * 0.9);
			if (this._size < a) {
				this.Capacity = this._size;
			}
		}
		public bool TrueForAll (Predicate<T> a)
		{
			if (a == null) {
				throw new ArgumentNullException ("match");
			}
			for (int b = 0; b < this._size; b++) {
				if (!a (this._items [b])) {
					return false;
				}
			}
			return true;
		}
		private static void VerifyValueType (object a)
		{
			if (!GDAList<T>.IsCompatibleObject (a)) {
				throw new ArgumentException ();
			}
		}
		public int Capacity {
			get {
				return this._items.Length;
			}
			set {
				if (value != this._items.Length) {
					if (value < this._size) {
						throw new ArgumentOutOfRangeException ("value", "ArgumentOutOfRange_SmallCapacity");
					}
					if (value > 0) {
						T[] a = new T[value];
						if (this._size > 0) {
							Array.Copy (this._items, 0, a, 0, this._size);
						}
						this._items = a;
					}
					else {
						this._items = GDAList<T>._emptyArray;
					}
				}
			}
		}
		public int Count {
			get {
				return GetCount ();
			}
		}
		internal protected virtual int GetCount ()
		{
			return this._size;
		}
		public T this [int a] {
			get {
				return GetItem (a);
			}
			set {
				SetItem (a, value);
			}
		}
		internal protected virtual T GetItem (int a)
		{
			if (a >= this._size) {
				throw new ArgumentOutOfRangeException ();
			}
			return this._items [a];
		}
		internal protected virtual void SetItem (int a, T b)
		{
			if (a >= this._size) {
				throw new ArgumentOutOfRangeException ();
			}
			OnSet (a, this._items [a], b);
			this._items [a] = b;
			this._version++;
			OnSetComplete (a, b);
		}
		bool ICollection<T>.IsReadOnly {
			get {
				return false;
			}
		}
		bool ICollection.IsSynchronized {
			get {
				return false;
			}
		}
		object ICollection.SyncRoot {
			get {
				if (this._syncRoot == null) {
					System.Threading.Interlocked.CompareExchange (ref this._syncRoot, new object (), null);
				}
				return this._syncRoot;
			}
		}
		bool IList.IsFixedSize {
			get {
				return false;
			}
		}
		bool IList.IsReadOnly {
			get {
				return false;
			}
		}
		object IList.this [int index] {
			get {
				return this [index];
			}
			set {
				GDAList<T>.VerifyValueType (value);
				this [index] = (T)value;
			}
		}
		public virtual void OnClear ()
		{
		}
		public virtual void OnClearComplete ()
		{
		}
		public virtual void OnInsert (int a, T b)
		{
		}
		public virtual void OnInsertComplete (int a, T b)
		{
		}
		public virtual void OnRemove (int a, T b)
		{
		}
		public virtual void OnRemoveComplete (int a, T b)
		{
		}
		public virtual void OnRemoveAll ()
		{
		}
		public virtual void OnSet (int a, T b, T c)
		{
		}
		public virtual void OnSetComplete (int a, T b)
		{
		}
		public struct Enumerator<T> : IEnumerator<T>, IDisposable, IEnumerator
		{
			private GDAList<T> list;
			private int index;
			private int version;
			private T current;
			internal Enumerator (GDAList<T> a)
			{
				this.list = a;
				this.index = 0;
				this.version = a._version;
				this.current = default(T);
			}
			public void Dispose ()
			{
			}
			public bool MoveNext ()
			{
				if (this.version != this.list._version) {
					throw new InvalidOperationException ("InvalidOperation_EnumFailedVersion");
				}
				if (this.index < this.list._size) {
					this.current = this.list._items [this.index];
					this.index++;
					return true;
				}
				this.index = this.list._size + 1;
				this.current = default(T);
				return false;
			}
			public T Current {
				get {
					return this.current;
				}
			}
			object IEnumerator.Current {
				get {
					if ((this.index == 0) || (this.index == (this.list._size + 1))) {
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
				this.current = default(T);
			}
		}
		public static implicit operator List<T> (GDAList<T> a) {
			return new List<T> (a);
		}
	}
}
