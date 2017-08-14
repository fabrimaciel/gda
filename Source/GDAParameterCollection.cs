using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
namespace GDA.Collections
{
	public class GDAParameterCollection : List<GDAParameter>, IGDAParameterContainer
	{
		public GDAParameterCollection () : base ()
		{
		}
		public GDAParameterCollection (IEnumerable<GDAParameter> a) : base (a)
		{
		}
		public GDAParameterCollection (int a) : base (a)
		{
		}
		public GDAParameterCollection Add (DbType a, object b)
		{
			return Add ("", a, b);
		}
		public GDAParameterCollection Add (string a, DbType b, object c)
		{
			return Add (a, b, 0, c);
		}
		public GDAParameterCollection Add (string a, object b)
		{
			Add (new GDAParameter (a, b));
			return this;
		}
		public GDAParameterCollection Add (DbType a, int b, object c)
		{
			return Add ("", a, b, c);
		}
		public GDAParameterCollection Add (string a, DbType b, int c, object d)
		{
			GDAParameter e = new GDAParameter ();
			e.ParameterName = a;
			e.DbType = b;
			e.Size = c;
			e.Value = d;
			this.Add (e);
			return this;
		}
		public bool TryGet (string a, out GDAParameter b)
		{
			foreach (var i in this)
				if (i.ParameterName == a) {
					b = i;
					return true;
				}
			b = null;
			return false;
		}
		public bool ContainsKey (string a)
		{
			foreach (var i in this)
				if (i.ParameterName == a)
					return true;
			return false;
		}
		public bool Remove (string a)
		{
			var b = this.FindIndex (c => c.ParameterName == a);
			if (b >= 0) {
				this.RemoveAt (b);
				return true;
			}
			return false;
		}
	}
}
