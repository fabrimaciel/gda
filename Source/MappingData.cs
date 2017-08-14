using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
namespace GDA.Mapping
{
	public class MappingData
	{
		private static object objLock = new object ();
		private static Dictionary<string, ClassMapping> _classes = new Dictionary<string, ClassMapping> ();
		private static Dictionary<string, SqlQueryMapping> _queries = new Dictionary<string, SqlQueryMapping> ();
		private static List<ReferenceMapping> _references = new List<ReferenceMapping> ();
		public static ClassMapping GetMapping (Type a)
		{
			if (a == null)
				return null;
			ClassMapping b = null;
			if (_classes.TryGetValue (a.FullName, out b))
				return b;
			return null;
		}
		public static ClassMapping GetMapping (string a)
		{
			ClassMapping b = null;
			if (_classes.TryGetValue (a, out b))
				return b;
			return null;
		}
		public static IEnumerable<ClassMapping> GetMappings ()
		{
			return _classes.Values;
		}
		public static void AddMapping (ClassMapping a)
		{
			if (a == null)
				throw new ArgumentNullException ("mapping");
			if (_classes.ContainsKey (a.TypeInfo.Fullname))
				_classes.Remove (a.TypeInfo.Name);
			_classes.Add (a.TypeInfo.Fullname, a);
		}
		public static void RemoteMapping (string a)
		{
			if (_classes.ContainsKey (a))
				_classes.Remove (a);
		}
		public static SqlQueryMapping GetSqlQuery (string a)
		{
			SqlQueryMapping b = null;
			if (_queries.TryGetValue (a, out b))
				return b;
			return null;
		}
		public static IEnumerable<SqlQueryMapping> GetSqlQueries ()
		{
			return _queries.Values;
		}
		public static void AddSqlQuery (SqlQueryMapping a)
		{
			if (a == null)
				throw new ArgumentNullException ("query");
			if (_queries.ContainsKey (a.Name))
				_queries.Remove (a.Name);
			_queries.Add (a.Name, a);
		}
		public static void RemoteSqlQuery (string a)
		{
			if (_queries.ContainsKey (a))
				_queries.Remove (a);
		}
		public static void Import (string a, string b)
		{
			var c = System.Reflection.Assembly.Load (a);
			if (c == null)
				return;
			using (System.IO.Stream d = c.GetManifestResourceStream (b)) {
				if (d == null)
					throw new GDAMappingException ("Not found resource \"{0}\" in \"{1}\".", b, a);
				Import (d);
			}
		}
		public static void Import (string a)
		{
			if (string.IsNullOrEmpty (a))
				throw new ArgumentNullException ("fileName");
			if (!System.IO.File.Exists (a))
				throw new GDAMappingException ("Mapping file \"{0}\" not exists.", a);
			using (var b = new System.IO.FileStream (a, System.IO.FileMode.Open, System.IO.FileAccess.Read))
				Import (b);
		}
		public static void Import (System.IO.Stream a)
		{
			if (a == null)
				throw new ArgumentNullException ("inputStream");
			try {
				var b = XmlReader.Create (a, new XmlReaderSettings {
					IgnoreWhitespace = true,
					IgnoreComments = true,
				});
				var c = new System.Xml.XmlDocument ();
				c.Load (b);
				var d = c.DocumentElement;
				if (!(d.LocalName == "gda-mapping" && d.NamespaceURI == "urn:gda-mapping-1.0"))
					return;
				var e = ElementMapping.GetAttributeString (d, "namespace");
				var f = ElementMapping.GetAttributeString (d, "assembly");
				var g = ElementMapping.GetAttributeString (d, "defaultProviderName");
				var h = ElementMapping.GetAttributeString (d, "defaultProviderConfigurationName");
				var j = ElementMapping.GetAttributeString (d, "defaultConnectionString");
				var k = ElementMapping.GetAttributeString (d, "defaultSchema");
				foreach (XmlElement referencesElement in d.GetElementsByTagName ("references")) {
					foreach (XmlElement i in referencesElement.GetElementsByTagName ("reference")) {
						var l = new ReferenceMapping (i);
						bool n = false;
						lock (objLock)
							n = _references.Exists (o => o.Equals (l));
						if (!n) {
							lock (objLock)
								_references.Add (l);
							if (!string.IsNullOrEmpty (l.FileName))
								Import (l.FileName);
							else
								Import (l.AssemblyName, l.ResourceName);
						}
					}
					break;
				}
				lock (objLock) {
					foreach (XmlElement i in d.GetElementsByTagName ("class")) {
						var p = new ClassMapping (i, e, f, g, h, j, k);
						if (!_classes.ContainsKey (p.TypeInfo.Fullname))
							_classes.Add (p.TypeInfo.Fullname, p);
					}
					foreach (XmlElement i in d.GetElementsByTagName ("sql-query")) {
						var q = new SqlQueryMapping (i);
						if (!_queries.ContainsKey (q.Name))
							_queries.Add (q.Name, q);
					}
					var r = ElementMapping.FirstOrDefault<XmlElement> (d.GetElementsByTagName ("modelsNamespace"));
					if (r != null)
						foreach (XmlElement i in r.GetElementsByTagName ("namespace")) {
							var s = new ModelsNamespaceMapping (i);
							GDASettings.AddModelsNamespace (s.Assembly, s.Namespace);
						}
					XmlElement t = ElementMapping.FirstOrDefault<XmlElement> (d.GetElementsByTagName ("generateKeyHandler"));
					if (t != null) {
						GDASettings.DefineGenerateKeyHandler (ElementMapping.GetAttributeString (t, "classType", true), ElementMapping.GetAttributeString (t, "methodName", true));
					}
					var u = ElementMapping.FirstOrDefault<XmlElement> (d.GetElementsByTagName ("generatorsKey"));
					if (u != null)
						foreach (XmlElement i in u.GetElementsByTagName ("generator")) {
							var w = new GeneratorKeyMapping (i);
							IGeneratorKey x = null;
							try {
								x = Activator.CreateInstance (w.ClassType) as IGeneratorKey;
							}
							catch (Exception ex) {
								if (ex is System.Reflection.TargetInvocationException)
									ex = ex.InnerException;
								throw new GDAMappingException ("Fail on create instance for \"{0}\".", w.ClassType.FullName);
							}
							if (x == null)
								throw new GDAMappingException ("\"{0}\" not inherits of {1}.", w.ClassType.FullName, typeof(IGeneratorKey).FullName);
							GDASettings.AddGeneratorKey (w.Name, x);
						}
				}
			}
			catch (Exception ex) {
				if (ex is GDAMappingException)
					throw ex;
				else
					throw new GDAMappingException ("Fail on load mapping", ex);
			}
		}
		public static XmlDocument RefactorSystemMapping (System.Reflection.Assembly a)
		{
			var b = new XmlDocument ();
			var c = a.FullName;
			c = c.Substring (0, c.IndexOf (','));
			var d = b.CreateElement ("gda-mapping");
			d.SetAttribute ("assembly", a.FullName);
			d.SetAttribute ("namespace", c);
			d.SetAttribute ("xmlns", "urn:gda-mapping-1.0");
			var e = b.CreateElement ("modelsNamespace");
			foreach (var i in GDASettings.ModelsNamespaces) {
				var f = b.CreateElement ("namespace");
				f.SetAttribute ("name", i.Namespace);
				f.SetAttribute ("assembly", i.AssemblyName);
				e.AppendChild (f);
			}
			d.AppendChild (e);
			#if !PocketPC
			if (GDAOperations.GlobalGenerateKey != null) {
				var g = GDAOperations.GlobalGenerateKey.Method;
				var h = b.CreateElement ("generateKeyHandler");
				h.SetAttribute ("methodName", g.Name);
				h.SetAttribute ("classType", g.DeclaringType.FullName);
			}
			#endif
			var j = b.CreateElement ("generatorsKey");
			foreach (var i in GDASettings.GetGeneratorsKey ()) {
				var k = b.CreateElement ("generator");
				k.SetAttribute ("name", i.Key);
				k.SetAttribute ("classType", i.Value.GetType ().FullName);
				j.AppendChild (k);
			}
			d.AppendChild (j);
			foreach (var i in a.GetTypes ()) {
				var l = RefactorClass (i, c, b);
				if (l != null)
					d.AppendChild (l);
			}
			b.AppendChild (d);
			return b;
		}
		public static XmlElement RefactorClass (Type a, string b, XmlDocument c)
		{
			var d = Caching.MappingManager.GetPersistenceClassAttribute (a);
			var e = c.CreateElement ("class");
			var f = a.FullName;
			if (f.StartsWith (b + '.'))
				f = f.Substring (b.Length + 1);
			e.SetAttribute ("name", f);
			if (d != null) {
				e.SetAttribute ("table", d.Name);
				if (!string.IsNullOrEmpty (d.Schema))
					e.SetAttribute ("schema", d.Schema);
			}
			var g = Caching.MappingManager.GetPersistenceBaseDAOAttribute (a);
			if (g != null) {
				var h = c.CreateElement ("baseDAO");
				f = g.BaseDAOType.FullName;
				if (f.StartsWith (b + '.'))
					f = f.Substring (b.Length + 1);
				h.SetAttribute ("name", f);
				if (g.BaseDAOGenericTypes != null)
					foreach (var i in g.BaseDAOGenericTypes) {
						var j = c.CreateElement ("genericType");
						j.SetAttribute ("name", i.FullName);
					}
				e.AppendChild (h);
			}
			var k = Caching.MappingManager.GetPersistenceProviderAttribute (a);
			if (k != null) {
				var l = c.CreateElement ("provider");
				l.SetAttribute ("name", k.ProviderName);
				if (!string.IsNullOrEmpty (k.ProviderConfigurationName))
					l.SetAttribute ("configurationName", k.ProviderConfigurationName);
				if (!string.IsNullOrEmpty (k.ConnectionString)) {
					var n = c.CreateElement ("connectionString");
					n.InnerText = k.ConnectionString;
					l.AppendChild (n);
				}
				e.AppendChild (l);
			}
			var o = Caching.MappingManager.GetForeignMemberMapper (a);
			foreach (var m in Caching.MappingManager.GetMappers (a)) {
				if (m.PropertyMapper.DeclaringType != m.PropertyMapper.ReflectedType)
					continue;
				var p = c.CreateElement ("property");
				p.SetAttribute ("name", m.PropertyMapperName);
				if (m.Name != m.PropertyMapperName)
					p.SetAttribute ("column", m.Name);
				if (m.ParameterType != PersistenceParameterType.Field)
					p.SetAttribute ("parameterType", m.ParameterType.ToString ());
				if (m.Size > 0)
					p.SetAttribute ("size", m.Size.ToString ());
				if (m.Direction != DirectionParameter.InputOutput)
					p.SetAttribute ("direction", m.Direction.ToString ());
				if (m.IsNotNull)
					p.SetAttribute ("not-null", m.IsNotNull.ToString ());
				if (!string.IsNullOrEmpty (m.GeneratorKeyName)) {
					var j = c.CreateElement ("generator");
					j.SetAttribute ("name", m.GeneratorKeyName);
					p.AppendChild (j);
				}
				if (m.ForeignKeys != null)
					foreach (var fk in m.ForeignKeys) {
						var q = c.CreateElement ("foreignKey");
						f = fk.TypeOfClassRelated.FullName;
						if (f.StartsWith (b + '.'))
							f = f.Substring (b.Length + 1);
						q.SetAttribute ("typeOfClassRelated", f);
						q.SetAttribute ("propertyName", fk.PropertyOfClassRelated.Name);
						if (!string.IsNullOrEmpty (fk.GroupOfRelationship))
							q.SetAttribute ("groupOfRelationship", fk.GroupOfRelationship);
						p.AppendChild (q);
					}
				foreach (var fkm in o)
					if (fkm.PropertyModel.Name == m.PropertyMapperName) {
						var q = c.CreateElement ("foreignKey");
						f = fkm.TypeOfClassRelated.FullName;
						if (f.StartsWith (b + '.'))
							f = f.Substring (b.Length + 1);
						q.SetAttribute ("typeOfClassRelated", f);
						q.SetAttribute ("propertyName", fkm.PropertyOfClassRelated.Name);
						if (!string.IsNullOrEmpty (fkm.GroupOfRelationship))
							q.SetAttribute ("groupOfRelationship", fkm.GroupOfRelationship);
						p.AppendChild (q);
					}
				if (m.Validation != null && m.Validation.Validators != null)
					foreach (var v in m.Validation.Validators) {
						var r = c.CreateElement ("validator");
						r.SetAttribute ("name", r.Name);
						foreach (var pi in v.GetType ().GetProperties ())
							try {
								var s = pi.GetValue (v, null);
								var t = c.CreateElement ("param");
								t.SetAttribute ("name", pi.Name);
								t.InnerText = s.ToString ();
							}
							catch {
							}
					}
				e.AppendChild (p);
			}
			if (e.IsEmpty)
				return null;
			return e;
		}
	}
}
