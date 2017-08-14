using System;
using System.Collections.Generic;
using System.Text;
using GDA.Collections;
namespace GDA.Sql
{
	public abstract class BaseQuery : IQuery
	{
		protected Type _returnTypeQuery;
		protected int _skipCount = 0;
		protected int _takeCount = 0;
		protected string _aggregationFunctionProperty;
		public string AggregationFunctionProperty {
			get {
				return _aggregationFunctionProperty;
			}
		}
		public Type ReturnTypeQuery {
			get {
				return _returnTypeQuery;
			}
			set {
				_returnTypeQuery = value;
			}
		}
		public int SkipCount {
			get {
				return _skipCount;
			}
		}
		public int TakeCount {
			get {
				return _takeCount;
			}
		}
		internal BaseQuery BaseSkip (int a)
		{
			_skipCount = a;
			return this;
		}
		internal BaseQuery BaseTake (int a)
		{
			_takeCount = a;
			return this;
		}
		public GDACursor<T> ToCursor<T> () where T : new()
		{
			return GDAOperations.GetDAO<T> ().Select (null, this);
		}
		public GDACursor<T> ToCursor<T> (GDASession a) where T : new()
		{
			return GDAOperations.GetDAO<T> ().Select (a, this);
		}
		public virtual IEnumerable<Result> ToCursor<T, Result> (GDASession session) where T : new() where Result : new()
		{
			return GDAOperations.GetDAO<T> ().SelectToDataRecord (session, this).Select<Result> ();
		}
		public virtual IEnumerable<Result> ToCursor<T, Result> () where T : new() where Result : new()
		{
			return ToCursor<T, Result> (null);
		}
		public GDADataRecordCursor<T> ToDataRecords<T> ()
		{
			return GDAOperations.GetSimpleDAO<T> ().SelectToDataRecord (null, this);
		}
		public GDADataRecordCursor<T> ToDataRecords<T> (GDASession a)
		{
			return GDAOperations.GetSimpleDAO<T> ().SelectToDataRecord (a, this);
		}
		public virtual GDAPropertyValue GetValue<T> (string a)
		{
			return GetValue<T> (null, a);
		}
		public virtual GDAPropertyValue GetValue<T> (GDASession a, string b)
		{
			foreach (var i in ToDataRecords<T> (a))
				return i [b];
			return new GDAPropertyValue (null, false);
		}
		public IEnumerable<GDAPropertyValue> GetValues<T> (string a)
		{
			return GetValues<T> (null, a);
		}
		public virtual IEnumerable<GDAPropertyValue> GetValues<T> (GDASession a, string b)
		{
			foreach (var i in ToDataRecords<T> (a))
				yield return i [b];
		}
		#if CLS_3_5
        /// <summary>
        /// Recupera o valor da propriedade.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertySelector">Nome da propriedade que será recuperada.</param>
        /// <returns></returns>
        public GDAPropertyValue GetValue<T>(System.Linq.Expressions.Expression<Func<T, object>> propertySelector)
        {
            return GetValue<T>(propertySelector.GetMember().Name);
        }
        /// <summary>
        /// Recupera o valor da propriedade.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session"></param>
        /// <param name="propertySelector">Nome da propriedade que será recuperada.</param>
        /// <returns></returns>
        public GDAPropertyValue GetValue<T>(GDASession session, System.Linq.Expressions.Expression<Func<T, object>> propertySelector)
        {
            return GetValue<T>(session, propertySelector.GetMember().Name);
        }
        /// <summary>
        /// Recupera os valores da propriedade.
        /// </summary>
        /// <typeparam name="T"></typeparam> 
        /// <param name="propertySelector">Propriedade que será recuperada.</param>
        /// <returns></returns>
        public IEnumerable<GDAPropertyValue> GetValues<T>(System.Linq.Expressions.Expression<Func<T, object>> propertySelector)
        {
            return GetValues<T>(null, propertySelector.GetMember().Name);
        }
        /// <summary>
        /// Recupera os valores da propriedade.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session"></param>
        /// <param name="propertySelector">Propriedade que será recuperada.</param>
        /// <returns></returns>
        public IEnumerable<GDAPropertyValue> GetValues<T>(GDASession session, System.Linq.Expressions.Expression<Func<T, object>> propertySelector)
        {
            var propertyName = propertySelector.GetMember().Name;
            foreach (var i in ToDataRecords<T>(session))
                yield return i[propertyName];
        }
#endif
		public GDAList<T> ToList<T> () where T : new()
		{
			return GDAOperations.GetDAO<T> ().Select (this);
		}
		public GDAList<T> ToList<T> (GDASession a) where T : new()
		{
			return GDAOperations.GetDAO<T> ().Select (a, this);
		}
		public T First<T> () where T : new()
		{
			return First<T> (null);
		}
		public T First<T> (GDASession a) where T : new()
		{
			foreach (var i in ToCursor<T> (a))
				return i;
			return default(T);
		}
		public virtual QueryReturnInfo BuildResultInfo<T> ()
		{
			return BuildResultInfo<T> (GDASettings.DefaultProviderConfiguration);
		}
		public abstract QueryReturnInfo BuildResultInfo<T> (GDA.Interfaces.IProviderConfiguration a);
		public abstract QueryReturnInfo BuildResultInfo (string a);
		public abstract QueryReturnInfo BuildResultInfo2 (GDA.Interfaces.IProvider a, string b);
		public abstract QueryReturnInfo BuildResultInfo2 (GDA.Interfaces.IProvider a, string b, Dictionary<string, Type> c);
	}
}
