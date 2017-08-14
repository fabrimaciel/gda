using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace GDA.Diagnostics
{
	class GDATraceInternal
	{
		internal static readonly object _critSec;
		private static bool _useGlobalLock;
		private static bool _autoFlush;
		private static GDATraceListenerCollection _listeners;
		public static bool UseGlobalLock {
			get {
				InitializeSettings ();
				return _useGlobalLock;
			}
			set {
				InitializeSettings ();
				_useGlobalLock = value;
			}
		}
		public static GDATraceListenerCollection Listeners {
			get {
				InitializeSettings ();
				if (_listeners == null) {
					lock (_critSec) {
						if (_listeners == null) {
							_listeners = new GDATraceListenerCollection ();
						}
					}
				}
				return _listeners;
			}
		}
		public static bool AutoFlush {
			get {
				InitializeSettings ();
				return _autoFlush;
			}
			set {
				InitializeSettings ();
				_autoFlush = value;
			}
		}
		static GDATraceInternal ()
		{
			_critSec = new object ();
		}
		private static void InitializeSettings ()
		{
			_useGlobalLock = true;
		}
		public static void NotifyBeginExecution (CommandExecutionInfo a)
		{
			try {
				if (UseGlobalLock) {
					lock (_critSec) {
						foreach (GDATraceListener listener in Listeners) {
							listener.NotifyBeginExecution (a);
							if (AutoFlush)
								listener.Flush ();
						}
						return;
					}
				}
				foreach (GDATraceListener listener2 in Listeners) {
					if (!listener2.IsThreadSafe) {
						lock (listener2) {
							listener2.NotifyBeginExecution (a);
							if (AutoFlush)
								listener2.Flush ();
							continue;
						}
					}
					listener2.NotifyBeginExecution (a);
					if (AutoFlush)
						listener2.Flush ();
				}
			}
			catch (Exception ex) {
				throw new GDATraceException (string.Format ("GDA Trace notify begin execution error. {0}", ex.Message), ex);
			}
		}
		public static void NotifyExecution (CommandExecutionInfo a)
		{
			try {
				if (UseGlobalLock) {
					lock (_critSec) {
						foreach (GDATraceListener listener in Listeners) {
							listener.NotifyExecution (a);
							if (AutoFlush)
								listener.Flush ();
						}
						return;
					}
				}
				foreach (GDATraceListener listener2 in Listeners) {
					if (!listener2.IsThreadSafe) {
						lock (listener2) {
							listener2.NotifyExecution (a);
							if (AutoFlush)
								listener2.Flush ();
							continue;
						}
					}
					listener2.NotifyExecution (a);
					if (AutoFlush)
						listener2.Flush ();
				}
			}
			catch (Exception ex) {
				throw new GDATraceException (string.Format ("GDA Trace notify execution error. {0}", ex.Message), ex);
			}
		}
	}
}
