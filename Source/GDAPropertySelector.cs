using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace GDA
{
	public class GDAPropertySelector<T> : IEnumerable<System.Reflection.PropertyInfo>
	{
		private T _instance;
		private List<System.Reflection.PropertyInfo> _properties = new List<System.Reflection.PropertyInfo> ();
		internal GDAPropertySelector (T a)
		{
			if (a == null)
				throw new ArgumentNullException ("instance");
			_instance = a;
		}
		public GDAPropertySelector<T> Add (params System.Linq.Expressions.Expression<Func<T, object>>[] a)
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
		public uint Insert ()
		{
			return GDAOperations.Insert (_instance, this.ToString ());
		}
		public uint Insert (GDASession a)
		{
			return GDAOperations.Insert (a, _instance, this.ToString ());
		}
		public uint Insert (GDASession a, DirectionPropertiesName b)
		{
			return GDAOperations.Insert (a, _instance, this.ToString (), b);
		}
		public int Update ()
		{
			return GDAOperations.Update (_instance, this.ToString ());
		}
		public int Update (GDASession a)
		{
			return GDAOperations.Update (a, _instance, this.ToString ());
		}
		public int Update (GDASession a, DirectionPropertiesName b)
		{
			return GDAOperations.Update (a, _instance, this.ToString (), b);
		}
		public override string ToString ()
		{
			return string.Join (",", _properties.Select (a => a.Name).Distinct ().ToArray ());
		}
		public static implicit operator string (GDAPropertySelector<T> a) {
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
