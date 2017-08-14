using System;
using System.Collections.Generic;
using System.Text;
namespace GDA.Mapping
{
	public class TypeInfo
	{
		public string Name {
			get;
			set;
		}
		public string Assembly {
			get;
			set;
		}
		public string Namespace {
			get;
			set;
		}
		public string FullnameWithAssembly {
			get {
				return (!string.IsNullOrEmpty (Namespace) ? Namespace + "." : "") + Name + (!string.IsNullOrEmpty (Assembly) ? ", " + Assembly : "");
			}
		}
		public string Fullname {
			get {
				return (!string.IsNullOrEmpty (Namespace) ? Namespace + "." : "") + Name;
			}
		}
		public TypeInfo (string a, string b, string c)
		{
			if (string.IsNullOrEmpty (a))
				throw new ArgumentNullException ("name");
			if (!string.IsNullOrEmpty (b))
				Namespace = b;
			if (!string.IsNullOrEmpty (c))
				Assembly = c;
			var d = a.Split (',');
			for (int e = 0; e < d.Length; e++)
				d [e] = d [e].Trim ();
			var f = d [0].Split ('.');
			for (int e = 0; e < f.Length; e++)
				f [e] = f [e].Trim ();
			if (f.Length > 1) {
				Name = ElementMapping.Last (f);
				var g = string.Join (".", f, 0, f.Length - 1);
				if (d.Length > 1 || string.IsNullOrEmpty (Namespace)) {
					Namespace = g;
					Assembly = d [1];
					return;
				}
				else if (g.IndexOf (Namespace) < 0)
					Namespace += (Namespace.EndsWith (".") ? "" : ".") + g;
			}
			else
				Name = a;
			if (d.Length > 1 && string.IsNullOrEmpty (Assembly))
				Assembly = d [1];
		}
		public TypeInfo (Type a)
		{
			Name = a.Name;
			Namespace = a.Namespace;
			Assembly = a.Assembly.FullName;
		}
		public TypeInfo (string a)
		{
			if (string.IsNullOrEmpty (a))
				throw new ArgumentNullException ("typeFullname");
			var b = a.Split (',');
			for (int c = 0; c < b.Length; c++)
				b [c] = b [c].Trim ();
			var d = b [0].Split ('.');
			for (int c = 0; c < d.Length; c++)
				d [c] = d [c].Trim ();
			if (d.Length > 1) {
				Name = ElementMapping.Last (d);
				var e = string.Join (".", d, 0, d.Length - 1);
				if (b.Length > 1 || string.IsNullOrEmpty (Namespace)) {
					Namespace = e;
					Assembly = b [1];
					return;
				}
				else if (e.IndexOf (Namespace) < 0)
					Namespace += (Namespace.EndsWith (".") ? "" : ".") + e;
			}
			else
				Name = a;
			if (b.Length > 1 && string.IsNullOrEmpty (Assembly))
				Assembly = b [1];
		}
	}
}
