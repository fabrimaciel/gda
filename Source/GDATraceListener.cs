using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace GDA.Diagnostics
{
	public abstract class GDATraceListener
	{
		public virtual string Name {
			get;
			set;
		}
		public virtual bool IsThreadSafe {
			get {
				return false;
			}
		}
		public abstract void NotifyBeginExecution (CommandExecutionInfo a);
		public abstract void NotifyExecution (CommandExecutionInfo a);
		public virtual void Flush ()
		{
		}
	}
}
