using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Collections;
namespace GDA.Common.Configuration
{
	internal class MemberAttributeInfo
	{
		public readonly MemberInfo MemberInfo;
		public readonly IList Attributes;
		public MemberAttributeInfo (MemberInfo a, IList b)
		{
			this.MemberInfo = a;
			this.Attributes = b;
		}
		public Attribute this [int a] {
			get {
				return Attributes [a] as Attribute;
			}
		}
	}
}
