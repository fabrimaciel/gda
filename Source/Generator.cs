using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.CodeDom;
using System.CodeDom.Compiler;
namespace GDA.Analysis
{
	public class Generator
	{
		private string _namespaceName;
		private string _codeLanguage = "C#";
		private List<CodeAttributeDeclaration> _propertyCustomAttributes = new List<CodeAttributeDeclaration> ();
		private List<CodeAttributeDeclaration> _classCustomAttributes = new List<CodeAttributeDeclaration> ();
		private List<CodeNamespaceImport> _imports = new List<CodeNamespaceImport> ();
		private bool _usingWCFPattern;
		public string NamespaceName {
			get {
				return _namespaceName;
			}
			set {
				_namespaceName = value;
			}
		}
		public string CodeLanguage {
			get {
				return _codeLanguage;
			}
			set {
				_codeLanguage = value;
			}
		}
		public List<CodeAttributeDeclaration> PropertyCustomAttributes {
			get {
				return _propertyCustomAttributes;
			}
		}
		public List<CodeAttributeDeclaration> ClassCustomAttributes {
			get {
				return _classCustomAttributes;
			}
		}
		public List<CodeNamespaceImport> Imports {
			get {
				return _imports;
			}
		}
		public bool UsingWCFPattern {
			get {
				return _usingWCFPattern;
			}
			set {
				_usingWCFPattern = value;
			}
		}
		public static string StandartName (string a, bool b)
		{
			bool c = false;
			bool d = false;
			string e = "";
			for (int f = 0; f < a.Length; f++) {
				if (!c && ((a [f] >= 'a' && a [f] <= 'z') || (a [f] >= 'A' && a [f] <= 'Z'))) {
					e += char.ToUpper (a [f]);
					c = true;
				}
				else if (d && ((a [f] >= 'a' && a [f] <= 'z') || (a [f] >= 'A' && a [f] <= 'Z') || (a [f] >= '0' && a [f] <= '9'))) {
					e += char.ToUpper (a [f]);
					d = false;
				}
				else if (!((a [f] >= 'a' && a [f] <= 'z') || (a [f] >= 'A' && a [f] <= 'Z') || (a [f] >= '0' && a [f] <= '9'))) {
					if (!b)
						e += a [f];
					d = true;
				}
				else if (f > 0 && (a [f - 1] >= 'a' && a [f - 1] <= 'z') && (a [f] >= 'A' && a [f] <= 'Z'))
					e += a [f];
				else
					e += char.ToLower (a [f]);
			}
			return e;
		}
		public void Generate (TableMap a, string b, string c)
		{
			using (FileStream d = new FileStream (c, FileMode.Create, FileAccess.Write)) {
				Generate (a, b, d);
			}
		}
		public void Generate (TableMap a, string b, Stream c)
		{
			if (a == null)
				throw new ArgumentNullException ("map");
			CodeCompileUnit d = new CodeCompileUnit ();
			d.StartDirectives.Clear ();
			CodeNamespace e = new CodeNamespace (_namespaceName);
			e.Imports.Add (new CodeNamespaceImport ("System"));
			e.Imports.Add (new CodeNamespaceImport ("GDA"));
			if (UsingWCFPattern)
				e.Imports.Add (new CodeNamespaceImport ("System.Runtime.Serialization"));
			foreach (CodeNamespaceImport cni in _imports)
				e.Imports.Add (cni);
			d.Namespaces.Add (e);
			CodeTypeDeclaration f = new CodeTypeDeclaration (StandartName (a.TableName, true));
			f.Comments.Add (new CodeCommentStatement ("<summary>", true));
			f.Comments.Add (new CodeCommentStatement (string.Format ("This class represent the {0} {1}.", (a.IsView ? "view" : "table"), a.TableName), true));
			if (!string.IsNullOrEmpty (a.Comment))
				f.Comments.Add (new CodeCommentStatement (a.Comment, true));
			f.Comments.Add (new CodeCommentStatement ("</summary>", true));
			if (!string.IsNullOrEmpty (b))
				f.BaseTypes.Add (new CodeTypeReference (b));
			f.CustomAttributes.Add (new CodeAttributeDeclaration ("PersistenceClass", new CodeAttributeArgument (new CodeTypeReferenceExpression ("\"" + StandartName (a.TableName, false) + "\""))));
			if (UsingWCFPattern) {
				CodeAttributeDeclaration g = new CodeAttributeDeclaration ("DataContract");
				g.Arguments.Add (new CodeAttributeArgument ("Name", new CodeTypeReferenceExpression ("\"" + StandartName (a.TableName, false) + "\"")));
				g.Arguments.Add (new CodeAttributeArgument ("Namespace", new CodeTypeReferenceExpression ("\"\"")));
				f.CustomAttributes.Add (g);
			}
			foreach (CodeAttributeDeclaration attr in _classCustomAttributes)
				f.CustomAttributes.Add (attr);
			for (int h = 0; h < a.Fields.Count; h++) {
				FieldMap i = a.Fields [h];
				if (string.IsNullOrEmpty (i.ColumnName))
					continue;
				string j = StandartName (i.ColumnName, true);
				if (j.Length > 1)
					j = "_" + char.ToLower (j [0]) + j.Substring (1);
				else
					j = "_" + j.ToUpper ();
				CodeMemberField k = new CodeMemberField (i.MemberType, j);
				f.Members.Add (k);
				if (h == 0) {
					CodeRegionDirective l = new CodeRegionDirective (CodeRegionMode.Start, "Local Variables\r\n");
					k.StartDirectives.Add (l);
				}
				if (h + 1 == a.Fields.Count || a.Fields.Count == 1) {
					CodeRegionDirective m = new CodeRegionDirective (CodeRegionMode.End, "Local Variables");
					k.EndDirectives.Add (m);
				}
			}
			for (int h = 0; h < a.Fields.Count; h++) {
				FieldMap i = a.Fields [h];
				if (string.IsNullOrEmpty (i.ColumnName))
					continue;
				string j = StandartName (i.ColumnName, true);
				if (j.Length > 1)
					j = "_" + char.ToLower (j [0]) + j.Substring (1);
				else
					j = "_" + j.ToUpper ();
				string n = StandartName (i.ColumnName, true);
				Type o = i.MemberType;
				CodeMemberProperty p = new CodeMemberProperty ();
				p.Name = n;
				p.Type = new CodeTypeReference (o);
				p.Attributes = MemberAttributes.Public | MemberAttributes.Final;
				if (!string.IsNullOrEmpty (i.Comment)) {
					p.Comments.Add (new CodeCommentStatement ("<summary>", true));
					p.Comments.Add (new CodeCommentStatement (i.Comment, true));
					p.Comments.Add (new CodeCommentStatement ("</summary>", true));
				}
				CodeAttributeDeclaration q = new CodeAttributeDeclaration ("PersistenceProperty");
				q.Arguments.Add (new CodeAttributeArgument (new CodeTypeReferenceExpression ("\"" + i.ColumnName + "\"")));
				if (i.IsAutoGenerated)
					q.Arguments.Add (new CodeAttributeArgument (new CodeTypeReferenceExpression ("PersistenceParameterType.IdentityKey")));
				else if (i.IsPrimaryKey)
					q.Arguments.Add (new CodeAttributeArgument (new CodeTypeReferenceExpression ("PersistenceParameterType.Key")));
				if (!o.IsValueType && i.Size > 0) {
					q.Arguments.Add (new CodeAttributeArgument (new CodeTypeReferenceExpression (i.Size.ToString ())));
				}
				p.CustomAttributes.Add (q);
				if (!o.IsValueType && !i.IsNullable) {
					CodeAttributeDeclaration r = new CodeAttributeDeclaration ("RequiredValidator");
					p.CustomAttributes.Add (r);
				}
				if (!string.IsNullOrEmpty (i.ForeignKeyTableName)) {
					p.CustomAttributes.Add (new CodeAttributeDeclaration ("PersistenceForeignKey", new CodeAttributeArgument (new CodeTypeReferenceExpression ("typeof(" + StandartName (i.ForeignKeyTableName, true) + ")")), new CodeAttributeArgument (new CodeTypeReferenceExpression ("\"" + StandartName (i.ForeignKeyColumnName, true) + "\""))));
				}
				if (UsingWCFPattern) {
					CodeAttributeDeclaration g = new CodeAttributeDeclaration ("DataMember");
					g.Arguments.Add (new CodeAttributeArgument ("Name", new CodeTypeReferenceExpression ("\"" + p.Name + "\"")));
					if (!i.IsNullable)
						g.Arguments.Add (new CodeAttributeArgument ("IsRequired", new CodePrimitiveExpression (true)));
					p.CustomAttributes.Add (g);
				}
				foreach (CodeAttributeDeclaration attr in _propertyCustomAttributes)
					p.CustomAttributes.Add (attr);
				p.GetStatements.Add (new CodeMethodReturnStatement (new CodePropertyReferenceExpression (null, j)));
				p.SetStatements.Add (new CodeAssignStatement (new CodePropertyReferenceExpression (null, j), new CodePropertySetValueReferenceExpression ()));
				f.Members.Add (p);
				if (h == 0) {
					CodeRegionDirective l = new CodeRegionDirective (CodeRegionMode.Start, "Properties\r\n");
					p.StartDirectives.Add (l);
				}
				if (h + 1 == a.Fields.Count || a.Fields.Count == 1) {
					CodeRegionDirective m = new CodeRegionDirective (CodeRegionMode.End, "Properties");
					p.EndDirectives.Add (m);
				}
			}
			e.Types.Add (f);
			StringBuilder s = new StringBuilder ();
			StringWriter t = new StringWriter (s);
			CodeGeneratorOptions u = new CodeGeneratorOptions ();
			u.BracingStyle = "C";
			u.ElseOnClosing = true;
			CodeDomProvider v = CodeDomProvider.CreateProvider (_codeLanguage);
			v.GenerateCodeFromNamespace (e, t, u);
			string w = s.ToString ();
			w = w.Replace ("System.DateTime", "DateTime");
			int x = 0, y = 0, z = 0;
			do {
				x = w.IndexOf ("System.Nullable<", x);
				if (x >= 0) {
					y = w.IndexOf ('>', x);
					z = x + "System.Nullable<".Length;
					w = w.Substring (0, x) + w.Substring (z, y - z) + "?" + w.Substring (y + 1);
				}
			}
			while (x >= 0);
			c.Write (System.Text.Encoding.Default.GetBytes (w), 0, w.Length);
		}
	}
}
