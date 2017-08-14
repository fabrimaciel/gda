using System;
using System.Collections.Generic;
using System.Text;
namespace GDA
{
	[AttributeUsage (AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
	public class PersistenceBaseDAOAttribute : Attribute
	{
		private Type _baseDAOType;
		private Type[] _baseDAOGenericTypes = null;
		public Type BaseDAOType {
			get {
				return _baseDAOType;
			}
		}
		public Type[] BaseDAOGenericTypes {
			get {
				return _baseDAOGenericTypes;
			}
		}
		public PersistenceBaseDAOAttribute (Type a)
		{
			_baseDAOType = a;
		}
		public PersistenceBaseDAOAttribute (Type a, params Type[] b) : this (a)
		{
			_baseDAOGenericTypes = b;
		}
	}
}
