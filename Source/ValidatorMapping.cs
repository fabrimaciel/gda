using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
namespace GDA.Mapping
{
	public class ValidatorMapping : ElementMapping
	{
		public string Name {
			get;
			set;
		}
		public List<ValidatorParamMapping> Parameters {
			get;
			set;
		}
		public ValidatorMapping (XmlElement a)
		{
			if (a == null)
				throw new ArgumentNullException ("element");
			Name = GetAttributeString (a, "name", true);
			Parameters = new List<ValidatorParamMapping> ();
			foreach (XmlElement i in a.GetElementsByTagName ("param")) {
				var b = new ValidatorParamMapping (i);
				if (!Parameters.Exists (c => c.Name == b.Name))
					Parameters.Add (b);
			}
		}
		public ValidatorMapping (string a, IEnumerable<ValidatorParamMapping> b)
		{
			if (string.IsNullOrEmpty (a))
				throw new ArgumentNullException ("name");
			this.Name = a;
			Parameters = new List<ValidatorParamMapping> ();
			if (b != null)
				foreach (var i in b)
					if (!Parameters.Exists (c => c.Name == i.Name))
						Parameters.Add (i);
		}
		public ValidatorAttribute GetValidator ()
		{
			var a = Name;
			Type b = null;
			switch (Name) {
			case "Unique":
				b = typeof(UniqueAttribute);
				break;
			case "RangeValidator":
			case "Range":
				b = typeof(RangeValidatorAttribute);
				break;
			case "RequiredValidator":
			case "Required":
				b = typeof(RequiredValidatorAttribute);
				break;
			default:
				b = Type.GetType (a, false, true);
				break;
			}
			if (b == null) {
				var c = a.Split (',');
				a = c [0].Trim () + "Attribute";
				if (c.Length > 1)
					a += ", " + c [1];
				b = Type.GetType (a, false, true);
				if (b == null)
					throw new GDAMappingException ("Fail on instance validator type \"{0}\"", Name);
			}
			var d = Activator.CreateInstance (b) as ValidatorAttribute;
			if (d == null)
				return null;
			foreach (var i in Parameters) {
				var e = b.GetProperty (i.Name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
				if (e != null && e.CanWrite && !string.IsNullOrEmpty (i.Value)) {
					try {
						if (e.PropertyType.IsEnum)
							e.SetValue (d, Enum.Parse (e.PropertyType, i.Value, true), null);
						else if (e.PropertyType == typeof(string))
							e.SetValue (d, i.Value, null);
						else
							e.SetValue (d, typeof(Convert).GetMethod ("To" + e.PropertyType.Name, new Type[] {
								typeof(string)
							}).Invoke (null, new object[] {
								i.Value
							}), null);
					}
					catch (Exception ex) {
						if (ex is System.Reflection.TargetInvocationException)
							ex = ex.InnerException;
						throw new GDAMappingException (string.Format ("Fail on set validator \"{0}\" property \"{1}\" value \"{2}\"", Name, e.Name, i.Value), ex);
					}
				}
			}
			return d;
		}
	}
}
