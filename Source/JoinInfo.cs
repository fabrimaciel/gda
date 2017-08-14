using System;
using System.Collections.Generic;
using System.Text;
namespace GDA.Sql
{
	public enum JoinType
	{
		InnerJoin,
		LeftJoin,
		RightJoin,
		CrossJoin
	}
	public class JoinInfo
	{
		private Type _classTypeMain;
		private Type _classTypeJoin;
		private JoinType _type;
		private string _classTypeMainAlias;
		private string _classTypeJoinAlias;
		private string _groupOfRelationship;
		private string _joinTableAlias;
		public Type ClassTypeMain {
			get {
				return _classTypeMain;
			}
			set {
				_classTypeMain = value;
			}
		}
		public Type ClassTypeJoin {
			get {
				return _classTypeJoin;
			}
			set {
				if (value == null)
					throw new ArgumentNullException ("TypeClassJoin");
				_classTypeJoin = value;
			}
		}
		public JoinType Type {
			get {
				return _type;
			}
			set {
				_type = value;
			}
		}
		public string ClassTypeMainAlias {
			get {
				return _classTypeMainAlias;
			}
			set {
				_classTypeMainAlias = value;
			}
		}
		public string ClassTypeJoinAlias {
			get {
				return _classTypeJoinAlias;
			}
			set {
				_classTypeJoinAlias = value;
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
		internal string JoinTableAlias {
			get {
				return _joinTableAlias;
			}
			set {
				_joinTableAlias = value;
			}
		}
		public JoinInfo (JoinType a, Type b, Type c, string d, string e, string f)
		{
			if (c == null)
				throw new ArgumentNullException ("typeClassJoin");
			_type = a;
			_classTypeJoin = c;
			_groupOfRelationship = f;
			_classTypeMainAlias = d;
			_classTypeJoinAlias = e;
			_classTypeMain = b;
		}
	}
}
