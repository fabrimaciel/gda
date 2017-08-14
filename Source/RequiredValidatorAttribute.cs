using System;
using System.Collections.Generic;
using System.Text;
namespace GDA
{
	[AttributeUsage (AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
	public class RequiredValidatorAttribute : ValidatorAttribute
	{
		private bool allowNull = false;
		public bool AllowNull {
			get {
				return allowNull;
			}
			set {
				allowNull = value;
			}
		}
		public RequiredValidatorAttribute () : base ()
		{
		}
		public RequiredValidatorAttribute (string a) : base ()
		{
			MessageText = a;
		}
		public override bool Validate (GDASession a, ValidationMode b, string c, object d, object e)
		{
			if (b == ValidationMode.Delete)
				return true;
			if (d == null) {
				return allowNull;
			}
			return !(d is string && string.Empty == (string)d);
		}
	}
}
