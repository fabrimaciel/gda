using System;
using System.Collections.Generic;
using System.Text;
namespace GDA
{
	public class ValidationMapper
	{
		private Mapper _mapperOwner;
		private ValidatorAttribute[] _validators;
		public Mapper MapperOwner {
			get {
				return _mapperOwner;
			}
		}
		public ValidatorAttribute[] Validators {
			get {
				return _validators;
			}
		}
		public ValidationMapper (Mapper a, ValidatorAttribute[] b)
		{
			if (a == null)
				throw new ArgumentNullException ("mapperOwner");
			else if (b == null)
				throw new ArgumentNullException ("validators");
			_mapperOwner = a;
			_validators = b;
		}
	}
}
