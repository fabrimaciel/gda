using System;
using System.Collections.Generic;
using System.Text;
namespace GDA
{
	[AttributeUsage (AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
	public abstract class ValidatorAttribute : Attribute
	{
		private ValidationMessage _message = new ValidationMessage ();
		public virtual ValidationMessage Message {
			get {
				return _message;
			}
			protected set {
				_message = value;
			}
		}
		public virtual string MessageText {
			get {
				return _message.Message;
			}
			set {
				_message.Message = value;
			}
		}
		public virtual bool Validate (GDASession a, ValidationMode b, string c, object d, object e)
		{
			return true;
		}
	}
}
