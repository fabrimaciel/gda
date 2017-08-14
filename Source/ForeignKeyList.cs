using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
namespace GDA.Analysis
{
	public class ForeignKeyList : IList<ForeignKeyMap>, IEnumerable<ForeignKeyMap>
	{
		private List<ForeignKeyMap> _foreignKeys;
		private HybridDictionary _constraints;
		public ForeignKeyMap this [int a] {
			get {
				return _foreignKeys [a];
			}
			set {
			}
		}
		public ForeignKeyMap this [string a] {
			get {
				if (_constraints == null) {
					_constraints = new HybridDictionary (_foreignKeys.Count, true);
					foreach (ForeignKeyMap fk in _foreignKeys) {
						_constraints.Add (fk.ConstraintName, fk);
					}
				}
				return _constraints [a] as ForeignKeyMap;
			}
		}
		internal ForeignKeyList () : base ()
		{
			_foreignKeys = new List<ForeignKeyMap> ();
		}
		public IEnumerable<ForeignKeyMap> FindForeignKeyTable (string a)
		{
			if (a == null)
				throw new NullReferenceException ("name");
			foreach (ForeignKeyMap fkm in _foreignKeys)
				if (fkm.ForeignKeyTable == a)
					yield return fkm;
		}
		public IEnumerable<ForeignKeyMap> FindPrimaryKeyTable (string a)
		{
			if (a == null)
				throw new NullReferenceException ("name");
			foreach (ForeignKeyMap fkm in _foreignKeys)
				if (fkm.PrimaryKeyColumn == a)
					yield return fkm;
		}
		public bool IsReadOnly {
			get {
				return true;
			}
		}
		public void RemoveAt (int a)
		{
			ForeignKeyMap b = _foreignKeys [a];
			Remove (b);
			if (_constraints != null)
				_constraints.Remove (b.ConstraintName);
		}
		public void Insert (int a, ForeignKeyMap b)
		{
			_foreignKeys.Insert (a, b);
			_constraints = null;
		}
		public bool Remove (ForeignKeyMap a)
		{
			bool b = _foreignKeys.Remove (a);
			if (_constraints != null)
				_constraints.Remove (a.ConstraintName);
			return b;
		}
		public bool Contains (ForeignKeyMap a)
		{
			return _foreignKeys.Contains (a);
		}
		public void Clear ()
		{
			_foreignKeys.Clear ();
			_constraints = null;
		}
		public int IndexOf (ForeignKeyMap a)
		{
			return _foreignKeys.IndexOf (a);
		}
		public void Add (ForeignKeyMap a)
		{
			Insert (_foreignKeys.Count, a);
		}
		public bool IsFixedSize {
			get {
				return false;
			}
		}
		public bool IsSynchronized {
			get {
				return false;
			}
		}
		public int Count {
			get {
				return _foreignKeys.Count;
			}
		}
		public void CopyTo (ForeignKeyMap[] a, int b)
		{
			_foreignKeys.CopyTo (a, b);
		}
		public object SyncRoot {
			get {
				return _foreignKeys;
			}
		}
		public IEnumerator GetEnumerator ()
		{
			return _foreignKeys.GetEnumerator ();
		}
		IEnumerator<ForeignKeyMap> IEnumerable<ForeignKeyMap>.GetEnumerator ()
		{
			return _foreignKeys.GetEnumerator ();
		}
	}
}
