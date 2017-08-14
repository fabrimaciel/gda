using System;
using System.Collections.Generic;
using System.Text;
namespace GDA
{
	public class GroupOfRelationshipInfo : IEquatable<GroupOfRelationshipInfo>
	{
		private Type _typeOfClass;
		private Type _typeOfClassRelated;
		private string _groupOfRelationship;
		private List<ForeignKeyMapper> _foreignKeys = new List<ForeignKeyMapper> ();
		public Type TypeOfClass {
			get {
				return _typeOfClass;
			}
			set {
				_typeOfClass = value;
			}
		}
		public Type TypeOfClassRelated {
			get {
				return _typeOfClassRelated;
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
		public List<ForeignKeyMapper> ForeignKeys {
			get {
				return _foreignKeys;
			}
		}
		public GroupOfRelationshipInfo (Type a, Type b, string c)
		{
			if (a == null)
				throw new ArgumentNullException ("typeOfClasse");
			else if (b == null)
				throw new ArgumentNullException ("typeOfClassRelated");
			_typeOfClass = a;
			_typeOfClassRelated = b;
			_groupOfRelationship = c;
		}
		public void AddForeignKey (ForeignKeyMapper a)
		{
			_foreignKeys.Add (a);
		}
		public override bool Equals (object a)
		{
			if (a is GroupOfRelationshipInfo)
				return Equals ((GroupOfRelationshipInfo)a);
			else
				return false;
		}
		public bool Equals (GroupOfRelationshipInfo a)
		{
			return _typeOfClassRelated == a.TypeOfClassRelated && _groupOfRelationship == a.GroupOfRelationship;
		}
		public static bool operator == (GroupOfRelationshipInfo a, GroupOfRelationshipInfo b) {
			bool c, d;
			c = Object.ReferenceEquals (a, null);
			d = Object.ReferenceEquals (b, null);
			if (c && d)
				return true;
			if (!c && !d)
				return a.Equals (b);
			else
				return false;
		}
		public static bool operator != (GroupOfRelationshipInfo a, GroupOfRelationshipInfo b) {
			bool c, d;
			c = Object.ReferenceEquals (a, null);
			d = Object.ReferenceEquals (b, null);
			if (c && d)
				return false;
			if (!c && !d)
				return !a.Equals (b);
			else
				return true;
		}
	}
}
