using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
#if CLS_3_5
using System.Linq.Expressions;
#endif
namespace GDA
{
	public class GDADataRecord : IDataRecord
	{
		internal protected IDataRecord _baseRecord;
		private IDictionary<string, int> _translator;
		public IDataRecord BaseDataRecord {
			get {
				return _baseRecord;
			}
		}
		protected internal IDictionary<string, int> Translator {
			get {
				return _translator;
			}
		}
		public GDADataRecord (IDataRecord a, IDictionary<string, int> b)
		{
			_baseRecord = a;
			_translator = b;
		}
		protected int GetFieldPosition (string a)
		{
			var b = -1;
			if (Translator != null) {
				if (!Translator.TryGetValue (a, out b))
					b = -1;
			}
			else
				return _baseRecord.GetOrdinal (a);
			if (b < 0) {
				try {
					b = _baseRecord.GetOrdinal (a);
				}
				catch {
				}
				if (b < 0)
					throw new GDAException ("Field {0} not found in result", a);
			}
			return b;
		}
		protected int GetFieldRoutePosition (int a)
		{
			if (Translator != null) {
				if (a >= 0) {
					int b = 0;
					foreach (int v in Translator.Values) {
						if (b == a)
							return v;
						b++;
					}
				}
				throw new IndexOutOfRangeException ("The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.");
			}
			return a;
		}
		public int FieldCount {
			get {
				if (Translator != null)
					return Translator.Count;
				else
					return _baseRecord.FieldCount;
			}
		}
		public bool GetBoolean (int a)
		{
			return _baseRecord.GetBoolean (GetFieldRoutePosition (a));
		}
		public bool GetBoolean (string a)
		{
			return _baseRecord.GetBoolean (GetFieldPosition (a));
		}
		public byte GetByte (int a)
		{
			return _baseRecord.GetByte (GetFieldRoutePosition (a));
		}
		public byte GetByte (string a)
		{
			return _baseRecord.GetByte (GetFieldPosition (a));
		}
		public long GetBytes (int a, long b, byte[] c, int d, int e)
		{
			return _baseRecord.GetBytes (a, b, c, d, e);
		}
		public long GetBytes (string a, long b, byte[] c, int d, int e)
		{
			return _baseRecord.GetBytes (GetFieldPosition (a), b, c, d, e);
		}
		public char GetChar (int a)
		{
			return _baseRecord.GetChar (GetFieldRoutePosition (a));
		}
		public char GetChar (string a)
		{
			return _baseRecord.GetChar (GetFieldPosition (a));
		}
		public long GetChars (int a, long b, char[] c, int d, int e)
		{
			return _baseRecord.GetChars (a, b, c, d, e);
		}
		public long GetChars (string a, long b, char[] c, int d, int e)
		{
			return _baseRecord.GetChars (GetFieldPosition (a), b, c, d, e);
		}
		public IDataReader GetData (int a)
		{
			return _baseRecord.GetData (GetFieldRoutePosition (a));
		}
		public IDataReader GetData (string a)
		{
			return _baseRecord.GetData (GetFieldPosition (a));
		}
		public string GetDataTypeName (int a)
		{
			return _baseRecord.GetDataTypeName (GetFieldRoutePosition (a));
		}
		public string GetDataTypeName (string a)
		{
			return _baseRecord.GetDataTypeName (GetFieldPosition (a));
		}
		public DateTime GetDateTime (int a)
		{
			return _baseRecord.GetDateTime (GetFieldRoutePosition (a));
		}
		public DateTime GetDateTime (string a)
		{
			return _baseRecord.GetDateTime (GetFieldPosition (a));
		}
		public decimal GetDecimal (int a)
		{
			return _baseRecord.GetDecimal (GetFieldRoutePosition (a));
		}
		public decimal GetDecimal (string a)
		{
			return _baseRecord.GetDecimal (GetFieldPosition (a));
		}
		public double GetDouble (int a)
		{
			return _baseRecord.GetDouble (GetFieldRoutePosition (a));
		}
		public double GetDouble (string a)
		{
			return _baseRecord.GetDouble (GetFieldPosition (a));
		}
		public Type GetFieldType (int a)
		{
			return _baseRecord.GetFieldType (GetFieldRoutePosition (a));
		}
		public Type GetFieldType (string a)
		{
			return _baseRecord.GetFieldType (GetFieldPosition (a));
		}
		public float GetFloat (int a)
		{
			return _baseRecord.GetFloat (GetFieldRoutePosition (a));
		}
		public float GetFloat (string a)
		{
			return _baseRecord.GetFloat (GetFieldPosition (a));
		}
		public Guid GetGuid (int a)
		{
			return _baseRecord.GetGuid (GetFieldRoutePosition (a));
		}
		public Guid GetGuid (string a)
		{
			return _baseRecord.GetGuid (GetFieldPosition (a));
		}
		public short GetInt16 (int a)
		{
			return _baseRecord.GetInt16 (GetFieldRoutePosition (a));
		}
		public short GetInt16 (string a)
		{
			return _baseRecord.GetInt16 (GetFieldPosition (a));
		}
		public ushort GetUInt16 (int a)
		{
			return unchecked((ushort)GetInt16 (a));
		}
		public ushort GetUInt16 (string a)
		{
			return unchecked((ushort)GetInt16 (a));
		}
		public int GetInt32 (int a)
		{
			return _baseRecord.GetInt32 (GetFieldRoutePosition (a));
		}
		public int GetInt32 (string a)
		{
			return _baseRecord.GetInt32 (GetFieldPosition (a));
		}
		public uint GetUInt32 (int a)
		{
			return unchecked((uint)GetInt32 (a));
		}
		public uint GetUInt32 (string a)
		{
			return unchecked((uint)GetInt32 (a));
		}
		public long GetInt64 (int a)
		{
			return _baseRecord.GetInt64 (GetFieldRoutePosition (a));
		}
		public long GetInt64 (string a)
		{
			return _baseRecord.GetInt64 (GetFieldPosition (a));
		}
		public ulong GetUInt64 (int a)
		{
			return unchecked((ulong)GetInt64 (a));
		}
		public ulong GetUInt64 (string a)
		{
			return unchecked((ulong)GetInt64 (a));
		}
		public string GetName (int a)
		{
			return _baseRecord.GetName (GetFieldRoutePosition (a));
		}
		public int GetOrdinal (string a)
		{
			return _baseRecord.GetOrdinal (a);
		}
		public string GetString (int a)
		{
			var b = GetFieldRoutePosition (a);
			if (!_baseRecord.IsDBNull (b))
				return _baseRecord.GetString (b);
			return null;
		}
		public string GetString (string a)
		{
			var b = GetFieldPosition (a);
			if (!_baseRecord.IsDBNull (b))
				return _baseRecord.GetString (b);
			return null;
		}
		public object GetValue (int a)
		{
			return _baseRecord.GetValue (GetFieldRoutePosition (a));
		}
		public object GetValue (string a)
		{
			return _baseRecord.GetValue (GetFieldPosition (a));
		}
		public int GetValues (object[] a)
		{
			int b = _baseRecord.GetValues (a);
			for (int c = 0; c < b; c++)
				if (a [c] is DBNull)
					a [c] = null;
			return b;
		}
		public bool IsDBNull (int a)
		{
			return _baseRecord.IsDBNull (GetFieldRoutePosition (a));
		}
		public bool IsDBNull (string a)
		{
			return _baseRecord.IsDBNull (GetFieldPosition (a));
		}
		public GDAPropertyValue this [string a] {
			get {
				if (a == null)
					throw new ArgumentNullException ("name");
				try {
					var b = _baseRecord [GetFieldPosition (a)];
					return new GDAPropertyValue ((b is DBNull ? null : b), !(b is DBNull));
				}
				catch (IndexOutOfRangeException ex) {
					throw new GDAException (string.Format ("No column with the name \"{0}\" was found.", a), ex);
				}
			}
		}
		public GDAPropertyValue this [int a] {
			get {
				var b = _baseRecord [GetFieldRoutePosition (a)];
				return new GDAPropertyValue ((b is DBNull ? null : b), !(b is DBNull));
			}
		}
		object IDataRecord.this [string name] {
			get {
				if (name == null)
					throw new ArgumentNullException ("name");
				try {
					var obj = _baseRecord [GetFieldPosition (name)];
					return (obj is DBNull ? null : obj);
				}
				catch (IndexOutOfRangeException ex) {
					throw new GDAException (string.Format ("No column with the name \"{0}\" was found.", name), ex);
				}
			}
		}
		object IDataRecord.this [int i] {
			get {
				var obj = _baseRecord [GetFieldRoutePosition (i)];
				return (obj is DBNull ? null : obj);
			}
		}
	}
	public class GDADataRecord<Model> : GDADataRecord
	{
		internal GDADataRecord (IDataRecord a, TranslatorDataInfoCollection b) : base (a, b)
		{
		}
		#if CLS_3_5
				        /// <summary>
        /// Recupera o valor da propriedade.
        /// </summary>
        /// <typeparam name="T">Tipo da propriedade.</typeparam>
        /// <param name="propertySelector"></param>
        /// <returns></returns>
        public T GetValue<T>(Expression<Func<Model, T>> propertySelector)
        {
            var property = propertySelector.GetMember();
            if (property == null)
                throw new ArgumentException("Not found property name.");
            return (T)this[property.Name].GetValue();
        }
#endif
		public virtual Model GetInstance ()
		{
			throw new NotSupportedException ();
		}
		public virtual T Fill<T> (T a)
		{
			object b = a;
			DataAccess.RecoverValueOfResult (ref _baseRecord, (TranslatorDataInfoCollection)Translator, ref b, a is Interfaces.IObjectDataRecord);
			return a;
		}
		public virtual T Fill<T> () where T : new()
		{
			T a = new T ();
			object b = a;
			DataAccess.RecoverValueOfResult (ref _baseRecord, (TranslatorDataInfoCollection)Translator, ref b, a is Interfaces.IObjectDataRecord);
			return a;
		}
	#if CLS_3_5
        /// <summary>
        /// Recupera o valor da propriedade.
        /// </summary>
        /// <param name="propertySelector"></param>
        /// <returns></returns>
        public GDAPropertyValue this[Expression<Func<Model, object>> propertySelector]
        {
            get
            {
                var property = propertySelector.GetMember();
                if (property == null)
                    throw new ArgumentException("Not found property name.");
                return this[property.Name];
            }
        }
#endif
	}
	public class GDADataRecordEx<Model> : GDADataRecord<Model> where Model : new()
	{
		internal GDADataRecordEx (IDataRecord a, TranslatorDataInfoCollection b) : base (a, b)
		{
		}
		public override Model GetInstance ()
		{
			var a = new Model ();
			PersistenceObjectBase<Model>.RecoverValueOfResult (ref _baseRecord, (TranslatorDataInfoCollection)Translator, ref a, true);
			return a;
		}
		public override T Fill<T> (T a)
		{
			object b = a;
			DataAccess.RecoverValueOfResult (ref _baseRecord, (TranslatorDataInfoCollection)Translator, ref b, a is Interfaces.IObjectDataRecord);
			return a;
		}
		public override T Fill<T> ()
		{
			T a = new T ();
			object b = a;
			DataAccess.RecoverValueOfResult (ref _baseRecord, (TranslatorDataInfoCollection)Translator, ref b, a is Interfaces.IObjectDataRecord);
			return a;
		}
	}
}
