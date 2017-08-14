using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
namespace GDA
{
	public abstract class ForeignMapper
	{
		private Type _typeOfClassRelated;
		private PropertyInfo _propertyOfClassRelated;
		private PropertyInfo _propertyModel;
		private string _groupOfRelationship;
		public Type TypeOfClassRelated {
			get {
				return _typeOfClassRelated;
			}
			set {
				_typeOfClassRelated = value;
			}
		}
		public PropertyInfo PropertyOfClassRelated {
			get {
				return _propertyOfClassRelated;
			}
			set {
				_propertyOfClassRelated = value;
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
		public PropertyInfo PropertyModel {
			get {
				return _propertyModel;
			}
			set {
				_propertyModel = value;
			}
		}
	}
}
