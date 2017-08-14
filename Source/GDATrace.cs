using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace GDA.Diagnostics
{
	public static class GDATrace
	{
		public static GDATraceListenerCollection Listeners {
			get {
				return GDATraceInternal.Listeners;
			}
		}
		internal static ExecutionHandler CreateExecutionHandler (System.Data.IDbCommand a)
		{
			return new ExecutionHandler (a);
		}
		public static void NotifyBeginExecution (CommandExecutionInfo a)
		{
			GDATraceInternal.NotifyBeginExecution (a);
		}
		public static void NotifyExecution (CommandExecutionInfo a)
		{
			GDATraceInternal.NotifyExecution (a);
		}
		public sealed class ExecutionHandler : IDisposable
		{
			private CommandExecutionInfo _executionInfo;
			private System.Diagnostics.Stopwatch _stopwatch = new System.Diagnostics.Stopwatch ();
			private int _rowsAffects;
			public int RowsAffects {
				get {
					return _rowsAffects;
				}
				set {
					_rowsAffects = value;
				}
			}
			public ExecutionHandler (System.Data.IDbCommand a)
			{
				_executionInfo = new CommandExecutionInfo (a);
				_stopwatch.Start ();
			}
			public void Fail (Exception a)
			{
				_stopwatch.Stop ();
				CommandExecutionInfo b = null;
				try {
					b = _executionInfo.Fail (_stopwatch.Elapsed, a);
				}
				catch (Exception ex) {
					throw new GDATraceException (string.Format ("An error occurred when get fail details for error \"{0}\".\r\n{1}", a.Message, ex.Message), ex);
				}
				GDATrace.NotifyExecution (b);
				_executionInfo = null;
			}
			public void Dispose ()
			{
				var a = _executionInfo;
				if (a != null) {
					_stopwatch.Stop ();
					GDATrace.NotifyExecution (a.Finish (_stopwatch.Elapsed, RowsAffects));
				}
			}
		}
	}
}
