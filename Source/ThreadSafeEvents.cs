using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
namespace GDA.Helper
{
	public static class ThreadSafeEvents
	{
		public static void FireEvent<T> (Delegate a, object b, T c)
		{
			#if PocketPC
						            throw new NotImplementedException();
#else
			if (a != null) {
				foreach (Delegate singleCast in a.GetInvocationList ()) {
					ISynchronizeInvoke d = singleCast.Target as ISynchronizeInvoke;
					try {
						if (d != null && d.InvokeRequired)
							d.Invoke (a, new object[] {
								b,
								c
							});
						else
							singleCast.DynamicInvoke (new object[] {
								b,
								c
							});
					}
					catch {
					}
				}
			}
			#endif
		}
	}
}
