using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GDA.Collections;
namespace GDA.Sql
{
	public class QueryPropertySelector<T> : IEnumerable<System.Reflection.PropertyInfo> where T : new()
	{
		private Query _query;
		private List<System.Reflection.PropertyInfo> _properties = new List<System.Reflection.PropertyInfo> ();
		public Query Query {
			get {
				_query.Select (this.ToString ());
				return _query;
			}
		}
		internal QueryPropertySelector (Query a)
		{
			if (a == null)
				throw new ArgumentNullException ("query");
			_query = a;
		}
		public QueryPropertySelector<T> Add (params System.Linq.Expressions.Expression<Func<T, object>>[] a)
		{
			if (a != null) {
				foreach (var i in a) {
					if (i == null)
						continue;
					var b = i.GetMember () as System.Reflection.PropertyInfo;
					if (b != null)
						_properties.Add (b);
				}
			}
			return this;
		}
		public override string ToString ()
		{
			return string.Join (",", _properties.Select (a => a.Name).Distinct ().ToArray ());
		}
		public GDACursor<T> ToCursor ()
		{
			return Query.ToCursor<T> ();
		}
		public GDACursor<T> ToCursor (GDASession a)
		{
			return Query.ToCursor<T> (a);
		}
		public virtual IEnumerable<Result> ToCursor<Result> (GDASession session) where Result : new()
		{
			return Query.ToCursor<T, Result> (session);
		}
		public virtual IEnumerable<Result> ToCursor<Result> () where Result : new()
		{
			return Query.ToCursor<T, Result> (null);
		}
		public GDADataRecordCursor<T> ToDataRecords ()
		{
			return Query.ToDataRecords<T> ();
		}
		public GDADataRecordCursor<T> ToDataRecords (GDASession a)
		{
			return Query.ToDataRecords<T> (a);
		}
		public GDAList<T> ToList ()
		{
			return Query.ToList<T> ();
		}
		public GDAList<T> ToList (GDASession a)
		{
			return Query.ToList<T> (a);
		}
		public ResultList<T> ToResultList (int a)
		{
			return Query.ToResultList<T> (a);
		}
		public ResultList<T> ToResultList (GDASession a, int b)
		{
			return Query.ToResultList<T> (a, b);
		}
		public static implicit operator string (QueryPropertySelector<T> a) {
			if (a != null)
				return a.ToString ();
			else
				return null;
		}
		public IEnumerator<System.Reflection.PropertyInfo> GetEnumerator ()
		{
			return _properties.GetEnumerator ();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return _properties.GetEnumerator ();
		}
	}
}
