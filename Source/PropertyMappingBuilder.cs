using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
namespace GDA.Mapping
{
	public class PropertyMappingBuilder<T> : IEnumerable<PropertyMapping>
	{
		private List<PropertyMapping> _properties = new List<PropertyMapping> ();
		public static PropertyMappingBuilder<T> Create (params Expression<Func<T, object>>[] a)
		{
			return new PropertyMappingBuilder<T> ().Add (a);
		}
		public PropertyMappingBuilder<T> Add (params Expression<Func<T, object>>[] a)
		{
			if (a == null)
				throw new ArgumentNullException ("propertiesSelector");
			foreach (var i in a.Where (b => b != null))
				_properties.Add (new PropertyMapping (GDA.Extensions.GetMember (i).Name, null, PersistenceParameterType.Field, 0, false, false, DirectionParameter.InputOutput, null, null, null, null));
			return this;
		}
		public PropertyMappingBuilder<T> Add (Expression<Func<T, object>> a, string b)
		{
			if (a == null)
				throw new ArgumentNullException ("propertySelector");
			_properties.Add (new PropertyMapping (GDA.Extensions.GetMember (a).Name, b, PersistenceParameterType.Field, 0, false, false, DirectionParameter.InputOutput, null, null, null, null));
			return this;
		}
		public PropertyMappingBuilder<T> Add (Expression<Func<T, object>> a, string b, DirectionParameter c)
		{
			if (a == null)
				throw new ArgumentNullException ("propertySelector");
			_properties.Add (new PropertyMapping (GDA.Extensions.GetMember (a).Name, b, PersistenceParameterType.Field, 0, false, false, c, null, null, null, null));
			return this;
		}
		public PropertyMappingBuilder<T> Add (Expression<Func<T, object>> a, string b, PersistenceParameterType c)
		{
			if (a == null)
				throw new ArgumentNullException ("propertySelector");
			_properties.Add (new PropertyMapping (GDA.Extensions.GetMember (a).Name, b, c, 0, false, false, DirectionParameter.InputOutput, null, null, null, null));
			return this;
		}
		public PropertyMappingBuilder<T> Add (Expression<Func<T, object>> a, DirectionParameter b)
		{
			if (a == null)
				throw new ArgumentNullException ("propertySelector");
			_properties.Add (new PropertyMapping (GDA.Extensions.GetMember (a).Name, null, PersistenceParameterType.Field, 0, false, false, b, null, null, null, null));
			return this;
		}
		public PropertyMappingBuilder<T> Add (PropertyMapping a)
		{
			if (a == null)
				throw new ArgumentNullException ("mapping");
			_properties.Add (a);
			return this;
		}
		public IEnumerator<PropertyMapping> GetEnumerator ()
		{
			return _properties.GetEnumerator ();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return _properties.GetEnumerator ();
		}
	}
}
