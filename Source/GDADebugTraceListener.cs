using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace GDA.Diagnostics
{
	public class GDADebugTraceListener : GDATraceListener
	{
		public override bool IsThreadSafe {
			get {
				return true;
			}
		}
		public override void NotifyBeginExecution (CommandExecutionInfo a)
		{
		}
		public override void NotifyExecution (CommandExecutionInfo a)
		{
		}
	}
}
