using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
namespace GDA
{
	public abstract class GDAConnectionListener
	{
		public string Name {
			get;
			set;
		}
		protected GDAConnectionListener ()
		{
		}
		public abstract void NotifyConnectionCreated (IDbConnection a);
		public abstract void NotifyConnectionOpened (IDbConnection a);
	}
	public class GDAConnectionListenerCollection : IList, ICollection, IEnumerable
	{
		internal static readonly object _critSec = new object ();
		private ArrayList _list = new ArrayList (1);
		public int Count {
			get {
				return this._list.Count;
			}
		}
		public GDAConnectionListener this [int a] {
			get {
				return (GDAConnectionListener)_list [a];
			}
			set {
				this.InitializeListener (value);
				_list [a] = value;
			}
		}
		public GDAConnectionListener this [string a] {
			get {
				foreach (GDAConnectionListener listener in this) {
					if (listener.Name == a)
						return listener;
				}
				return null;
			}
		}
		internal GDAConnectionListenerCollection ()
		{
		}
		public int Add (GDAConnectionListener a)
		{
			this.InitializeListener (a);
			lock (_critSec)
				return _list.Add (a);
		}
		public void AddRange (GDAConnectionListener[] a)
		{
			if (a == null)
				throw new ArgumentNullException ("value");
			for (int b = 0; b < a.Length; b++)
				this.Add (a [b]);
		}
		public void AddRange (GDAConnectionListenerCollection a)
		{
			if (a == null)
				throw new ArgumentNullException ("value");
			int b = a.Count;
			for (int c = 0; c < b; c++)
				this.Add (a [c]);
		}
		public void Clear ()
		{
			_list = new ArrayList ();
		}
		public bool Contains (GDAConnectionListener a)
		{
			return ((IList)this).Contains (a);
		}
		public void CopyTo (GDAConnectionListener[] a, int b)
		{
			((ICollection)this).CopyTo (a, b);
		}
		public IEnumerator GetEnumerator ()
		{
			return _list.GetEnumerator ();
		}
		public int IndexOf (GDAConnectionListener a)
		{
			return ((IList)this).IndexOf (a);
		}
		internal void InitializeListener (GDAConnectionListener a)
		{
			if (a == null)
				throw new ArgumentNullException ("listener");
		}
		public void Insert (int a, GDAConnectionListener b)
		{
			this.InitializeListener (b);
			lock (_critSec)
				_list.Insert (a, b);
		}
		public void Remove (GDAConnectionListener a)
		{
			((IList)this).Remove (a);
		}
		public void Remove (string a)
		{
			GDAConnectionListener b = this [a];
			if (b != null)
				((IList)this).Remove (b);
		}
		public void RemoveAt (int a)
		{
			lock (_critSec)
				_list.RemoveAt (a);
		}
		void ICollection.CopyTo (Array a, int b)
		{
			lock (_critSec)
				_list.CopyTo (a, b);
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
		int IList.Add (object a)
		{
			GDAConnectionListener b = a as GDAConnectionListener;
			if (b == null)
				throw new ArgumentException ("MustAddListener", "value");
			this.InitializeListener (b);
			lock (_critSec)
				return _list.Add (a);
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
			GDAConnectionListener c = b as GDAConnectionListener;
			if (c == null)
				throw new ArgumentException ("MustAddListener", "value");
			this.InitializeListener (c);
			lock (_critSec) {
				_list.Insert (a, b);
			}
		}
		void IList.Remove (object a)
		{
			lock (_critSec) {
				_list.Remove (a);
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
				return _list [index];
			}
			set {
				var listener = value as GDAConnectionListener;
				if (listener == null)
					throw new ArgumentException ("MustAddListener", "value");
				this.InitializeListener (listener);
				_list [index] = listener;
			}
		}
	}
	public static class GDAConnectionManager
	{
		private static GDAConnectionListenerCollection _listeners;
		internal static void NotifyConnectionCreated (IDbConnection a)
		{
			foreach (GDAConnectionListener listener in Listeners) {
				listener.NotifyConnectionCreated (a);
			}
		}
		internal static void NotifyConnectionOpened (IDbConnection a)
		{
			foreach (GDAConnectionListener listener in Listeners) {
				listener.NotifyConnectionOpened (a);
			}
		}
		public static GDAConnectionListenerCollection Listeners {
			get {
				if (_listeners == null) {
					lock (GDAConnectionListenerCollection._critSec) {
						if (_listeners == null)
							_listeners = new GDAConnectionListenerCollection ();
					}
				}
				return _listeners;
			}
		}
	}
}
