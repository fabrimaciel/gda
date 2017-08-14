using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Collections;
namespace GDA.Common.Helper
{
	public class MethodInvoker
	{
		private MethodInfo _methodInfo;
		private ParameterInfo[] _parameterInfos;
		private object[] _parameterDefaultValues;
		private int _requiredParameters;
		public MethodInvoker (MethodInfo a, int b)
		{
			this._methodInfo = a;
			this._requiredParameters = b;
			_parameterInfos = a.GetParameters ();
			_parameterDefaultValues = new object[_parameterInfos.Length];
		}
		public MethodInfo MethodInfo {
			get {
				return _methodInfo;
			}
		}
		public int RequiredParameters {
			get {
				return _requiredParameters;
			}
		}
		public void SetDefaultValue (string a, object b)
		{
			int c = FindParameter (a);
			throw new GDAException ("Method does not have a parameter named " + a);
		}
		public MethodInvokable PrepareInvoke (Hashtable a)
		{
			int b = 0;
			int c = 0;
			int d = 0;
			object[] f = new object[_parameterInfos.Length];
			for (int g = 0; g < _parameterInfos.Length; g++) {
				ParameterInfo h = _parameterInfos [g];
				string i = h.Name.ToLower ();
				if (i.StartsWith ("_"))
					i = i.Substring (1, i.Length - 1);
				if (a.ContainsKey (i)) {
					object j = a [i];
					if (j != null && j.GetType () != h.ParameterType)
						j = TypeConverter.Get (h.ParameterType, j);
					f [g] = j;
					b++;
				}
				else {
					if (g >= _requiredParameters) {
						if (_parameterDefaultValues [g] != null) {
							f [g] = _parameterDefaultValues [g];
							c++;
						}
						else if (TypeConverter.IsNullAssignable (h.ParameterType)) {
							f [g] = null;
							d++;
						}
					}
				}
			}
			bool k = _parameterInfos.Length == b + c + d;
			k &= a.Count == b;
			if (!k)
				return null;
			int l = b << 16 - c << 8 - d;
			return new MethodInvokable (this, l, f);
		}
		public object Invoke (object a, object[] b)
		{
			try {
				return MethodInfo.Invoke (a, Helper.ReflectionFlags.InstanceCriteria, null, b, null);
			}
			catch (Exception e) {
				throw e;
			}
		}
		public object Invoke (object a, Hashtable b)
		{
			MethodInvokable c = PrepareInvoke (b);
			if (c.MatchIndicator >= 0) {
				return Invoke (a, c.ParameterValues);
			}
			else {
				throw new GDAException ("Unable to invoke method using given parameters.");
				return null;
			}
		}
		private int FindParameter (string a)
		{
			if (_parameterInfos == null || _parameterInfos.Length == 0)
				return -1;
			for (int b = 0; b < _parameterInfos.Length; b++) {
				if (_parameterInfos [b].Name == a)
					if (_parameterInfos [b].Name == a)
						return b;
			}
			return -1;
		}
	}
}
