using System;
using System.Collections.Generic;
using System.Text;
namespace GDA
{
	[AttributeUsage (AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class PersistenceForeignKeyAttribute : Attribute
	{
		private Type _typeOfClassRelated;
		private string _propertyName;
		private string _groupOfRelationship;
		public Type TypeOfClassRelated {
			get {
				return _typeOfClassRelated;
			}
			set {
				_typeOfClassRelated = value;
			}
		}
		public string PropertyName {
			get {
				return _propertyName;
			}
			set {
				_propertyName = value;
			}
		}
		public string GroupOfRelationship {
			get {
				return _groupOfRelationship;
			}
			set {
				_groupOfRelationship = value;
			}
		}
		public PersistenceForeignKeyAttribute (Type a, string b, int c) : this (a, b, c.ToString ())
		{
		}
		public PersistenceForeignKeyAttribute (Type a, string b, string c)
		{
			_typeOfClassRelated = a;
			_propertyName = b;
			_groupOfRelationship = c;
		}
		public PersistenceForeignKeyAttribute (Type a, string b) : this (a, b, null)
		{
		}
	}
}
