using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
namespace GDA.Mapping
{
	public class BaseDAOMapping : ElementMapping
	{
		public TypeInfo TypeInfo {
			get;
			set;
		}
		public string[] GenericTypes {
			get;
			set;
		}
		public BaseDAOMapping (XmlElement a, string b, string c)
		{
			var d = GetAttributeString (a, "name", true);
			this.TypeInfo = new TypeInfo (d, b, c);
			var e = new List<string> ();
			foreach (XmlElement i in a.GetElementsByTagName ("genericType"))
				e.Add (GetAttributeString (i, "name", true));
			GenericTypes = e.ToArray ();
		}
		public BaseDAOMapping (TypeInfo a, string[] b)
		{
			if (a == null)
				throw new ArgumentNullException ("typeInfo");
			this.TypeInfo = a;
			this.GenericTypes = b ?? new string[0];
		}
		public PersistenceBaseDAOAttribute GetPersistenceBaseDAO ()
		{
			var a = Type.GetType (this.TypeInfo.FullnameWithAssembly, true, true);
			var b = new List<Type> ();
			foreach (var i in GenericTypes)
				b.Add (Type.GetType (i, true, true));
			return new PersistenceBaseDAOAttribute (a, b.ToArray ());
		}
	}
}
