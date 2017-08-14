using System.Collections;
namespace GDA.Common.Helper
{
	public class MethodDispatcher
	{
		private object _lock = new object ();
		private IList invokers;
		private Hashtable dispatchCache;
		public MethodDispatcher (MethodInvoker a)
		{
			invokers = new ArrayList ();
			invokers.Add (a);
			dispatchCache = new Hashtable ();
		}
		public void AddInvoker (MethodInvoker a)
		{
			lock (_lock) {
				invokers.Add (a);
			}
		}
		public object Invoke (object a, Hashtable b)
		{
			MethodInvokable c = DetermineBestMatch (b);
			if (c == null)
				throw new GDAException ("No compatible method found to invoke for the given parameters.");
			return c.Invoke (a);
		}
		private MethodInvokable DetermineBestMatch (Hashtable a)
		{
			MethodInvokable b = null;
			foreach (MethodInvoker invoker in invokers) {
				MethodInvokable c = invoker.PrepareInvoke (a);
				bool d = b == null && c != null && c.MatchIndicator > 0;
				d |= b != null && c != null && c.MatchIndicator > b.MatchIndicator;
				if (d)
					b = c;
			}
			return b;
		}
	}
}
