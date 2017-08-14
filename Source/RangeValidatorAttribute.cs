using System;
using System.Collections.Generic;
using System.Text;
namespace GDA
{
	[AttributeUsage (AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
	public class RangeValidatorAttribute : ValidatorAttribute
	{
		private object min;
		private object max;
		public object Min {
			get {
				return min;
			}
			set {
				min = value;
			}
		}
		public object Max {
			get {
				return max;
			}
			set {
				max = value;
			}
		}
		public RangeValidatorAttribute () : this (null, null)
		{
		}
		public RangeValidatorAttribute (object a, object b, string c)
		{
			MessageText = c;
			this.min = a;
			this.max = b;
		}
		public RangeValidatorAttribute (object a, object b)
		{
			this.min = a;
			this.max = b;
		}
		public override bool Validate (GDASession a, ValidationMode b, string c, object d, object f)
		{
			if (b == ValidationMode.Delete)
				return true;
			if (d == null) {
				return false;
			}
			else {
				Type g = d.GetType ();
				if (g == typeof(short) || g == typeof(int) || g == typeof(long)) {
					return ValidateRangeLong (Convert.ToInt64 (d));
				}
				else if (g == typeof(float) || g == typeof(double)) {
					return ValidateRangeDouble (Convert.ToDouble (d));
				}
				else if (g == typeof(DateTime)) {
					return ValidateRangeDateTime ((DateTime)d);
				}
				return false;
			}
		}
		private bool ValidateRangeLong (long a)
		{
			long b = min == null ? long.MinValue : Convert.ToInt64 (min);
			long c = max == null ? long.MaxValue : Convert.ToInt64 (max);
			return a >= b && a <= c;
		}
		private bool ValidateRangeDouble (double a)
		{
			double b = min == null ? double.MinValue : Convert.ToDouble (min);
			double c = max == null ? double.MaxValue : Convert.ToDouble (max);
			return a >= b && a <= c;
		}
		private bool ValidateRangeDateTime (DateTime a)
		{
			DateTime b = min == null ? DateTime.MinValue : GetDateTime (min);
			DateTime c = max == null ? DateTime.MaxValue : GetDateTime (max);
			return a >= b && a <= c;
		}
		private DateTime GetDateTime (object a)
		{
			if (a.GetType () == typeof(DateTime)) {
				return (DateTime)a;
			}
			try {
				string b = "0001-01-01 00:00:00.000";
				string c = (string)a;
				c += b.Substring (c.Length, b.Length - c.Length);
				return DateTime.Parse (c, System.Globalization.DateTimeFormatInfo.InvariantInfo);
			}
			catch (Exception e) {
				throw;
			}
		}
	}
}
