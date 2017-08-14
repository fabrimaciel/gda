using System;
using System.Collections.Generic;
using System.Text;
namespace GDA
{
	class GDAConditionalClauseException : Exception
	{
		public GDAConditionalClauseException (string a) : base (a)
		{
		}
	}
}
