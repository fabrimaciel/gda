using System;
using System.Collections.Generic;
using System.Text;
namespace GDA
{
	public class ValidationMessage
	{
		private string _Id;
		private string _message;
		private string _propertyName;
		public string Id {
			get {
				return _Id;
			}
			set {
				_Id = value;
			}
		}
		public string Message {
			get {
				return _message;
			}
			set {
				_message = value;
			}
		}
		public string PropertyName {
			get {
				return _propertyName;
			}
			set {
				_propertyName = value;
			}
		}
		public virtual ValidationMessage Clone (string a)
		{
			ValidationMessage b = new ValidationMessage ();
			b.Message = this.Message;
			b.Id = this.Id;
			b.PropertyName = a;
			return b;
		}
	}
}
