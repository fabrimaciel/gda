using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
namespace GDA
{
	public class ForeignKeyMapper : ForeignMapper
	{
		public ForeignKeyMapper (PersistenceForeignKeyAttribute a, PropertyInfo b)
		{
			PropertyInfo c = a.TypeOfClassRelated.GetProperty (a.PropertyName);
			if (c == null)
				throw new GDAException ("Property {0} not found in class {1}", a.PropertyName, a.TypeOfClassRelated.FullName);
			this.TypeOfClassRelated = a.TypeOfClassRelated;
			this.PropertyOfClassRelated = c;
			this.GroupOfRelationship = a.GroupOfRelationship;
			this.PropertyModel = b;
		}
	}
}
