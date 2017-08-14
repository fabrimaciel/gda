using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
namespace GDA
{
	public class TranslatorDataInfoCollection : List<TranslatorDataInfo>, IDictionary<string, int>
	{
		private Dictionary<string, int> _translator;
		public Dictionary<string, int> Translator {
			get {
				if (_translator == null)
					throw new GDAException ("Translator fields not processed.");
				return _translator;
			}
		}
		public TranslatorDataInfoCollection () : base ()
		{
		}
		public TranslatorDataInfoCollection (IEnumerable<TranslatorDataInfo> a) : base (a)
		{
		}
		public TranslatorDataInfoCollection (IEnumerable<Mapper> a) : base ()
		{
			if (a != null)
				foreach (var i in a)
					if (!string.IsNullOrEmpty (i.Name))
						this.Add (new TranslatorDataInfo (i.Name, i.PropertyMapper));
		}
		public void ProcessFieldsPositions (System.Data.IDataRecord a)
		{
			if (a == null)
				throw new ArgumentNullException ("dataRecord", "Datarecord couldn't be null");
			lock (this) {
				List<string> b = new List<string> ();
				for (int c = 0; c < a.FieldCount; c++) {
					var d = a.GetName (c);
					if (d != null)
						b.Add (d.ToLower ());
				}
				var e = new Dictionary<string, int> ();
				foreach (TranslatorDataInfo i in this) {
					var f = b.FindIndex (g => string.Compare (i.FieldName, g, true, System.Globalization.CultureInfo.InvariantCulture) == 0);
					if (f >= 0) {
						try {
							e.Add (i.PathAddress, f);
						}
						catch (System.ArgumentException ex) {
							throw new GDAException (string.Format ("PathAddress '{0}' with field name '{1}' duplicated.", i.PathAddress, i.FieldName), ex);
						}
					}
					i.FieldPosition = f;
				}
				_translator = e;
			}
		}
		void IDictionary<string, int>.Add (string a, int b)
		{
			throw new NotSupportedException ();
		}
		bool IDictionary<string, int>.ContainsKey (string a)
		{
			return Translator.ContainsKey (a);
		}
		ICollection<string> IDictionary<string, int>.Keys {
			get {
				return Translator.Keys;
			}
		}
		bool IDictionary<string, int>.Remove (string a)
		{
			throw new NotSupportedException ();
		}
		bool IDictionary<string, int>.TryGetValue (string a, out int b)
		{
			return Translator.TryGetValue (a, out b);
		}
		ICollection<int> IDictionary<string, int>.Values {
			get {
				return Translator.Values;
			}
		}
		int IDictionary<string, int>.this [string a] {
			get {
				return Translator [a];
			}
			set {
				throw new NotSupportedException ();
			}
		}
		void ICollection<KeyValuePair<string, int>>.Add (KeyValuePair<string, int> a)
		{
			throw new NotSupportedException ();
		}
		void ICollection<KeyValuePair<string, int>>.Clear ()
		{
			throw new NotSupportedException ();
		}
		bool ICollection<KeyValuePair<string, int>>.Contains (KeyValuePair<string, int> a)
		{
			return ((ICollection<KeyValuePair<string, int>>)Translator).Contains (a);
		}
		void ICollection<KeyValuePair<string, int>>.CopyTo (KeyValuePair<string, int>[] a, int b)
		{
			((ICollection<KeyValuePair<string, int>>)Translator).CopyTo (a, b);
		}
		int ICollection<KeyValuePair<string, int>>.Count {
			get {
				return ((ICollection<KeyValuePair<string, int>>)Translator).Count;
			}
		}
		bool ICollection<KeyValuePair<string, int>>.IsReadOnly {
			get {
				return true;
			}
		}
		bool ICollection<KeyValuePair<string, int>>.Remove (KeyValuePair<string, int> a)
		{
			throw new NotSupportedException ();
		}
		IEnumerator<KeyValuePair<string, int>> IEnumerable<KeyValuePair<string, int>>.GetEnumerator ()
		{
			return Translator.GetEnumerator ();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return Translator.GetEnumerator ();
		}
	}
	public class TranslatorDataInfo
	{
		private PropertyInfo[] _path;
		private string _pathAddress;
		public IEnumerable<PropertyInfo> Path {
			get {
				return _path;
			}
		}
		public int PathLength {
			get {
				return _path.Length;
			}
		}
		public string PathAddress {
			get {
				return _pathAddress;
			}
		}
		public PropertyInfo Property {
			get {
				return _path [_path.Length - 1];
			}
		}
		public string FieldName {
			get;
			set;
		}
		public int FieldPosition {
			get;
			set;
		}
		public void SetValue (object a, object b)
		{
			var c = 0;
			PropertyInfo d = null;
			foreach (var pi in Path) {
				c++;
				if (c == PathLength) {
					b = DataAccess.ConvertValue (b, pi.PropertyType);
					pi.SetValue (a, b, null);
					break;
				}
				if (a == null && d != null) {
					var e = Activator.CreateInstance (pi.DeclaringType);
					d.SetValue (a, e, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, null, System.Globalization.CultureInfo.CurrentCulture);
					a = e;
				}
				d = pi;
			}
		}
		public TranslatorDataInfo (string a, params PropertyInfo[] b)
		{
			if (a == null)
				throw new ArgumentNullException ("fieldName");
			FieldName = a;
			_path = b;
			if (b != null) {
				var c = new List<string> ();
				foreach (var i in b)
					c.Add (i.Name);
				_pathAddress = string.Join (".", c.ToArray ());
			}
		}
	}
}
