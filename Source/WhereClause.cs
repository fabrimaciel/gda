using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
namespace GDA.Sql
{
	public abstract class WhereClause<T> where T : WhereClause<T>
	{
		private ConditionalContainer _container;
		public WhereClause (ConditionalContainer a)
		{
			if (a == null)
				throw new ArgumentNullException ("container");
			_container = a;
		}
		public WhereClause<T> And (Conditional a)
		{
			_container.And (a);
			return this;
		}
		public WhereClause<T> Or (Conditional a)
		{
			_container.Or (a);
			return this;
		}
		public WhereClause<T> And (string a)
		{
			_container.And (a);
			return this;
		}
		public WhereClause<T> Or (string a)
		{
			_container.Or (a);
			return this;
		}
		public virtual WhereClause<T> Start (Conditional a)
		{
			_container.Start (a);
			return this;
		}
		public virtual WhereClause<T> Start (string a)
		{
			_container.Start (a);
			return this;
		}
		public override string ToString ()
		{
			return _container.ToString ();
		}
		public WhereClause<T> Add (string a, DbType b, object c)
		{
			_container.Add (new GDAParameter (a, c) {
				DbType = b
			});
			return this;
		}
		public WhereClause<T> Add (string a, object b)
		{
			_container.Add (new GDAParameter (a, b));
			return this;
		}
		public WhereClause<T> Add (DbType a, int b, object c)
		{
			_container.Add (new GDAParameter () {
				DbType = a,
				Size = b,
				Value = c
			});
			return this;
		}
		public WhereClause<T> Add (string a, DbType b, int c, object d)
		{
			_container.Add (new GDAParameter () {
				ParameterName = a,
				DbType = b,
				Size = c,
				Value = d
			});
			return this;
		}
		public WhereClause<T> Add (GDAParameter a)
		{
			_container.Add (a);
			return this;
		}
	}
}
