using System;
using System.Collections.Generic;
using System.Text;
using GDA.Common.Configuration.Attributes;
using System.Reflection;
namespace GDA.Common.Configuration.Targets
{
	internal class PropertyTarget : ElementTarget
	{
		public readonly PropertyInfo RepresentPropertyInfo;
		public PropertyTarget (ConfigurationAttribute a, PropertyInfo b) : base (a)
		{
			this.RepresentPropertyInfo = b;
		}
		public override void Configure (object a, System.Xml.XmlNode b)
		{
			object c = GDA.Common.Helper.TypeConverter.Get (RepresentPropertyInfo.PropertyType, b);
			RepresentPropertyInfo.SetValue (a, c, Helper.ReflectionFlags.InstanceCriteria, null, null, null);
		}
	}
}
