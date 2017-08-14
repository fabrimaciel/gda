using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace GDA.Diagnostics
{
	public class GDATraceException : GDAException
	{
		public GDATraceException (Exception a) : base (a.Message, a)
		{
		}
		public GDATraceException (string a) : base (a)
		{
		}
		public GDATraceException (string a, Exception b) : base (a, b)
		{
		}
	}
}
