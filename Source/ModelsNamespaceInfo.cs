using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
namespace GDA
{
	public class ModelsNamespaceInfo
	{
		public readonly string Namespace;
		public readonly string AssemblyName;
		private Assembly _currentAssembly;
		public Assembly CurrentAssembly {
			get {
				LoadCurrentAssembly ();
				return _currentAssembly;
			}
		}
		private void LoadCurrentAssembly ()
		{
			if (_currentAssembly == null) {
				if (AssemblyName == "*")
					#if PocketPC
										                    // Carrega o assembly da aplicação que está utilizado o gda
                    _currentAssembly = System.Reflection.Assembly.GetExecutingAssembly();
#else
					_currentAssembly = System.Reflection.Assembly.GetEntryAssembly ();
				#endif
				else {
					#if PocketPC
										                    // Carrega o assembly com os dados completos.
                    _currentAssembly = Assembly.Load(AssemblyName);
#else
					var a = System.Reflection.Assembly.GetEntryAssembly ();
					if (a != null) {
						AssemblyName[] b = System.Reflection.Assembly.GetEntryAssembly ().GetReferencedAssemblies ();
						foreach (AssemblyName an in b) {
							if (an.Name == AssemblyName) {
								_currentAssembly = System.Reflection.Assembly.Load (an);
								return;
							}
						}
					}
					if (AssemblyName.IndexOf (',') == -1) {
						_currentAssembly = Assembly.LoadWithPartialName (AssemblyName);
					}
					else {
						_currentAssembly = Assembly.Load (AssemblyName);
					}
					#endif
				}
			}
		}
		public ModelsNamespaceInfo (string a, string b)
		{
			if (string.IsNullOrEmpty (b))
				throw new ArgumentNullException ("assemblyName");
			Namespace = a;
			AssemblyName = b;
		}
		public ModelsNamespaceInfo (string a, Assembly b)
		{
			if (b == null)
				throw new ArgumentNullException ("currentAssembly");
			_currentAssembly = b;
			Namespace = a;
		}
	}
}
