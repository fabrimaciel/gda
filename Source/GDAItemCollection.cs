using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Collections;
using System.Globalization;
namespace GDA.Collections
{
	public class GDAItemCollection<T> : GDAList<T>, IList<T>, IBindingList, IBindingListView
	{
		public GDAItemCollection ()
		{
		}
		public GDAItemCollection (IEnumerable<T> a) : base (a)
		{
		}
		public event ListChangedEventHandler ListChanged;
		private ListChangedEventArgs resetEvent = new ListChangedEventArgs (ListChangedType.Reset, -1);
		protected virtual void OnListChanged (ListChangedEventArgs a)
		{
			if (ListChanged != null) {
				ListChanged (this, a);
			}
		}
		public bool AllowEdit {
			get {
				return true;
			}
		}
		public bool AllowNew {
			get {
				return true;
			}
		}
		public bool AllowRemove {
			get {
				return true;
			}
		}
		public bool SupportsChangeNotification {
			get {
				return false;
			}
		}
		public bool SupportsSearching {
			get {
				return true;
			}
		}
		public bool SupportsSorting {
			get {
				return true;
			}
		}
		public object AddNew ()
		{
			T a = (T)Activator.CreateInstance (typeof(T));
			this.Add (a);
			return a;
		}
		private bool isSorted = false;
		public bool IsSorted {
			get {
				return isSorted;
			}
		}
		private ListSortDirection listSortDirection = ListSortDirection.Ascending;
		public ListSortDirection SortDirection {
			get {
				return listSortDirection;
			}
		}
		PropertyDescriptor sortProperty = null;
		string sortPropertyName = null;
		public PropertyDescriptor SortProperty {
			get {
				return sortProperty;
			}
		}
		public void AddIndex (PropertyDescriptor a)
		{
			isSorted = true;
			sortProperty = a;
			sortPropertyName = a.Name;
		}
		public void ApplySort (PropertyDescriptor a, ListSortDirection b)
		{
			sortProperty = a;
			ApplySort (a.Name, b);
		}
		public void ApplySort (string a, ListSortDirection b)
		{
			isSorted = true;
			sortPropertyName = a;
			listSortDirection = b;
			ArrayList c = new ArrayList ();
			this.Sort (new ObjectPropertyComparer<T> (sortPropertyName));
			if (b == ListSortDirection.Descending)
				this.Reverse ();
			OnListChanged (resetEvent);
		}
		private int[] Find (string a, object b, bool c)
		{
			GDAList<int> d = new GDAList<int> ();
			Type e = typeof(T);
			int f = 0;
			foreach (T o in this) {
				if (Match (e.GetProperty (a).GetValue (o, null), b)) {
					if (c)
						return new int[] {
							f
						};
					else
						d.Add (f);
				}
				f++;
			}
			if (c)
				return null;
			else
				return d.ToArray ();
		}
		public int Find (string a, object b)
		{
			int[] c = Find (a, b, true);
			if (c == null) {
				return -1;
			}
			else {
				OnListChanged (new ListChangedEventArgs (ListChangedType.ItemMoved, c [0], 0));
				return c [0];
			}
		}
		public int Find (PropertyDescriptor a, object b)
		{
			return Find (a.Name, b);
		}
		public void RemoveIndex (PropertyDescriptor a)
		{
			sortProperty = null;
			sortPropertyName = null;
		}
		public void RemoveSort ()
		{
			isSorted = false;
			sortProperty = null;
			sortPropertyName = null;
			OnListChanged (resetEvent);
		}
		private bool applyFilter = false;
		private int[] itensFilter;
		public void ApplyFilter (string a, object b)
		{
			itensFilter = Find (a, b, false);
			applyFilter = true;
			OnListChanged (resetEvent);
		}
		public void RemoveFilter ()
		{
			applyFilter = false;
			OnListChanged (resetEvent);
		}
		protected internal override T GetItem (int a)
		{
			if (applyFilter)
				return base.GetItem (itensFilter [a]);
			else
				return base.GetItem (a);
		}
		protected internal override void SetItem (int a, T b)
		{
			if (applyFilter)
				base.SetItem (itensFilter [a], b);
			else
				base.SetItem (a, b);
		}
		protected internal override int GetCount ()
		{
			if (applyFilter)
				return itensFilter.Length;
			else
				return base.GetCount ();
		}
		public override void OnInsertComplete (int a, T b)
		{
			base.OnInsert (a, b);
			OnListChanged (new ListChangedEventArgs (ListChangedType.ItemAdded, a));
		}
		public override void OnRemoveComplete (int a, T b)
		{
			base.OnRemove (a, b);
			OnListChanged (new ListChangedEventArgs (ListChangedType.ItemDeleted, a));
		}
		public override void OnRemoveAll ()
		{
			base.OnRemoveAll ();
			OnListChanged (resetEvent);
		}
		public override void OnClearComplete ()
		{
			base.OnClear ();
			OnListChanged (resetEvent);
		}
		public override void OnSetComplete (int a, T b)
		{
			base.OnSetComplete (a, b);
			OnListChanged (new ListChangedEventArgs (ListChangedType.ItemChanged, a));
		}
		protected bool Match (object a, object b)
		{
			if (a == null || b == null) {
				return (bool)(a == b);
			}
			bool c = (bool)(a is string);
			if (a.GetType () != b.GetType ())
				throw new ArgumentException ("Objects must be of the same type");
			if (!(a.GetType ().IsValueType || a is string))
				throw new ArgumentException ("Objects must be a value type");
			if (c) {
				string d = ((string)a).ToLower (CultureInfo.CurrentCulture);
				string e = ((string)b).ToLower (CultureInfo.CurrentCulture);
				return (bool)(d.IndexOf (e) >= 0);
			}
			else {
				return (bool)(Comparer.Default.Compare (a, b) == 0);
			}
		}
		private string m_FilterString = null;
		void IBindingListView.ApplySort (ListSortDescriptionCollection a)
		{
			throw new NotSupportedException ();
		}
		string IBindingListView.Filter {
			get {
				return m_FilterString;
			}
			set {
				m_FilterString = value;
				UpdateFilter ();
			}
		}
		void IBindingListView.RemoveFilter ()
		{
			RemoveFilter ();
		}
		ListSortDescriptionCollection IBindingListView.SortDescriptions {
			get {
				return null;
			}
		}
		bool IBindingListView.SupportsAdvancedSorting {
			get {
				return false;
			}
		}
		bool IBindingListView.SupportsFiltering {
			get {
				return true;
			}
		}
		protected virtual void UpdateFilter ()
		{
			int a = m_FilterString.IndexOf ('=');
			string b = m_FilterString.Substring (0, a).Trim ();
			string c = m_FilterString.Substring (a + 1, m_FilterString.Length - a - 1).Trim ();
			c = c.Substring (1, c.Length - 2);
			ApplyFilter (b, c);
		}
		public static implicit operator List<T> (GDAItemCollection<T> a) {
			return new List<T> (a);
		}
	}
}
