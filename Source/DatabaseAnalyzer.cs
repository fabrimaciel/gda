using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using GDA.Interfaces;
namespace GDA.Analysis
{
	public abstract class DatabaseAnalyzer
	{
		private bool done = false;
		private ForeignKeyList _foreignKeys = new ForeignKeyList ();
		protected Hashtable tablesMaps;
		protected List<string> tablesNames;
		private IProviderConfiguration providerConfiguration;
		protected IProviderConfiguration ProviderConfiguration {
			get {
				return providerConfiguration;
			}
		}
		public DatabaseAnalyzer (IProviderConfiguration a)
		{
			this.providerConfiguration = a;
			tablesMaps = new Hashtable ();
		}
		public abstract void Analyze (string a);
		public virtual void Analyze (string a, string b)
		{
			throw new NotSupportedException ();
		}
		[System.Obsolete ("Use GetTableMap(string tableName, string schema)")]
		public virtual TableMap GetTableMap (string a)
		{
			if (a == null)
				return null;
			a = a.ToLower ();
			return tablesMaps.ContainsKey (a) ? tablesMaps [a] as TableMap : null;
		}
		public virtual TableMap GetTableMap (string a, string b)
		{
			if (a == null)
				return null;
			a = a.ToLower ();
			TableMap c = tablesMaps [a] as TableMap;
			if (c != null && string.Compare (c.TableSchema, b, true) == 0)
				return c;
			return null;
		}
		public virtual IList<string> GetTablesName ()
		{
			if (tablesNames == null)
				tablesNames = new List<string> ();
			if (tablesMaps.Count != tablesNames.Count) {
				tablesNames.Clear ();
				foreach (string tn in tablesMaps.Keys)
					tablesNames.Add (tn);
			}
			return tablesNames;
		}
		public Hashtable TablesMaps {
			get {
				return tablesMaps;
			}
		}
		public ForeignKeyList ForeignKeys {
			get {
				return _foreignKeys;
			}
		}
		public IEnumerable<TableMap> Tables {
			get {
				foreach (DictionaryEntry de in TablesMaps)
					yield return (TableMap)de.Value;
			}
		}
		#if !PocketPC
		public void GenerateCode (string a, string b, string c, string d)
		{
			if (string.IsNullOrEmpty (a))
				throw new ArgumentNullException ("language");
			else if (string.IsNullOrEmpty (b))
				throw new ArgumentNullException ("namespaceName");
			else if (string.IsNullOrEmpty (d))
				throw new ArgumentNullException ("directoryName");
			if (!System.IO.Directory.Exists (d))
				throw new System.IO.DirectoryNotFoundException (string.Format ("Directory {0} not found.", d));
			GDA.Analysis.Generator e = new Generator ();
			e.CodeLanguage = a;
			e.NamespaceName = b;
			this.Analyze (null);
			string f = System.CodeDom.Compiler.CodeDomProvider.CreateProvider (a).FileExtension;
			foreach (TableMap map in this.TablesMaps.Values)
				e.Generate (map, c, d + "\\" + Generator.StandartName (map.TableName, true) + "." + f);
		}
	#endif
	}
}
