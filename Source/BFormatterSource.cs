using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace GDA.Helper.Serialization
{
	public class BFormatterSource : IDisposable
	{
		private Stream source;
		private int _count = 0;
		private long sourceBeginPosition = 0;
		private Type baseType = null;
		private bool localStream = false;
		private BFormatter.InfoCoreSupport[] coreSupports;
		private short memberAllowNullCount;
		public int Count {
			get {
				return _count;
			}
		}
		public BFormatterSource (Stream a, Type b)
		{
			source = a;
			localStream = false;
			this.baseType = b;
			Initialize ();
		}
		public BFormatterSource (Stream a, Type b, bool c)
		{
			var d = new byte[sizeof(int)];
			if (a.Read (d, 0, d.Length) == d.Length)
				_count = BitConverter.ToInt32 (d, 0);
			a.Seek (0, SeekOrigin.End);
			source = a;
			localStream = false;
			this.baseType = b;
			Initialize ();
		}
		private void Initialize ()
		{
			coreSupports = BFormatter.LoadTypeInformation (baseType, out memberAllowNullCount);
			sourceBeginPosition = source.Position;
			source.Write (BitConverter.GetBytes (_count), 0, sizeof(int));
		}
		public void Flush ()
		{
			long a = source.Position;
			source.Seek (sourceBeginPosition, SeekOrigin.Begin);
			source.Write (BitConverter.GetBytes ((int)_count), 0, sizeof(int));
			source.Seek (a, SeekOrigin.Begin);
		}
		public void Add (object a)
		{
			if (a == null)
				throw new ArgumentNullException ("item");
			if (a.GetType () != baseType)
				throw new InvalidOperationException ("Invalid item type.");
			long b = source.Position;
			source.Write (new byte[] {
				0,
				0,
				0,
				0
			}, 0, sizeof(int));
			BFormatter.SerializeBase (source, coreSupports, memberAllowNullCount, 0, a);
			int c = (int)(source.Position - sizeof(int) - b);
			long d = source.Position;
			source.Seek (b, SeekOrigin.Begin);
			source.Write (BitConverter.GetBytes (c), 0, sizeof(int));
			source.Seek (d, SeekOrigin.Begin);
			_count++;
		}
		public void Dispose ()
		{
			Flush ();
			if (localStream)
				source.Close ();
		}
	}
}
