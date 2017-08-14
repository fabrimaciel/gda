using System;
using System.Collections.Generic;
using System.Text;
using GDA.Interfaces;
using System.Reflection;
namespace GDA.Caching
{
	public class MappingManager
	{
		internal class LoadPersistencePropertyAttributesResult
		{
			public List<Mapper> Mappers;
			public List<GroupOfRelationshipInfo> ForeignKeyMappers;
			public List<ForeignMemberMapper> ForeignMembers;
		}
		internal static Dictionary<Type, ISimpleBaseDAO> MembersDAO = new Dictionary<Type, ISimpleBaseDAO> ();
		internal static Dictionary<string, Dictionary<string, string>> ModelsNamespaceLoaded = new Dictionary<string, Dictionary<string, string>> ();
		private static int[] _accessModelsMapper;
		private static string[] _typeNamesModelsMapper;
		private static List<Mapper>[] _listsModelsMapper;
		private static List<GroupOfRelationshipInfo>[] _listForeignKeyMapper;
		private static List<ForeignMemberMapper>[] _listForeignMemberMapper;
		private static object _loadPersistencePropertyAttributesLock = new object ();
		static MappingManager ()
		{
			try {
				GDASettings.LoadConfiguration ();
			}
			catch {
			}
		}
		internal static PersistenceBaseDAOAttribute GetPersistenceBaseDAOAttribute (Type a)
		{
			object[] b = a.GetCustomAttributes (typeof(PersistenceBaseDAOAttribute), false);
			if (b.Length == 0) {
				var c = Mapping.MappingData.GetMapping (a);
				return c != null && c.BaseDAO != null ? c.BaseDAO.GetPersistenceBaseDAO () : null;
			}
			else
				return (PersistenceBaseDAOAttribute)b [0];
		}
		internal static PersistenceProviderAttribute GetPersistenceProviderAttribute (Type a)
		{
			object[] b = a.GetCustomAttributes (typeof(PersistenceProviderAttribute), true);
			if (b.Length == 0) {
				var c = Mapping.MappingData.GetMapping (a);
				return c != null && c.Provider != null ? c.Provider.GetPersistenceProvider () : null;
			}
			else
				return (PersistenceProviderAttribute)b [0];
		}
		internal static PersistenceClassAttribute GetPersistenceClassAttribute (Type a)
		{
			object[] b = a.GetCustomAttributes (typeof(PersistenceClassAttribute), true);
			if (b.Length == 0) {
				var c = Mapping.MappingData.GetMapping (a);
				if (c == null && a.BaseType != typeof(object))
					c = Mapping.MappingData.GetMapping (a.BaseType);
				if (c == null)
					foreach (var i in a.GetInterfaces ()) {
						c = Mapping.MappingData.GetMapping (i);
						if (c != null)
							break;
					}
				return c != null ? c.GetPersistenceClass () : null;
			}
			else
				return (PersistenceClassAttribute)b [0];
		}
		internal static PersistencePropertyAttribute GetPersistenceProperty (PropertyInfo a)
		{
			object[] b = a.GetCustomAttributes (typeof(PersistencePropertyAttribute), true);
			if (b != null && b.Length > 0) {
				if (string.IsNullOrEmpty (((PersistencePropertyAttribute)b [0]).Name))
					((PersistencePropertyAttribute)b [0]).Name = a.Name;
				return (PersistencePropertyAttribute)b [0];
			}
			else {
				Type c = a.DeclaringType;
				var d = Mapping.MappingData.GetMapping (a.DeclaringType);
				if (d == null && a.DeclaringType != a.ReflectedType) {
					d = Mapping.MappingData.GetMapping (a.ReflectedType);
					c = a.ReflectedType;
				}
				if (d != null) {
					do {
						GDA.Mapping.PropertyMapping e = null;
						if (d != null)
							e = d.Properties.Find (f => f.Name == a.Name);
						if (e != null)
							return e.GetPersistenceProperty ();
						else {
							c = c.BaseType;
							d = Mapping.MappingData.GetMapping (c);
						}
					}
					while (c != typeof(object));
				}
			}
			return null;
		}
		internal static Mapper GetPropertyMapper (Type a, string b)
		{
			var c = MappingManager.LoadPersistencePropertyAttributes (a);
			return c.Mappers.Find (delegate (Mapper d) {
				return d.PropertyMapperName == b;
			});
		}
		internal static LoadPersistencePropertyAttributesResult LoadPersistencePropertyAttributes (Type a)
		{
			if (a == null)
				throw new ArgumentNullException ("type");
			int b = -1;
			List<Mapper> c = null;
			List<GroupOfRelationshipInfo> d = null;
			List<ForeignMemberMapper> e = null;
			lock (_loadPersistencePropertyAttributesLock) {
				if (_accessModelsMapper == null) {
					_accessModelsMapper = new int[GDASettings.MaximumMapperCache];
					_typeNamesModelsMapper = new string[GDASettings.MaximumMapperCache];
					_listsModelsMapper = new List<Mapper>[GDASettings.MaximumMapperCache];
					_listForeignKeyMapper = new List<GroupOfRelationshipInfo>[GDASettings.MaximumMapperCache];
					_listForeignMemberMapper = new List<ForeignMemberMapper>[GDASettings.MaximumMapperCache];
					for (int f = 0; f < _accessModelsMapper.Length; f++)
						_accessModelsMapper [f] = -1;
				}
				for (int f = 0; f < _typeNamesModelsMapper.Length; f++)
					if (_typeNamesModelsMapper [f] != null && _typeNamesModelsMapper [f] == a.FullName) {
						_accessModelsMapper [f]++;
						b = f;
					}
					else {
						if (_accessModelsMapper [f] > int.MinValue)
							_accessModelsMapper [f]--;
					}
				if (b >= 0) {
					return new LoadPersistencePropertyAttributesResult {
						Mappers = _listsModelsMapper [b],
						ForeignKeyMappers = _listForeignKeyMapper [b],
						ForeignMembers = _listForeignMemberMapper [b]
					};
				}
				c = new List<Mapper> ();
				d = new List<GroupOfRelationshipInfo> ();
				e = new List<ForeignMemberMapper> ();
				b = 0;
				for (int f = 1; f < _accessModelsMapper.Length; f++)
					if (_accessModelsMapper [f] < _accessModelsMapper [b])
						b = f;
				if (b != -1 && _listsModelsMapper [b] != null) {
				}
			}
			LoadPersistencePropertyAttributes (a, c, d, e);
			lock (_loadPersistencePropertyAttributesLock) {
				_accessModelsMapper [b] = 10;
				_typeNamesModelsMapper [b] = a.FullName;
				_listsModelsMapper [b] = c;
				_listForeignKeyMapper [b] = d;
				_listForeignMemberMapper [b] = e;
			}
			return new LoadPersistencePropertyAttributesResult {
				Mappers = c,
				ForeignKeyMappers = d,
				ForeignMembers = e
			};
		}
		private static void LoadPersistencePropertyAttributes (Type a, List<Mapper> b, List<GroupOfRelationshipInfo> c, List<ForeignMemberMapper> d)
		{
			PropertyInfo[] e = a.GetProperties (BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
			var f = Mapping.MappingData.GetMapping (a);
			foreach (PropertyInfo property in e) {
				Mapping.PropertyMapping g = null;
				if (f != null)
					g = f.Properties.Find (h => h.Name == property.Name);
				PersistencePropertyAttribute j = GetPersistenceProperty (property);
				Mapper l = null;
				if (j != null) {
					l = new Mapper (a, j, property);
					b.Add (l);
				}
				object[] m = property.GetCustomAttributes (typeof(PersistenceForeignKeyAttribute), true);
				if ((m == null || m.Length == 0) && g != null && g.ForeignKey != null)
					m = new object[] {
						g.ForeignKey.GetPersistenceForeignKey ()
					};
				foreach (object obj in m) {
					PersistenceForeignKeyAttribute n = (PersistenceForeignKeyAttribute)obj;
					GroupOfRelationshipInfo o = new GroupOfRelationshipInfo (a, n.TypeOfClassRelated, n.GroupOfRelationship);
					int p = c.FindIndex (delegate (GroupOfRelationshipInfo q) {
						return q == o;
					});
					if (p < 0) {
						c.Add (o);
					}
					else
						o = c [p];
					ForeignKeyMapper r = new ForeignKeyMapper (n, property);
					o.AddForeignKey (r);
					if (l != null)
						l.AddForeignKey (r);
				}
				m = property.GetCustomAttributes (typeof(ValidatorAttribute), true);
				if ((m == null || m.Length == 0) && g != null && g.Validators.Count > 0) {
					m = new object[g.Validators.Count];
					for (int s = 0; s < m.Length; s++)
						m [s] = g.Validators [s].GetValidator ();
				}
				if (m.Length > 0) {
					ValidatorAttribute[] t = new ValidatorAttribute[m.Length];
					for (int s = 0; s < m.Length; s++)
						t [s] = (ValidatorAttribute)m [s];
					l.Validation = new ValidationMapper (l, t);
				}
				m = property.GetCustomAttributes (typeof(PersistenceForeignMemberAttribute), true);
				if ((m == null || m.Length == 0) && g != null && g.ForeignMember != null)
					m = new object[] {
						g.ForeignMember.GetPersistenceForeignMember ()
					};
				if (m.Length > 0) {
					d.Add (new ForeignMemberMapper ((PersistenceForeignMemberAttribute)m [0], property));
				}
			}
			if (!a.IsInterface) {
				Type[] u = a.GetInterfaces ();
				PersistenceClassAttribute v;
				for (int s = 0; s < u.Length; s++) {
					v = GetPersistenceClassAttribute (u [s]);
					if (v != null) {
						LoadPersistencePropertyAttributes (u [s], b, c, d);
					}
				}
			}
		}
		internal static Type LoadModel (string a)
		{
			GDASettings.LoadConfiguration ();
			string b = null;
			foreach (KeyValuePair<string, Dictionary<string, string>> k in ModelsNamespaceLoaded) {
				if (k.Value.TryGetValue (a, out b))
					return Type.GetType (b);
			}
			foreach (ModelsNamespaceInfo ns in GDASettings.ModelsNamespaces) {
				string c = a;
				if (c.IndexOf (ns.Namespace) < 0)
					c = ns.Namespace + "." + a;
				var d = ns.CurrentAssembly;
				if (d == null)
					throw new GDAException ("Fail on load assembly \"{0}\" for model namespace.", ns.AssemblyName);
				Type e = ns.CurrentAssembly.GetType (c);
				if (e == null)
					e = ns.CurrentAssembly.GetType (a);
				if (e != null) {
					if (!ModelsNamespaceLoaded.ContainsKey (ns.Namespace))
						ModelsNamespaceLoaded.Add (ns.Namespace, new Dictionary<string, string> ());
					ModelsNamespaceLoaded [ns.Namespace].Add (a, e.FullName + ", " + e.Assembly.FullName);
					return e;
				}
			}
			Type f = null;
			#if PocketPC
						            if (Assembly.GetExecutingAssembly() != null)
                rt = Assembly.GetExecutingAssembly().GetType(modelTypeName);
#else
			var g = Assembly.GetEntryAssembly ();
			if (g != null)
				f = g.GetType (a);
			else {
				g = Assembly.GetCallingAssembly ();
				if (g != null)
					f = g.GetType (a);
				else {
					g = Assembly.GetExecutingAssembly ();
					if (g != null)
						f = g.GetType (a);
				}
			}
			#endif
			if (f != null) {
				GDASettings.AddModelsNamespace (f.Assembly, f.Namespace);
				if (!ModelsNamespaceLoaded.ContainsKey (f.Namespace))
					ModelsNamespaceLoaded.Add (f.Namespace, new Dictionary<string, string> ());
				ModelsNamespaceLoaded [f.Namespace].Add (a, f.FullName);
				return f;
			}
			return Type.GetType (a, false);
		}
		internal static GDA.Sql.TableName GetTableName (Type a)
		{
			var b = GetPersistenceClassAttribute (a);
			if (b == null || string.IsNullOrEmpty (b.Name)) {
				Type[] c = a.GetInterfaces ();
				PersistenceClassAttribute d;
				for (int e = 0; e < c.Length; e++) {
					d = MappingManager.GetPersistenceClassAttribute (c [e]);
					if (d != null && d.Name != null)
						return d.GetTableName ();
				}
				throw new GDATableNameRepresentNotExistsException ("The class: " + a.FullName + ", not found PersistenceClassAttribute");
			}
			return b.GetTableName ();
		}
		internal static IList<Mapper> GetMappers<Model> ()
		{
			var a = MappingManager.LoadPersistencePropertyAttributes (typeof(Model));
			return a.Mappers;
		}
		internal static IList<ForeignMemberMapper> GetForeignMemberMapper (Type a)
		{
			var b = MappingManager.LoadPersistencePropertyAttributes (a);
			return b.ForeignMembers;
		}
		internal static List<Mapper> GetMappers (Type a)
		{
			var b = MappingManager.LoadPersistencePropertyAttributes (a);
			return b.Mappers;
		}
		internal static List<Mapper> GetMappers<Model> (PersistenceParameterType[] a, DirectionParameter[] b)
		{
			return GetMappers<Model> (a, b, false);
		}
		internal static List<Mapper> GetMappers<Model> (PersistenceParameterType[] a, DirectionParameter[] b, bool c)
		{
			return GetMappers (typeof(Model), a, b, c);
		}
		internal static List<Mapper> GetMappers (Type a, PersistenceParameterType[] b, DirectionParameter[] c)
		{
			return GetMappers (a, b, c, false);
		}
		public static List<Mapper> GetMappers (Type a, PersistenceParameterType[] b, DirectionParameter[] c, bool d)
		{
			var e = MappingManager.LoadPersistencePropertyAttributes (a);
			List<Mapper> f = new List<Mapper> ();
			var g = e.Mappers;
			if (c == null)
				c = new DirectionParameter[] {
					DirectionParameter.Input,
					DirectionParameter.InputOptionalOutput,
					DirectionParameter.InputOutput,
					DirectionParameter.OutputOnlyInsert,
					DirectionParameter.InputOptional,
					DirectionParameter.InputOptionalOutput
				};
			bool h;
			if (g != null)
				foreach (Mapper mapper in g) {
					if (mapper == null)
						continue;
					if (b != null) {
						h = false;
						foreach (PersistenceParameterType ppt in b)
							if (ppt == mapper.ParameterType) {
								h = true;
								break;
							}
						if (!h)
							continue;
					}
					if (c != null) {
						h = false;
						foreach (DirectionParameter dp in c)
							if (dp == mapper.Direction) {
								h = true;
								break;
							}
						if (!h)
							continue;
					}
					f.Add (mapper);
					if (d)
						break;
				}
			return f;
		}
		internal static void LoadClassMapper (Type a)
		{
			LoadPersistencePropertyAttributes (a);
		}
		internal static List<GroupOfRelationshipInfo> GetForeignKeyAttributes (Type a)
		{
			var b = MappingManager.LoadPersistencePropertyAttributes (a);
			return (b.ForeignKeyMappers == null ? new List<GroupOfRelationshipInfo> () : b.ForeignKeyMappers);
		}
		internal static List<ForeignKeyMapper> LoadRelationships (Type a, GroupOfRelationshipInfo b)
		{
			List<GroupOfRelationshipInfo> c = MappingManager.GetForeignKeyAttributes (a);
			int d = c.FindIndex (delegate (GroupOfRelationshipInfo e) {
				return e == b;
			});
			if (d >= 0)
				return c [d].ForeignKeys;
			else
				return new List<ForeignKeyMapper> ();
		}
		internal static bool CheckExistsIdentityKey (Type a)
		{
			var b = MappingManager.LoadPersistencePropertyAttributes (a);
			List<Mapper> c = b.Mappers;
			if (c.FindIndex (delegate (Mapper d) {
				return d.ParameterType == PersistenceParameterType.IdentityKey;
			}) >= 0)
				return true;
			return false;
		}
		internal static Mapper GetIdentityKey (Type a)
		{
			var b = MappingManager.LoadPersistencePropertyAttributes (a);
			List<Mapper> c = b.Mappers;
			return c.Find (delegate (Mapper d) {
				return d.ParameterType == PersistenceParameterType.IdentityKey;
			});
		}
	}
}
