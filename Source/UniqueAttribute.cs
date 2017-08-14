using System;
using System.Collections.Generic;
using System.Text;
using GDA.Sql;
using System.Reflection;
namespace GDA
{
	[AttributeUsage (AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class UniqueAttribute : ValidatorAttribute
	{
		public UniqueAttribute () : base ()
		{
		}
		public UniqueAttribute (string a) : base ()
		{
			MessageText = a;
		}
		public override bool Validate (GDASession a, ValidationMode b, string c, object d, object e)
		{
			if (b == ValidationMode.Delete)
				return true;
			return !GDAOperations.CheckExist (a, b, c, d, e);
		}
	}
}
