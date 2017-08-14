using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace GDA.Mapping
{
	public class MappingExtractor
	{
		public void ExtractXmlMapping (System.Reflection.Assembly a, System.IO.Stream b)
		{
			if (a == null)
				throw new ArgumentNullException ("assembly");
			if (b == null)
				throw new ArgumentNullException ("outStream");
			var c = a.GetTypes ();
			var d = new System.Xml.XmlDocument ();
			var e = d.CreateElement ("gda-mapping");
			d.AppendChild (e);
			e.SetAttribute ("namespace", "");
			e.SetAttribute ("assembly", a.GetName ().Name);
			foreach (var type in c) {
				var f = type.GetCustomAttributes (typeof(global::GDA.PersistenceClassAttribute), false);
				if (f.Length > 0) {
					var g = (global::GDA.PersistenceClassAttribute)f [0];
					var h = d.CreateElement ("class");
					h.SetAttribute ("name", type.FullName);
					h.SetAttribute ("table", g.Name);
					h.SetAttribute ("schema", g.Schema);
					e.AppendChild (h);
					foreach (var property in type.GetProperties ()) {
						var i = (global::GDA.PersistencePropertyAttribute)property.GetCustomAttributes (typeof(global::GDA.PersistencePropertyAttribute), false).FirstOrDefault ();
						if (i != null) {
							var j = d.CreateElement ("property");
							j.SetAttribute ("name", property.Name);
							if (!StringComparer.InvariantCultureIgnoreCase.Equals (property.Name, i.Name))
								j.SetAttribute ("column", i.Name);
							if (i.ParameterType != PersistenceParameterType.Field)
								j.SetAttribute ("parameterType", i.ParameterType.ToString ());
							if (i.Size > 0)
								j.SetAttribute ("size", i.Size.ToString ());
							if (i.Direction != DirectionParameter.InputOutput)
								j.SetAttribute ("direction", i.Direction.ToString ());
							if (i.IsNotNull)
								j.SetAttribute ("not-null", i.IsNotNull.ToString ().ToLower ());
							h.AppendChild (j);
						}
					}
				}
			}
			d.Save (b);
		}
	}
}
