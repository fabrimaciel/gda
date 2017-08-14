using System;
using System.Collections.Generic;
using System.Text;
namespace GDA
{
	public class GDATableNameRepresentNotExistsException : Exception
	{
		public GDATableNameRepresentNotExistsException (string a) : base (a)
		{
		}
	}
}
