using System;
using System.Collections.Generic;
using System.Text;
namespace GDA.Common.Configuration.Exceptions
{
	public class MissingConfigurationKeyException : GDA.GDAException
	{
		public MissingConfigurationKeyException (string a) : base ("Key {0} not found in file config.", a)
		{
		}
	}
}
