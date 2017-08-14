using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
namespace GDA.Mapping
{
	public class ForeignMemberMapping : ForeignItemMapping
	{
		public ForeignMemberMapping (XmlElement a, string b, string c) : base (a, b, c)
		{
		}
		public ForeignMemberMapping (TypeInfo a, string b, string c) : base (a, b, c)
		{
		}
		public PersistenceForeignMemberAttribute GetPersistenceForeignMember ()
		{
			return new PersistenceForeignMemberAttribute (Type.GetType (TypeOfClassRelated.FullnameWithAssembly, true, true), PropertyName, GroupOfRelationship);
		}
	}
}
