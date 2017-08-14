using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
namespace GDA.Sql
{
	public class BindingResultList<Model> : ResultList<Model>, IBindingList where Model : new()
	{
		public BindingResultList (int a) : base (a)
		{
		}
		public BindingResultList (Query a, int b) : base (a, b)
		{
		}
		public BindingResultList (Query a, GDASession b, int c) : base (a, b, c)
		{
		}
		private PropertyDescriptor _sortProperty;
		private ListSortDirection _direction;
		public void AddIndex (PropertyDescriptor a)
		{
			throw new NotSupportedException ();
		}
		public object AddNew ()
		{
			throw new NotSupportedException ();
		}
		public bool AllowEdit {
			get {
				return false;
			}
		}
		public bool AllowNew {
			get {
				return false;
			}
		}
		public bool AllowRemove {
			get {
				return false;
			}
		}
		public void ApplySort (PropertyDescriptor a, ListSortDirection b)
		{
			if (_queryInstance == null)
				throw new GDAException ("ResultList not support for Query {0}.", _myQuery.GetType ().FullName);
			_sortProperty = a;
			_direction = b;
			_queryInstance.SetOrder (a.Name + " " + (b == ListSortDirection.Ascending ? "ASC" : "DESC"));
			Refresh ();
		}
		public void RemoveSort ()
		{
			if (_queryInstance == null)
				throw new GDAException ("ResultList not support for Query {0}.", _myQuery.GetType ().FullName);
			_sortProperty = null;
			_queryInstance.SetOrder (null);
			Refresh ();
		}
		public int Find (PropertyDescriptor a, object b)
		{
			throw new NotSupportedException ();
		}
		public bool IsSorted {
			get {
				return (_sortProperty != null);
			}
		}
		public event ListChangedEventHandler ListChanged;
		public void RemoveIndex (PropertyDescriptor a)
		{
			throw new NotImplementedException ();
		}
		public ListSortDirection SortDirection {
			get {
				return _direction;
			}
		}
		public PropertyDescriptor SortProperty {
			get {
				return _sortProperty;
			}
		}
		public bool SupportsChangeNotification {
			get {
				return true;
			}
		}
		public bool SupportsSearching {
			get {
				return false;
			}
		}
		public bool SupportsSorting {
			get {
				return true;
			}
		}
		public int Add (object a)
		{
			throw new NotImplementedException ();
		}
		public void Clear ()
		{
			throw new NotImplementedException ();
		}
		public bool Contains (object a)
		{
			throw new NotImplementedException ();
		}
		public int IndexOf (object a)
		{
			throw new NotImplementedException ();
		}
		public void Insert (int a, object b)
		{
			throw new NotImplementedException ();
		}
		public bool IsFixedSize {
			get {
				throw new NotImplementedException ();
			}
		}
		public void Remove (object a)
		{
			throw new NotImplementedException ();
		}
		public void RemoveAt (int a)
		{
			throw new NotImplementedException ();
		}
		public new object this [int a] {
			get {
				return base [a];
			}
			set {
				base [a] = (Model)value;
			}
		}
	}
}
