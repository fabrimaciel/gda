using System;
using System.Collections.Generic;
using System.Text;
using GDA.Common.Configuration.Attributes;
using System.Reflection;
namespace GDA.Common.Configuration.Targets
{
	internal class FieldTarget : ElementTarget
	{
		public readonly FieldInfo RepresentFieldInfo;
		public FieldTarget (ConfigurationAttribute a, FieldInfo b) : base (a)
		{
			this.RepresentFieldInfo = b;
		}
		public override void Configure (object a, System.Xml.XmlNode b)
		{
			object c = GDA.Common.Helper.TypeConverter.Get (RepresentFieldInfo.FieldType, b);
			RepresentFieldInfo.SetValue (a, c, Helper.ReflectionFlags.InstanceCriteria, null, null);
		}
	}
}
