using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
namespace GDA.Analysis
{
	public sealed class FieldList : IList, IEnumerable
	{
		private ArrayList fields;
		private HybridDictionary columns;
		internal FieldList () : base ()
		{
			fields = new ArrayList ();
		}
		public FieldMap FindColumn (string a)
		{
			if (a == null)
				throw new NullReferenceException ("name");
			if (columns == null) {
				columns = new HybridDictionary (fields.Count, true);
				foreach (FieldMap fm in fields) {
					columns.Add (fm.ColumnName, fm);
				}
			}
			return columns [a] as FieldMap;
		}
		public FieldMap FindColumnById (int a)
		{
			foreach (FieldMap fm in this) {
				if (a != -1 && a == fm.ColumnId)
					return fm;
			}
			return null;
		}
		public int PrimaryKeyCount {
			get {
				int a = 0;
				foreach (FieldMap fm in this) {
					if (fm.IsPrimaryKey)
						a++;
				}
				return a;
			}
		}
		public FieldMap this [int a] {
			get {
				return fields [a] as FieldMap;
			}
		}
		public bool IsReadOnly {
			get {
				return false;
			}
		}
		object IList.this [int index] {
			get {
				return fields [index];
			}
			set {
				fields [index] = value;
			}
		}
		public void RemoveAt (int a)
		{
			FieldMap b = fields [a] as FieldMap;
			Remove (b);
			if (columns != null)
				columns.Remove (b.ColumnName);
		}
		public void Insert (int a, object b)
		{
			FieldMap c = b as FieldMap;
			fields.Insert (a, c);
			columns = null;
		}
		public void Remove (object a)
		{
			FieldMap b = a as FieldMap;
			fields.Remove (b);
			if (columns != null)
				columns.Remove (b.ColumnName);
		}
		public bool Contains (object a)
		{
			return fields.Contains (a);
		}
		public void Clear ()
		{
			fields.Clear ();
			columns = null;
		}
		public int IndexOf (object a)
		{
			return fields.IndexOf (a);
		}
		public int Add (object a)
		{
			Insert (fields.Count, a);
			return fields.Count;
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
				return fields.Count;
			}
		}
		public void CopyTo (Array a, int b)
		{
			fields.CopyTo (a, b);
		}
		public object SyncRoot {
			get {
				return fields.SyncRoot;
			}
		}
		public IEnumerator GetEnumerator ()
		{
			return fields.GetEnumerator ();
		}
	}
}
