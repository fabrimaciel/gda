using System;
using System.Collections.Generic;
using System.Text;
namespace GDA
{
	public class GDAMappingException : GDAException
	{
		public GDAMappingException (string a) : base (a)
		{
		}
		public GDAMappingException (Exception a) : base (a.Message, a)
		{
		}
		public GDAMappingException (string a, Exception b) : base (a, b)
		{
		}
		public GDAMappingException (string a, params object[] b) : this (String.Format (a, b))
		{
		}
	}
}
