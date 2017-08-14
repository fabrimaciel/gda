using System;
using System.Collections.Generic;
using System.Text;
namespace GDA.Common.Helper
{
	public class MethodInvokable
	{
		public readonly MethodInvoker MethodInvoker;
		public readonly int MatchIndicator;
		public readonly object[] ParameterValues;
		public MethodInvokable (MethodInvoker a, int b, object[] c)
		{
			this.MethodInvoker = a;
			this.MatchIndicator = b;
			this.ParameterValues = c;
		}
		public object Invoke (object a)
		{
			return MethodInvoker.Invoke (a, ParameterValues);
		}
	}
}
