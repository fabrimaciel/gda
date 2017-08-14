using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Collections;
using System.Reflection;
using GDA.Common.Configuration.Attributes;
using GDA.Common.Configuration.Targets;
using GDA.Common.Configuration.Handlers;
namespace GDA.Common.Configuration
{
	internal class ConfigurationMap
	{
		private IList instanceTargets = new ArrayList ();
		private Hashtable instanceOverloads = new Hashtable ();
		private IList staticTargets = new ArrayList ();
		private Hashtable staticOverloads = new Hashtable ();
		public ConfigurationMap (Type a)
		{
			List<MemberAttributeInfo> b = LoadMembersConfigurationAttributes (Helper.ReflectionFlags.InstanceCriteria, a);
			foreach (MemberAttributeInfo mai in b) {
				ConfigurationAttribute c = mai.Attributes [0] as ConfigurationAttribute;
				switch (mai.MemberInfo.MemberType) {
				case MemberTypes.Method:
					MethodTarget d = null;
					if (instanceOverloads.ContainsKey (c.XmlNodePath))
						d = instanceOverloads [c.XmlNodePath] as MethodTarget;
					if (d != null)
						d.AddCallbackMethod (mai.MemberInfo as MethodInfo, c.RequiredParameters);
					else {
						d = new MethodTarget (c, mai.MemberInfo as MethodInfo);
						instanceTargets.Add (d);
						instanceOverloads [c.XmlNodePath] = d;
					}
					break;
				case MemberTypes.Field:
					instanceTargets.Add (new FieldTarget (c, mai.MemberInfo as FieldInfo));
					break;
				case MemberTypes.Property:
					instanceTargets.Add (new PropertyTarget (c, mai.MemberInfo as PropertyInfo));
					break;
				default:
					throw new GDAException ("Unknown configuration target type for member {0} on class {1}.", mai.MemberInfo.Name, a);
				}
			}
			b = LoadMembersConfigurationAttributes (Helper.ReflectionFlags.StaticCriteria, a);
			foreach (MemberAttributeInfo mai in b) {
				ConfigurationAttribute c = mai.Attributes [0] as ConfigurationAttribute;
				switch (mai.MemberInfo.MemberType) {
				case MemberTypes.Method:
					MethodTarget d = null;
					if (staticOverloads.ContainsKey (c.XmlNodePath))
						d = staticOverloads [c.XmlNodePath] as MethodTarget;
					if (d != null)
						d.AddCallbackMethod (mai.MemberInfo as MethodInfo, c.RequiredParameters);
					else {
						d = new MethodTarget (c, mai.MemberInfo as MethodInfo);
						staticTargets.Add (d);
						staticOverloads [c.XmlNodePath] = d;
					}
					break;
				case MemberTypes.Field:
					staticTargets.Add (new FieldTarget (c, mai.MemberInfo as FieldInfo));
					break;
				case MemberTypes.Property:
					staticTargets.Add (new PropertyTarget (c, mai.MemberInfo as PropertyInfo));
					break;
				default:
					throw new GDAException ("Unknown configuration target type for member {0} on class {1}.", mai.MemberInfo.Name, a);
				}
			}
		}
		public void Configure (IList a, object b)
		{
			IList c = b is Type ? staticTargets : instanceTargets;
			foreach (ElementTarget target in c) {
				int d = target is MethodTarget ? a.Count : 1;
				for (int e = 0; e < d; e++) {
					BaseSectionHandler f = a [e] as BaseSectionHandler;
					bool g = false;
					if (target is MethodTarget) {
						XmlNodeList h = f.GetNodes (target.XmlNodePath);
						if (h != null && h.Count > 0) {
							target.Configure (b, h);
							g = true;
							break;
						}
					}
					else {
						XmlNode i = f.GetNode (target.XmlNodePath);
						if (i != null) {
							target.Configure (b, i);
							g = true;
							break;
						}
					}
					VerifyKeyPresence (target, g);
				}
			}
		}
		private List<MemberAttributeInfo> LoadMembersConfigurationAttributes (BindingFlags a, Type b)
		{
			List<MemberAttributeInfo> c = new List<MemberAttributeInfo> ();
			MemberInfo[] d = b.GetMembers (a);
			foreach (MemberInfo mi in d) {
				object[] e = mi.GetCustomAttributes (typeof(ConfigurationAttribute), true);
				if (e != null && e.Length > 0) {
					c.Add (new MemberAttributeInfo (mi, e));
				}
			}
			return c;
		}
		private void VerifyKeyPresence (ElementTarget a, bool b)
		{
			if (!b && a.KeyPresenceRequirement != ConfigKeyPresence.Optional) {
				throw new GDA.Common.Configuration.Exceptions.MissingConfigurationKeyException (a.XmlNodePath);
			}
		}
	}
}
