using System;
using System.Collections.Generic;
using System.Text;
namespace GDA.Sql
{
	public class Conditional
	{
		public virtual string Expression {
			get;
			set;
		}
		public virtual ConditionalContainer Parent {
			get;
			internal set;
		}
		protected Conditional ()
		{
		}
		public Conditional (string a)
		{
			if (a == null)
				throw new ArgumentNullException ("expression");
			Expression = a;
		}
		public override string ToString ()
		{
			return Expression;
		}
	}
}
