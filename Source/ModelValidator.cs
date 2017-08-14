using System;
using System.Collections.Generic;
using System.Text;
namespace GDA
{
	public enum ValidationMode
	{
		Insert,
		Update,
		Delete
	}
	public sealed class ModelValidator
	{
		public static ValidationResult Validate (ValidationMode a, object b)
		{
			return Validate (null, a, b);
		}
		public static ValidationResult Validate (GDASession a, ValidationMode b, object c)
		{
			if (c == null)
				throw new ArgumentNullException ("obj");
			List<ValidationMessage> d = new List<ValidationMessage> ();
			IList<Mapper> e = Caching.MappingManager.GetMappers (c.GetType ());
			foreach (Mapper m in e) {
				object f = m.PropertyMapper.GetValue (c, null);
				if (m.Validation != null)
					foreach (ValidatorAttribute va in m.Validation.Validators) {
						bool g = va.Validate (a, b, m.PropertyMapperName, f, c);
						if (!g) {
							if (va.Message == null) {
								ValidationMessage h = new ValidationMessage ();
								h.Message = va.GetType ().Name.Replace ("Attribute", "");
								h.PropertyName = m.PropertyMapperName;
								d.Add (h);
							}
							else
								d.Add (va.Message.Clone (m.PropertyMapperName));
						}
					}
			}
			return new ValidationResult (d, d.Count == 0);
		}
	}
}
