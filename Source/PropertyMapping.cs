using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
namespace GDA.Mapping
{
	public interface IPropertyMappingInfo
	{
		string Name {
			get;
			set;
		}
		string Column {
			get;
			set;
		}
		DirectionParameter Direction {
			get;
			set;
		}
		PersistenceParameterType ParameterType {
			get;
			set;
		}
	}
	public class PropertyMappingInfo : IPropertyMappingInfo
	{
		public string Name {
			get;
			set;
		}
		public string Column {
			get;
			set;
		}
		public DirectionParameter Direction {
			get;
			set;
		}
		public PersistenceParameterType ParameterType {
			get;
			set;
		}
	}
	public class PropertyMapping : ElementMapping, IPropertyMappingInfo
	{
		public string Name {
			get;
			set;
		}
		public string Column {
			get;
			set;
		}
		public PersistenceParameterType ParameterType {
			get;
			set;
		}
		public int Size {
			get;
			set;
		}
		public DirectionParameter Direction {
			get;
			set;
		}
		public ForeignKeyMapping ForeignKey {
			get;
			set;
		}
		public ForeignMemberMapping ForeignMember {
			get;
			set;
		}
		public List<ValidatorMapping> Validators {
			get;
			set;
		}
		public string GeneratorKeyName {
			get;
			set;
		}
		public bool IsNotPersists {
			get;
			set;
		}
		public bool IsNotNull {
			get;
			set;
		}
		public PropertyMapping (XmlElement a, string b, string c)
		{
			Name = GetAttributeString (a, "name", true);
			Column = GetAttributeString (a, "column", Name);
			var d = GetAttributeString (a, "parameterType", "Field");
			ParameterType = (PersistenceParameterType)Enum.Parse (typeof(PersistenceParameterType), d, true);
			d = GetAttributeString (a, "size");
			var e = 0;
			if (Helper.GDAHelper.TryParse (d, out e))
				Size = e;
			bool f = false;
			if (Helper.GDAHelper.TryParse (GetAttributeString (a, "not-persists"), out f))
				IsNotPersists = f;
			if (Helper.GDAHelper.TryParse (GetAttributeString (a, "not-null"), out f))
				IsNotNull = f;
			d = GetAttributeString (a, "direction", "InputOutput");
			Direction = (DirectionParameter)Enum.Parse (typeof(DirectionParameter), d, true);
			var g = FirstOrDefault<XmlElement> (a.GetElementsByTagName ("generator"));
			if (g != null)
				GeneratorKeyName = GetAttributeString (g, "name", true);
			var h = FirstOrDefault<XmlElement> (a.GetElementsByTagName ("foreignKey"));
			if (h != null)
				ForeignKey = new ForeignKeyMapping (h, b, c);
			var j = FirstOrDefault<XmlElement> (a.GetElementsByTagName ("foreignMember"));
			if (j != null)
				ForeignMember = new ForeignMemberMapping (j, b, c);
			Validators = new List<ValidatorMapping> ();
			foreach (XmlElement i in a.GetElementsByTagName ("validator")) {
				var k = new ValidatorMapping (i);
				if (!Validators.Exists (l => l.Name == k.Name))
					Validators.Add (k);
			}
		}
		public PropertyMapping (string a, string b, PersistenceParameterType c, int d, bool e, bool f, DirectionParameter g, string h, ForeignKeyMapping j, ForeignMemberMapping k, IEnumerable<ValidatorMapping> l)
		{
			if (string.IsNullOrEmpty (a))
				throw new ArgumentNullException ("name");
			this.Name = a;
			this.Column = string.IsNullOrEmpty (b) ? a : b;
			this.ParameterType = c;
			this.Size = d;
			this.IsNotPersists = e;
			this.IsNotNull = f;
			this.Direction = g;
			this.GeneratorKeyName = h;
			this.ForeignKey = j;
			this.ForeignMember = k;
			Validators = new List<ValidatorMapping> ();
			if (l != null) {
				foreach (var i in l)
					if (!Validators.Exists (m => m.Name == i.Name))
						Validators.Add (i);
			}
		}
		public PersistencePropertyAttribute GetPersistenceProperty ()
		{
			if (IsNotPersists)
				return null;
			return new PersistencePropertyAttribute {
				Name = Column,
				PropertyName = Name,
				Size = Size,
				ParameterType = ParameterType,
				Direction = Direction,
				GeneratorKeyName = GeneratorKeyName,
				IsNotNull = IsNotNull
			};
		}
	#if CLS_3_5
        /// <summary>
        /// Constrói um mapeamento com base no tipo.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertySelector"></param>
        /// <param name="column">Nome da coluna relacionada com a propriedade.</param>
        /// <returns></returns>
        public static PropertyMapping Build<T>(System.Linq.Expressions.Expression<Func<T, object>> propertySelector, string column)
        {
            if (propertySelector == null)
                throw new ArgumentNullException("propertySelector");
            return new PropertyMapping(GDA.Extensions.GetMember(propertySelector).Name, column, PersistenceParameterType.Field, 0, false, false, DirectionParameter.InputOutput, null, null, null, null);
        }
#endif
	}
}
