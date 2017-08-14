using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
namespace GDA.Diagnostics
{
	public class GDATraceListenerCollection : IList, ICollection, IEnumerable
	{
		private ArrayList _list = new ArrayList (1);
		public int Count {
			get {
				return this._list.Count;
			}
		}
		public GDATraceListener this [int a] {
			get {
				return (GDATraceListener)this._list [a];
			}
			set {
				this.InitializeListener (value);
				this._list [a] = value;
			}
		}
		public GDATraceListener this [string a] {
			get {
				foreach (GDATraceListener listener in this) {
					if (listener.Name == a) {
						return listener;
					}
				}
				return null;
			}
		}
		bool ICollection.IsSynchronized {
			get {
				return true;
			}
		}
		object ICollection.SyncRoot {
			get {
				return this;
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
				return this._list [index];
			}
			set {
				GDATraceListener listener = value as GDATraceListener;
				if (listener == null) {
					throw new ArgumentException ("MustAddListener", "value");
				}
				this.InitializeListener (listener);
				this._list [index] = listener;
			}
		}
		internal GDATraceListenerCollection ()
		{
		}
		public int Add (GDATraceListener a)
		{
			this.InitializeListener (a);
			lock (GDATraceInternal._critSec)
				return this._list.Add (a);
		}
		public void AddRange (GDATraceListener[] a)
		{
			if (a == null)
				throw new ArgumentNullException ("value");
			for (int b = 0; b < a.Length; b++)
				this.Add (a [b]);
		}
		public void AddRange (GDATraceListenerCollection a)
		{
			if (a == null)
				throw new ArgumentNullException ("value");
			int b = a.Count;
			for (int c = 0; c < b; c++)
				this.Add (a [c]);
		}
		public void Clear ()
		{
			this._list = new ArrayList ();
		}
		public bool Contains (GDATraceListener a)
		{
			return ((IList)this).Contains (a);
		}
		public void CopyTo (GDATraceListener[] a, int b)
		{
			((ICollection)this).CopyTo (a, b);
		}
		public IEnumerator GetEnumerator ()
		{
			return this._list.GetEnumerator ();
		}
		public int IndexOf (GDATraceListener a)
		{
			return ((IList)this).IndexOf (a);
		}
		internal void InitializeListener (GDATraceListener a)
		{
			if (a == null)
				throw new ArgumentNullException ("listener");
		}
		public void Insert (int a, GDATraceListener b)
		{
			this.InitializeListener (b);
			lock (GDATraceInternal._critSec) {
				this._list.Insert (a, b);
			}
		}
		public void Remove (GDATraceListener a)
		{
			((IList)this).Remove (a);
		}
		public void Remove (string a)
		{
			GDATraceListener b = this [a];
			if (b != null) {
				((IList)this).Remove (b);
			}
		}
		public void RemoveAt (int a)
		{
			lock (GDATraceInternal._critSec) {
				this._list.RemoveAt (a);
			}
		}
		void ICollection.CopyTo (Array a, int b)
		{
			lock (GDATraceInternal._critSec) {
				this._list.CopyTo (a, b);
			}
		}
		int IList.Add (object a)
		{
			GDATraceListener b = a as GDATraceListener;
			if (b == null)
				throw new ArgumentException ("MustAddListener", "value");
			this.InitializeListener (b);
			lock (GDATraceInternal._critSec) {
				return this._list.Add (a);
			}
		}
		bool IList.Contains (object a)
		{
			return this._list.Contains (a);
		}
		int IList.IndexOf (object a)
		{
			return this._list.IndexOf (a);
		}
		void IList.Insert (int a, object b)
		{
			GDATraceListener c = b as GDATraceListener;
			if (c == null) {
				throw new ArgumentException ("MustAddListener", "value");
			}
			this.InitializeListener (c);
			lock (GDATraceInternal._critSec) {
				this._list.Insert (a, b);
			}
		}
		void IList.Remove (object a)
		{
			lock (GDATraceInternal._critSec) {
				this._list.Remove (a);
			}
		}
	}
}
