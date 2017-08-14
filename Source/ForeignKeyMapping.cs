using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
namespace GDA.Mapping
{
	public class ForeignKeyMapping : ForeignItemMapping
	{
		public ForeignKeyMapping (XmlElement a, string b, string c) : base (a, b, c)
		{
		}
		public ForeignKeyMapping (TypeInfo a, string b, string c) : base (a, b, c)
		{
		}
		public PersistenceForeignKeyAttribute GetPersistenceForeignKey ()
		{
			return new PersistenceForeignKeyAttribute (Type.GetType (TypeOfClassRelated.FullnameWithAssembly, true, true), PropertyName, GroupOfRelationship);
		}
	}
}
