using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Collections;
using GDA.Collections;
using GDA.Interfaces;
using GDA.Sql;
using GDA.Caching;
namespace GDA
{
	public static class GDAOperations
	{
		internal static GenerateKeyHandler GlobalGenerateKey;
		internal static ProviderConfigurationLoadHandler GlobalProviderConfigurationLoad;
		public static event DebugTraceDelegate DebugTrace;
		internal static void AddMemberDAO (Type a, ISimpleBaseDAO b)
		{
			try {
				MappingManager.MembersDAO [a] = b;
			}
			catch (NullReferenceException) {
			}
		}
		internal static void CallDebugTrace (object a, string b)
		{
			if (!GDASettings.EnabledDebugTrace)
				return;
			#if PocketPC
						            if (DebugTrace != null)
                DebugTrace(sender, message);
#else
			Helper.ThreadSafeEvents.FireEvent<string> (DebugTrace, a, b);
			#endif
		}
		public static void SetGlobalGenerateKeyHandler (GenerateKeyHandler a)
		{
			GlobalGenerateKey = a;
		}
		public static void SetGlobalProviderConfigurationLoadHandler (ProviderConfigurationLoadHandler a)
		{
			GlobalProviderConfigurationLoad = a;
		}
		public static List<PropertyInfo> GetPropertiesKey<Model> ()
		{
			var a = new List<PropertyInfo> ();
			foreach (var i in MappingManager.GetMappers<Model> (new PersistenceParameterType[] {
				PersistenceParameterType.Key,
				PersistenceParameterType.IdentityKey
			}, null))
				a.Add (i.PropertyMapper);
			return a;
		}
		public static List<PropertyInfo> GetPropertiesKey (Type a)
		{
			var b = new List<PropertyInfo> ();
			foreach (var i in MappingManager.GetMappers (a, new PersistenceParameterType[] {
				PersistenceParameterType.Key,
				PersistenceParameterType.IdentityKey
			}, null))
				b.Add (i.PropertyMapper);
			return b;
		}
		public static GDA.Sql.TableName GetTableNameInfo<Model> ()
		{
			return MappingManager.GetTableName (typeof(Model));
		}
		[Obsolete ("Use GetTableNameInfo<Model>")]
		public static string GetTableName<Model> ()
		{
			var a = MappingManager.GetTableName (typeof(Model));
			return a == null ? null : a.Name;
		}
		public static GDA.Sql.TableName GetTableNameInfo (Type a)
		{
			if (a == null)
				throw new ArgumentNullException ("type");
			return MappingManager.GetTableName (a);
		}
		[Obsolete ("Use GetTableNameInfo<Model>")]
		public static string GetTableName (Type a)
		{
			if (a == null)
				throw new ArgumentNullException ("type");
			var b = MappingManager.GetTableName (a);
			return b == null ? null : b.Name;
		}
		public static uint Insert (object a, string b, DirectionPropertiesName c)
		{
			return Insert (null, a, b, c);
		}
		public static uint Insert (GDASession a, object b, string c, DirectionPropertiesName d)
		{
			return GetDAO (b).Insert (a, b, c, d);
		}
		public static uint Insert (GDASession a, object b, string c)
		{
			return GetDAO (b).Insert (a, b, c);
		}
		public static uint Insert (object a, string b)
		{
			return GetDAO (a).Insert (a, b);
		}
		public static uint Insert (GDASession a, object b)
		{
			return GetDAO (b).Insert (a, b);
		}
		public static uint Insert (object a)
		{
			return GetDAO (a).Insert (a);
		}
		public static int Update (GDASession a, object b, string c, DirectionPropertiesName d)
		{
			return GetDAO (b).Update (a, b, c, d);
		}
		public static int Update (object a, string b, DirectionPropertiesName c)
		{
			return GetDAO (a).Update (a, b, c);
		}
		#if CLS_3_5
        /// <summary>
        /// Recupera o seletor de propriedades que pode ser usado para realizar opera��es de inser��o ou atualiza��o.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model">Instancia da model model que ser� usada.</param>
        /// <returns></returns>
        public static GDAPropertySelector<T> PropertySelector<T>(T model)
        {
            if (model == null)
                throw new ArgumentNullException("model");
            return new GDAPropertySelector<T>(model);
        }
        /// <summary>
        /// Recupera o seletor de propriedades que pode ser usado para realizar opera��es de inser��o ou atualiza��o.
        /// </summary>
        /// <typeparam name="T"></typeparam>        
        /// <param name="model">Instancia da model model que ser� usada.</param>
        /// <param name="propertiesSelector">Propriedades que ser�o selecionadas inicialmente.</param>
        /// <returns></returns>
        public static GDAPropertySelector<T> PropertySelector<T>(T model, params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector)
        {
            if (model == null)
                throw new ArgumentNullException("model");
            return new GDAPropertySelector<T>(model).Add(propertiesSelector);
        }
#endif
		public static int Update (GDASession a, object b, string c)
		{
			return GetDAO (b).Update (a, b, c);
		}
		public static int Update (object a, string b)
		{
			return GetDAO (a).Update (a, b);
		}
		public static int Update (GDASession a, object b)
		{
			return GetDAO (b).Update (a, b);
		}
		public static int Update (object a)
		{
			return GetDAO (a).Update (a);
		}
		public static int Delete (GDASession a, object b)
		{
			return GetDAO (b).Delete (a, b);
		}
		public static int Delete (object a)
		{
			return GetDAO (a).Delete (a);
		}
		public static uint Save (GDASession a, object b)
		{
			return GetDAO (b).InsertOrUpdate (a, b);
		}
		public static uint Save (object a)
		{
			return GetDAO (a).InsertOrUpdate (a);
		}
		public static void RecoverData (GDASession a, object b)
		{
			object c = GetDAO (b);
			MethodInfo d = c.GetType ().GetMethod ("RecoverData", new Type[] {
				typeof(GDASession),
				b.GetType ()
			});
			if (d == null)
				throw new GDAException ("Method RecoverData not found in DAO.");
			try {
				d.Invoke (c, new object[] {
					a,
					b
				});
			}
			catch (TargetInvocationException ex) {
				if (ex.InnerException != null)
					throw ex.InnerException;
				else
					throw ex;
			}
		}
		public static void RecoverData (object a)
		{
			RecoverData (null, a);
		}
		#if CLS_3_5
        /// <summary>
        /// Recupera no nome da campo da BD que a propriedade representa.
        /// </summary>
        /// <typeparam name="Model">Tipo da class que contem a propriedade.</typeparam>
        /// <param name="propertyName">Nome da propriedade.</param>
        /// <returns>Nome do campo da BD.</returns>
        public static string GetPropertyDBFieldName<Model>(System.Linq.Expressions.Expression<Func<Model, object>> propertySelector)
        {
             var property = propertySelector.GetMember() as System.Reflection.PropertyInfo;            
             return GetPropertyDBFieldName<Model>(property.Name);
        }
#endif
		public static string GetPropertyDBFieldName<Model> (string a)
		{
			Type b = typeof(Model);
			PropertyInfo c = b.GetProperty (a);
			if (c == null)
				throw new GDAException ("Property {0} not found in {1}", a, b.FullName);
			PersistencePropertyAttribute d = MappingManager.GetPersistenceProperty (c);
			if (d == null)
				throw new GDAException ("DBFieldName not found in Property {0}.", a);
			return d.Name;
		}
		public static DAO GetDAO<Model, DAO> () where Model : new() where DAO : IBaseDAO<Model>
		{
			return (DAO)GetDAO<Model> ();
		}
		public static IBaseDAO<T> GetDAO<T> () where T : new()
		{
			Type persistenceType = typeof(T);
			if (MappingManager.MembersDAO.ContainsKey (persistenceType)) {
				return (IBaseDAO<T>)MappingManager.MembersDAO [persistenceType];
			}
			else {
				PersistenceBaseDAOAttribute info = MappingManager.GetPersistenceBaseDAOAttribute (persistenceType);
				if (info != null) {
					IBaseDAO<T> dao;
					try {
						if (info.BaseDAOType.IsGenericType) {
							Type t = info.BaseDAOType.MakeGenericType (info.BaseDAOGenericTypes);
							dao = (IBaseDAO<T>)Activator.CreateInstance (t);
						}
						else
							dao = (IBaseDAO<T>)Activator.CreateInstance (info.BaseDAOType);
					}
					catch (InvalidCastException) {
						throw new GDAException (String.Format ("Invalid cast, type {0} not inherit interface ISimpleBaseDAO.", info.BaseDAOType.FullName));
						;
					}
					catch (Exception ex) {
						if (ex is TargetInvocationException)
							throw new GDAException (ex.InnerException);
						else
							throw new GDAException (ex);
					}
					return dao;
				}
				else {
					try {
						return new BaseDAO<T> ();
					}
					catch (Exception ex) {
						throw new GDAException ("Error to create instance BaseDAO<> for type " + persistenceType.FullName + ".\r\n" + ex.Message, ex);
					}
				}
			}
		}
		public static ISimpleBaseDAO<T> GetSimpleDAO<T> ()
		{
			Type a = typeof(T);
			if (MappingManager.MembersDAO.ContainsKey (a)) {
				return (ISimpleBaseDAO<T>)MappingManager.MembersDAO [a];
			}
			else {
				PersistenceBaseDAOAttribute b = MappingManager.GetPersistenceBaseDAOAttribute (a);
				if (b != null) {
					ISimpleBaseDAO<T> c;
					try {
						if (b.BaseDAOType.IsGenericType) {
							Type d = b.BaseDAOType.MakeGenericType (b.BaseDAOGenericTypes);
							c = (ISimpleBaseDAO<T>)Activator.CreateInstance (d);
						}
						else
							c = (ISimpleBaseDAO<T>)Activator.CreateInstance (b.BaseDAOType);
					}
					catch (InvalidCastException) {
						throw new GDAException (String.Format ("Invalid cast, type {0} not inherit interface ISimpleBaseDAO.", b.BaseDAOType.FullName));
						;
					}
					catch (Exception ex) {
						if (ex is TargetInvocationException)
							throw new GDAException (ex.InnerException);
						else
							throw new GDAException (ex);
					}
					return c;
				}
				else {
					try {
						return new SimpleBaseDAO<T> ();
					}
					catch (Exception ex) {
						throw new GDAException ("Error to create instance SimpleBaseDAO<> for type " + a.FullName + ".\r\n" + ex.Message, ex);
					}
				}
			}
		}
		public static ISimpleBaseDAO GetDAO (object a)
		{
			if (a == null)
				throw new ArgumentNullException ("model");
			return GetDAO (a.GetType ());
		}
		public static ISimpleBaseDAO GetDAO (Type a)
		{
			if (MappingManager.MembersDAO.ContainsKey (a)) {
				return MappingManager.MembersDAO [a];
			}
			else {
				PersistenceBaseDAOAttribute b = MappingManager.GetPersistenceBaseDAOAttribute (a);
				if (b != null) {
					ISimpleBaseDAO c;
					try {
						if (b.BaseDAOType.IsGenericType) {
							Type d = b.BaseDAOType.MakeGenericType (b.BaseDAOGenericTypes);
							c = (ISimpleBaseDAO)Activator.CreateInstance (d);
						}
						else
							c = (ISimpleBaseDAO)Activator.CreateInstance (b.BaseDAOType);
					}
					catch (InvalidCastException) {
						throw new GDAException (String.Format ("Invalid cast, type {0} not inherit interface ISimpleBaseDAO.", b.BaseDAOType.FullName));
						;
					}
					catch (Exception ex) {
						if (ex is TargetInvocationException)
							throw new GDAException (ex.InnerException);
						else
							throw new GDAException (ex);
					}
					return c;
				}
				else {
					try {
						return (ISimpleBaseDAO)Activator.CreateInstance (typeof(BaseDAO<object>).GetGenericTypeDefinition ().MakeGenericType (a));
					}
					catch (Exception ex) {
						throw new GDAException ("Error to create instance BaseDAO<> for type " + a.FullName + ".\r\n" + ex.Message, ex);
					}
				}
			}
		}
		public static GDAList<ClassRelated> LoadRelationship1toN<ClassRelated> (object a, string b, InfoSortExpression c, InfoPaging d) where ClassRelated : new()
		{
			object e = GetDAO (a);
			MethodInfo f = e.GetType ().GetMethod ("LoadDataForeignKeyParentToChild", new Type[] {
				a.GetType (),
				typeof(string),
				typeof(InfoSortExpression),
				typeof(InfoPaging)
			});
			if (f == null)
				throw new GDAException ("DAO of model not suport LoadDataForeignKeyParentToChild.");
			else
				f = f.MakeGenericMethod (new Type[] {
					typeof(ClassRelated)
				});
			try {
				return (GDAList<ClassRelated>)f.Invoke (e, new object[] {
					a,
					b,
					c,
					d
				});
			}
			catch (Exception ex) {
				throw ex.InnerException;
			}
		}
		public static GDAList<ClassRelated> LoadRelationship1toN<ClassRelated> (object a, InfoSortExpression b, InfoPaging c) where ClassRelated : new()
		{
			return LoadRelationship1toN<ClassRelated> (a, null, b, c);
		}
		public static GDAList<ClassRelated> LoadRelationship1toN<ClassRelated> (object a, string b, InfoSortExpression c) where ClassRelated : new()
		{
			return LoadRelationship1toN<ClassRelated> (a, b, c, null);
		}
		public static GDAList<ClassRelated> LoadRelationship1toN<ClassRelated> (object a, InfoSortExpression b) where ClassRelated : new()
		{
			return LoadRelationship1toN<ClassRelated> (a, null, b, null);
		}
		public static GDAList<ClassRelated> LoadRelationship1toN<ClassRelated> (object a, string b, InfoPaging c) where ClassRelated : new()
		{
			return LoadRelationship1toN<ClassRelated> (a, b, null, c);
		}
		public static GDAList<ClassRelated> LoadRelationship1toN<ClassRelated> (object a, InfoPaging b) where ClassRelated : new()
		{
			return LoadRelationship1toN<ClassRelated> (a, null, null, b);
		}
		public static GDAList<ClassRelated> LoadRelationship1toN<ClassRelated> (object a, string b) where ClassRelated : new()
		{
			return LoadRelationship1toN<ClassRelated> (a, b, null, null);
		}
		public static GDAList<ClassRelated> LoadRelationship1toN<ClassRelated> (object a) where ClassRelated : new()
		{
			return LoadRelationship1toN<ClassRelated> (a, (string)null);
		}
		public static ClassRelated LoadRelationship1to1<ClassRelated> (object a, string b) where ClassRelated : new()
		{
			GDAList<ClassRelated> c = LoadRelationship1toN<ClassRelated> (a, b);
			if (c.Count > 1)
				throw new GDAException ("There is more one row found for this relationship.");
			else if (c.Count == 1)
				return c [0];
			else
				return default(ClassRelated);
		}
		public static ClassRelated LoadRelationship1to1<ClassRelated> (object a) where ClassRelated : new()
		{
			return LoadRelationship1to1<ClassRelated> (a, null);
		}
		public static int CountRowRelationship1toN<ClassRelated> (object a, string b) where ClassRelated : new()
		{
			object c = GetDAO (a);
			MethodInfo d = c.GetType ().GetMethod ("CountRowForeignKeyParentToChild", new Type[] {
				a.GetType (),
				typeof(string)
			});
			if (d == null)
				throw new GDAException ("DAO of model not suport CountRowForeignKeyParentToChild.");
			else
				d = d.MakeGenericMethod (new Type[] {
					typeof(ClassRelated)
				});
			try {
				return (int)d.Invoke (c, new object[] {
					a,
					b
				});
			}
			catch (Exception ex) {
				throw ex.InnerException;
			}
		}
		public static int CountRowRelationship1toN<ClassRelated> (object a) where ClassRelated : new()
		{
			return CountRowRelationship1toN<ClassRelated> (a, null);
		}
		public static GDACursor<Model> SelectToCursor<Model> (GDASession a) where Model : new()
		{
			return GetDAO<Model> ().Select (a);
		}
		public static GDACursor<Model> SelectToCursor<Model> () where Model : new()
		{
			return GetDAO<Model> ().Select ();
		}
		public static GDAList<Model> Select<Model> (GDASession a) where Model : new()
		{
			return GetDAO<Model> ().Select (a);
		}
		public static GDAList<Model> Select<Model> () where Model : new()
		{
			return GetDAO<Model> ().Select ();
		}
		public static long Count<Model> (GDASession a) where Model : new()
		{
			return GetDAO<Model> ().Count (a);
		}
		public static long Count<Model> () where Model : new()
		{
			return GetDAO<Model> ().Count ();
		}
		public static long Count (GDASession a, IQuery b)
		{
			if (a != null)
				return new DataAccess (a.ProviderConfiguration).Count (a, b);
			else
				return new DataAccess ().Count (b);
		}
		public static bool CheckExist (GDASession a, ValidationMode b, string c, object d, object e)
		{
			return GetDAO (e).CheckExist (a, b, c, d, e);
		}
		public static bool CheckExist (ValidationMode a, string b, object c, object d)
		{
			return GetDAO (d).CheckExist (null, a, b, c, d);
		}
	}
}
