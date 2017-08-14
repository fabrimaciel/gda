using System;
using System.Collections.Generic;
using System.Text;
namespace GDA
{
	public class ValidationResult
	{
		public List<ValidationMessage> Messages {
			get;
			private set;
		}
		public bool IsValid {
			get;
			set;
		}
		internal ValidationResult (List<ValidationMessage> a, bool b)
		{
			Messages = a;
			IsValid = b;
		}
		public ValidationResult ()
		{
			Messages = new List<ValidationMessage> ();
		}
	}
}
