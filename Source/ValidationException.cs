using System;
using System.Collections.Generic;
using System.Text;
namespace GDA
{
	public class ValidationException : GDAException
	{
		public ValidationException () : base ("Validation error.")
		{
		}
		public ValidationException (string a) : base (a)
		{
		}
		public ValidationException (string a, Exception b) : base (a, b)
		{
		}
	}
}
